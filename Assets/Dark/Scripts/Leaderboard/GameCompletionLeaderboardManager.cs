using System;
using System.Collections;
using System.Collections.Generic;
using Dark.Scripts.STOVE;
using InGame.CharacterClass;
using UnityEngine;
using static Stove.PCSDK.GameSupport;

namespace Dark.Scripts.Leaderboard
{
    /// <summary>
    /// STOVE leaderboard adapter.
    /// STOVE rankings are backed by stats, so `stoveStatId` is used for score uploads
    /// and `stoveLeaderboardId` is used for rank queries.
    /// </summary>
    public class GameCompletionLeaderboardManager : MonoBehaviour
    {
        [Tooltip("Legacy Steam leaderboard name. Used as a STOVE ID fallback to keep existing prefab data valid.")]
        public string leaderboardName;

        [Header("STOVE")]
        [SerializeField] private string stoveLeaderboardId;
        [SerializeField] private string stoveStatId;
        [SerializeField] private string fallbackPlayerName = "Player";
        [SerializeField] private bool enableDebugLogs = true;

        [Header("Display")]
        [SerializeField] private bool useConfiguredClassForEntries;
        [SerializeField] private CharacterClass configuredClass;

        private bool leaderboardReady;
        private bool initializeStarted;

        private readonly List<LeaderboardEntryData> offlineLeaderboard = new List<LeaderboardEntryData>();

        public event Action<List<LeaderboardEntryData>> OnTopScoresDownloaded;
        public event Action<List<LeaderboardEntryData>> OnPlayerScoresDownloaded;
        public event Action OnPlayerScoreUploaded;

        private string LeaderboardId => string.IsNullOrWhiteSpace(stoveLeaderboardId)
            ? leaderboardName
            : stoveLeaderboardId;

        private string StatId => string.IsNullOrWhiteSpace(stoveStatId)
            ? leaderboardName
            : stoveStatId;

        private string DebugName => string.IsNullOrWhiteSpace(name) ? leaderboardName : name;

        public void SetConfiguredClass(CharacterClass characterClass)
        {
            configuredClass = characterClass;
            useConfiguredClassForEntries = true;
        }

        public void Initialize()
        {
            if (initializeStarted) return;

            initializeStarted = true;
            leaderboardReady = false;
            Log($"Initialize requested. leaderboardName='{leaderboardName}', stoveLeaderboardId='{stoveLeaderboardId}', stoveStatId='{stoveStatId}'.");
            StartCoroutine(IEInitialize());
        }

        private IEnumerator IEInitialize()
        {
            var stoveManager = STOVEPCSDK3Manager.Instance;

            while (stoveManager.IsInitializing)
                yield return null;

            if (!stoveManager.IsGameSupportInitialized)
            {
                Debug.LogWarning($"STOVE leaderboard '{name}' skipped because GameSupport SDK is not initialized.");
                yield break;
            }

            if (string.IsNullOrWhiteSpace(LeaderboardId) || string.IsNullOrWhiteSpace(StatId))
            {
                Debug.LogWarning($"STOVE leaderboard '{name}' skipped because leaderboard/stat ID is missing.");
                yield break;
            }

            leaderboardReady = true;
            Log($"Ready. LeaderboardId='{LeaderboardId}', StatId='{StatId}', configuredClass='{configuredClass}', useConfiguredClassForEntries={useConfiguredClassForEntries}.");
        }

        public void UploadScore(int score, int[] details = null)
        {
            if (!CanUseGameSupport())
                return;

            try
            {
                Log($"Uploading score. StatId='{StatId}', score={score}, details=[{FormatDetails(details)}].");
                GameSupport_ModifyStat(StatId, score, (callbackResult, _) =>
                {
                    STOVEPCSDK3Manager.Instance.PrintCallbackResult(callbackResult);
                    Log($"Upload callback. StatId='{StatId}', score={score}, success={callbackResult.result.IsSuccessful()}, resultCode={callbackResult.result.resultCode}, error='{callbackResult.errorMessage}'.");

                    if (!callbackResult.result.IsSuccessful())
                        return;

                    OnPlayerScoreUploaded?.Invoke();
                    OnPlayerScoreUploaded = null;
                });
            }
            catch (Exception exception)
            {
                Debug.LogError($"STOVE stat upload failed for '{StatId}': {exception}");
            }
        }

        public void DownloadTop(int count)
        {
            if (!CanUseGameSupport())
                return;

            Log($"DownloadTop requested. count={count}.");
            DownloadRankPage(1, Mathf.Max(1, count), includeMyRank: false, OnTopScoresDownloaded);
        }

        public void DownloadAroundPlayer(int range)
        {
            if (!CanUseGameSupport())
                return;

            // STOVE PC SDK 3 does not expose Steam's global-around-user request.
            // includeMyRank returns the current user's rank entry, which is all the UI needs here.
            var pageSize = Mathf.Max(1, (range * 2) + 1);
            Log($"DownloadAroundPlayer requested. range={range}, pageSize={pageSize}.");
            DownloadRankPage(1, pageSize, includeMyRank: true, OnPlayerScoresDownloaded);
        }

        private void DownloadRankPage(int pageIndex, int pageSize, bool includeMyRank,
            Action<List<LeaderboardEntryData>> onDownloaded)
        {
            try
            {
                var rankParams = new StovePCRankParams
                {
                    leaderboardId = LeaderboardId,
                    pageIndex = (uint)Mathf.Max(1, pageIndex),
                    pageSize = (uint)Mathf.Max(1, pageSize),
                    includeMyRank = includeMyRank
                };

                Log($"Rank request. leaderboardId='{rankParams.leaderboardId}', pageIndex={rankParams.pageIndex}, pageSize={rankParams.pageSize}, includeMyRank={rankParams.includeMyRank}.");
                GameSupport_Rank(rankParams, (callbackResult, ranks, _) =>
                {
                    STOVEPCSDK3Manager.Instance.PrintCallbackResult(callbackResult);
                    Log($"Rank callback. leaderboardId='{LeaderboardId}', includeMyRank={includeMyRank}, success={callbackResult.result.IsSuccessful()}, resultCode={callbackResult.result.resultCode}, error='{callbackResult.errorMessage}', rawCount={(ranks == null ? 0 : ranks.Length)}.");

                    if (!callbackResult.result.IsSuccessful())
                    {
                        onDownloaded?.Invoke(new List<LeaderboardEntryData>());
                        return;
                    }

                    var entries = ConvertRanks(ranks, includeMyRank);
                    LogRankEntries(entries, includeMyRank);
                    onDownloaded?.Invoke(entries);
                });
            }
            catch (Exception exception)
            {
                Debug.LogError($"STOVE rank download failed for '{LeaderboardId}': {exception}");
                onDownloaded?.Invoke(new List<LeaderboardEntryData>());
            }
        }

        private List<LeaderboardEntryData> ConvertRanks(StovePCRank[] ranks, bool firstEntryIsCurrentPlayer)
        {
            var entries = new List<LeaderboardEntryData>();

            if (ranks == null)
                return entries;

            for (var i = 0; i < ranks.Length; i++)
            {
                var rank = ranks[i];
                entries.Add(new LeaderboardEntryData
                {
                    rank = (int)rank.rank,
                    score = (int)rank.score,
                    playerName = string.IsNullOrWhiteSpace(rank.nickname) ? fallbackPlayerName : rank.nickname,
                    classType = useConfiguredClassForEntries ? configuredClass : (CharacterClass)(-1),
                    isCurrentPlayer = firstEntryIsCurrentPlayer && i == 0
                });
            }

            return entries;
        }

        private bool CanUseGameSupport()
        {
            if (!leaderboardReady)
            {
                Log($"GameSupport call skipped. Leaderboard is not ready yet. LeaderboardId='{LeaderboardId}', StatId='{StatId}'.");
                return false;
            }

            if (!STOVEPCSDK3Manager.Instance.IsGameSupportInitialized)
            {
                Log($"GameSupport call skipped. GameSupport SDK is not initialized. LeaderboardId='{LeaderboardId}', StatId='{StatId}'.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(LeaderboardId) || string.IsNullOrWhiteSpace(StatId))
            {
                Log($"GameSupport call skipped. Missing ID. LeaderboardId='{LeaderboardId}', StatId='{StatId}'.");
                return false;
            }

            return true;
        }

        private void LogRankEntries(IReadOnlyList<LeaderboardEntryData> entries, bool includeMyRank)
        {
            if (!enableDebugLogs)
                return;

            Debug.Log($"[STOVE Leaderboard:{DebugName}] Converted rank entries. includeMyRank={includeMyRank}, count={(entries == null ? 0 : entries.Count)}.");

            if (entries == null)
                return;

            var count = Mathf.Min(entries.Count, 10);
            for (var i = 0; i < count; i++)
            {
                var entry = entries[i];
                Debug.Log($"[STOVE Leaderboard:{DebugName}] Entry[{i}] rank={entry.rank}, score={entry.score}, name='{entry.playerName}', class={entry.classType}, isCurrentPlayer={entry.isCurrentPlayer}.");
            }
        }

        private void Log(string message)
        {
            if (!enableDebugLogs)
                return;

            Debug.Log($"[STOVE Leaderboard:{DebugName}] {message}");
        }

        private static string FormatDetails(int[] details)
        {
            if (details == null || details.Length == 0)
                return string.Empty;

            return string.Join(",", details);
        }

        void AddOfflineScore(int score)
        {
            offlineLeaderboard.Add(new LeaderboardEntryData
            {
                playerName = fallbackPlayerName,
                score = score
            });

            offlineLeaderboard.Sort((a, b) => b.score.CompareTo(a.score));

            for (int i = 0; i < offlineLeaderboard.Count; i++)
                offlineLeaderboard[i].rank = i + 1;
        }

        List<LeaderboardEntryData> GetOfflineNeighbors(int range)
        {
            if (offlineLeaderboard.Count == 0)
                return new List<LeaderboardEntryData>();

            int playerIndex = 0;

            int start = Mathf.Max(0, playerIndex - range);
            int end = Mathf.Min(offlineLeaderboard.Count - 1, playerIndex + range);

            return offlineLeaderboard.GetRange(start, end - start + 1);
        }
    }
}
