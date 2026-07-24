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
        private readonly Dictionary<CharacterClass, GameCompletionLeaderboardManager> mergedClassLeaderboards =
            new Dictionary<CharacterClass, GameCompletionLeaderboardManager>();
        private readonly List<LeaderboardEntryData> lastMergedTopEntries = new List<LeaderboardEntryData>();
        private bool useMergedClassLeaderboards;

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

        public void SetMergedClassLeaderboards(
            IReadOnlyDictionary<CharacterClass, GameCompletionLeaderboardManager> classLeaderboards)
        {
            mergedClassLeaderboards.Clear();

            if (classLeaderboards != null)
            {
                foreach (var leaderboard in classLeaderboards)
                {
                    if (leaderboard.Value == null || leaderboard.Value == this)
                        continue;

                    mergedClassLeaderboards[leaderboard.Key] = leaderboard.Value;
                }
            }

            useMergedClassLeaderboards = mergedClassLeaderboards.Count > 0;
            Log($"Merged class leaderboard mode set. enabled={useMergedClassLeaderboards}, sourceCount={mergedClassLeaderboards.Count}.");
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

            if (!useMergedClassLeaderboards && (string.IsNullOrWhiteSpace(LeaderboardId) || string.IsNullOrWhiteSpace(StatId)))
            {
                Debug.LogWarning($"STOVE leaderboard '{name}' skipped because leaderboard/stat ID is missing.");
                yield break;
            }

            leaderboardReady = true;
            Log($"Ready. LeaderboardId='{LeaderboardId}', StatId='{StatId}', configuredClass='{configuredClass}', useConfiguredClassForEntries={useConfiguredClassForEntries}, useMergedClassLeaderboards={useMergedClassLeaderboards}.");
        }

        public void UploadScore(int score, int[] details = null)
        {
            if (!CanUseGameSupport())
                return;

            STOVEPCSDK3GameSupportRequestQueue.Enqueue(complete =>
            {
                try
                {
                    Log($"Uploading score. StatId='{StatId}', score={score}, details=[{FormatDetails(details)}].");
                    GameSupport_ModifyStat(StatId, score, (callbackResult, _) =>
                    {
                        try
                        {
                            STOVEPCSDK3Manager.Instance.PrintCallbackResult(callbackResult);
                            Log($"Upload callback. StatId='{StatId}', score={score}, success={callbackResult.result.IsSuccessful()}, resultCode={callbackResult.result.resultCode}, error='{callbackResult.errorMessage}'.");

                            if (!callbackResult.result.IsSuccessful())
                                return;

                            OnPlayerScoreUploaded?.Invoke();
                            OnPlayerScoreUploaded = null;
                        }
                        finally
                        {
                            complete();
                        }
                    });
                }
                catch (Exception exception)
                {
                    Debug.LogError($"STOVE stat upload failed for '{StatId}': {exception}");
                    complete();
                }
            });
        }

        public void DownloadTop(int count)
        {
            if (useMergedClassLeaderboards)
            {
                DownloadMergedTop(count);
                return;
            }

            if (!CanUseGameSupport())
                return;

            Log($"DownloadTop requested. count={count}.");
            DownloadRankPage(1, Mathf.Max(1, count), includeMyRank: false, OnTopScoresDownloaded);
        }

        public void DownloadAroundPlayer(int range)
        {
            if (useMergedClassLeaderboards)
            {
                DownloadMergedAroundPlayer(range);
                return;
            }

            if (!CanUseGameSupport())
                return;

            // STOVE PC SDK 3 does not expose Steam's global-around-user request.
            // includeMyRank returns the current user's rank entry, which is all the UI needs here.
            var pageSize = Mathf.Max(1, (range * 2) + 1);
            Log($"DownloadAroundPlayer requested. range={range}, pageSize={pageSize}.");
            DownloadRankPage(1, pageSize, includeMyRank: true, OnPlayerScoresDownloaded);
        }

        private void DownloadTopEntries(int count, Action<List<LeaderboardEntryData>> onDownloaded)
        {
            if (useMergedClassLeaderboards)
            {
                DownloadMergedTop(count, onDownloaded);
                return;
            }

            if (!CanUseGameSupport())
            {
                onDownloaded?.Invoke(new List<LeaderboardEntryData>());
                return;
            }

            DownloadRankPage(1, Mathf.Max(1, count), includeMyRank: false, onDownloaded);
        }

        private void DownloadCurrentPlayerEntry(Action<LeaderboardEntryData> onDownloaded)
        {
            if (!CanUseGameSupport())
            {
                onDownloaded?.Invoke(null);
                return;
            }

            DownloadRankPage(1, 1, includeMyRank: true, entries =>
            {
                onDownloaded?.Invoke(entries != null && entries.Count > 0 ? entries[0] : null);
            });
        }

        private void DownloadMergedTop(int count)
        {
            DownloadMergedTop(count, OnTopScoresDownloaded);
        }

        private void DownloadMergedTop(int count, Action<List<LeaderboardEntryData>> onDownloaded)
        {
            if (!CanUseMergedClassLeaderboards())
            {
                onDownloaded?.Invoke(new List<LeaderboardEntryData>());
                return;
            }

            var pageSize = Mathf.Max(1, count);
            var pendingDownloads = mergedClassLeaderboards.Count;
            var mergedEntries = new List<LeaderboardEntryData>();

            Log($"Merged DownloadTop requested. count={pageSize}, sourceCount={pendingDownloads}.");

            foreach (var source in mergedClassLeaderboards)
            {
                source.Value.DownloadTopEntries(pageSize, entries =>
                {
                    if (entries != null)
                    {
                        for (var i = 0; i < entries.Count; i++)
                        {
                            if (entries[i] == null)
                                continue;

                            var entry = CloneEntry(entries[i]);
                            entry.classType = source.Key;
                            mergedEntries.Add(entry);
                        }
                    }

                    pendingDownloads--;

                    if (pendingDownloads > 0)
                        return;

                    var result = MergeClassEntries(mergedEntries, pageSize);
                    lastMergedTopEntries.Clear();
                    lastMergedTopEntries.AddRange(result);
                    Log($"Merged DownloadTop complete. rawCount={mergedEntries.Count}, mergedCount={result.Count}.");
                    onDownloaded?.Invoke(result);
                });
            }
        }

        private void DownloadMergedAroundPlayer(int range)
        {
            if (!CanUseMergedClassLeaderboards())
            {
                OnPlayerScoresDownloaded?.Invoke(new List<LeaderboardEntryData>());
                return;
            }

            var pendingDownloads = mergedClassLeaderboards.Count;
            var currentPlayerEntries = new List<LeaderboardEntryData>();

            Log($"Merged DownloadAroundPlayer requested. range={range}, sourceCount={pendingDownloads}.");

            foreach (var source in mergedClassLeaderboards)
            {
                source.Value.DownloadCurrentPlayerEntry(entry =>
                {
                    if (entry != null)
                    {
                        var classEntry = CloneEntry(entry);
                        classEntry.classType = source.Key;
                        currentPlayerEntries.Add(classEntry);
                    }

                    pendingDownloads--;

                    if (pendingDownloads > 0)
                        return;

                    var bestEntry = SelectHighestScoreEntry(currentPlayerEntries);

                    if (bestEntry != null)
                    {
                        var mergedTopEntry = FindEntryInLastMergedTop(bestEntry);

                        if (mergedTopEntry != null)
                            bestEntry.rank = mergedTopEntry.rank;

                        bestEntry.isCurrentPlayer = true;
                    }

                    OnPlayerScoresDownloaded?.Invoke(bestEntry == null
                        ? new List<LeaderboardEntryData>()
                        : new List<LeaderboardEntryData> { bestEntry });
                });
            }
        }

        private void DownloadRankPage(int pageIndex, int pageSize, bool includeMyRank,
            Action<List<LeaderboardEntryData>> onDownloaded)
        {
            STOVEPCSDK3GameSupportRequestQueue.Enqueue(complete =>
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
                        try
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
                        }
                        finally
                        {
                            complete();
                        }
                    });
                }
                catch (Exception exception)
                {
                    Debug.LogError($"STOVE rank download failed for '{LeaderboardId}': {exception}");
                    onDownloaded?.Invoke(new List<LeaderboardEntryData>());
                    complete();
                }
            });
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

        private bool CanUseMergedClassLeaderboards()
        {
            if (!leaderboardReady)
            {
                Log("Merged leaderboard call skipped. Leaderboard is not ready yet.");
                return false;
            }

            if (!STOVEPCSDK3Manager.Instance.IsGameSupportInitialized)
            {
                Log("Merged leaderboard call skipped. GameSupport SDK is not initialized.");
                return false;
            }

            if (mergedClassLeaderboards.Count <= 0)
            {
                Log("Merged leaderboard call skipped. No class leaderboard sources are configured.");
                return false;
            }

            return true;
        }

        private static List<LeaderboardEntryData> MergeClassEntries(
            IReadOnlyList<LeaderboardEntryData> entries,
            int maxCount)
        {
            var bestByPlayer = new Dictionary<string, LeaderboardEntryData>(StringComparer.OrdinalIgnoreCase);

            if (entries != null)
            {
                for (var i = 0; i < entries.Count; i++)
                {
                    var entry = entries[i];

                    if (entry == null)
                        continue;

                    var key = GetPlayerKey(entry);

                    if (!bestByPlayer.TryGetValue(key, out var existing) ||
                        entry.score > existing.score ||
                        entry.score == existing.score && entry.rank < existing.rank)
                    {
                        bestByPlayer[key] = CloneEntry(entry);
                    }
                }
            }

            var merged = new List<LeaderboardEntryData>(bestByPlayer.Values);
            merged.Sort((a, b) =>
            {
                var scoreCompare = b.score.CompareTo(a.score);

                if (scoreCompare != 0)
                    return scoreCompare;

                var nameCompare = string.Compare(a.playerName, b.playerName, StringComparison.OrdinalIgnoreCase);

                if (nameCompare != 0)
                    return nameCompare;

                return a.rank.CompareTo(b.rank);
            });

            var count = Mathf.Min(Mathf.Max(0, maxCount), merged.Count);

            if (count < merged.Count)
                merged.RemoveRange(count, merged.Count - count);

            for (var i = 0; i < merged.Count; i++)
                merged[i].rank = i + 1;

            return merged;
        }

        private static LeaderboardEntryData SelectHighestScoreEntry(IReadOnlyList<LeaderboardEntryData> entries)
        {
            LeaderboardEntryData bestEntry = null;

            if (entries == null)
                return null;

            for (var i = 0; i < entries.Count; i++)
            {
                var entry = entries[i];

                if (entry == null)
                    continue;

                if (bestEntry == null ||
                    entry.score > bestEntry.score ||
                    entry.score == bestEntry.score && entry.rank < bestEntry.rank)
                {
                    bestEntry = CloneEntry(entry);
                }
            }

            return bestEntry;
        }

        private LeaderboardEntryData FindEntryInLastMergedTop(LeaderboardEntryData target)
        {
            if (target == null)
                return null;

            var targetKey = GetPlayerKey(target);

            for (var i = 0; i < lastMergedTopEntries.Count; i++)
            {
                var entry = lastMergedTopEntries[i];

                if (entry != null && string.Equals(GetPlayerKey(entry), targetKey, StringComparison.OrdinalIgnoreCase))
                    return entry;
            }

            return null;
        }

        private static LeaderboardEntryData CloneEntry(LeaderboardEntryData entry)
        {
            if (entry == null)
                return null;

            return new LeaderboardEntryData
            {
                rank = entry.rank,
                score = entry.score,
                playerName = entry.playerName,
                classType = entry.classType,
                isShowFullScoreText = entry.isShowFullScoreText,
                isCurrentPlayer = entry.isCurrentPlayer
            };
        }

        private static string GetPlayerKey(LeaderboardEntryData entry)
        {
            if (entry == null)
                return string.Empty;

            if (!string.IsNullOrWhiteSpace(entry.playerName))
                return entry.playerName.Trim();

            return $"{entry.classType}:{entry.rank}:{entry.score}";
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
