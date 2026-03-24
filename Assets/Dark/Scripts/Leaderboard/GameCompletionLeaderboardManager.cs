using System;
using System.Collections;
using Core;
using System.Collections.Generic;
using InGame.CharacterClass;
using Sirenix.OdinInspector;
using Steamworks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Dark.Scripts.Leaderboard
{
    /// <summary>
    /// detail[0] dùng để lưu class
    /// </summary>
    public class GameCompletionLeaderboardManager : MonoBehaviour
    {
        public string leaderboardName;
        
        bool leaderboardReady;
        private bool initializeStarted;

        SteamLeaderboard_t leaderboard;

        CallResult<LeaderboardFindResult_t> findLeaderboard;
        CallResult<LeaderboardScoreUploaded_t> uploadResult;
        CallResult<LeaderboardScoresDownloaded_t> downloadTopResult;
        CallResult<LeaderboardScoresDownloaded_t> downloadAroundPlayerResult;

        List<LeaderboardEntryData> offlineLeaderboard = new List<LeaderboardEntryData>();
        public event Action<List<LeaderboardEntryData>> OnTopScoresDownloaded;
        public event Action<List<LeaderboardEntryData>> OnPlayerScoresDownloaded;
        public event Action OnPlayerScoreUploaded;
        
        // -----------------------------
        // Initialize
        // -----------------------------

        public void Initialize()
        {
            if (initializeStarted) return;
            initializeStarted = true;
            leaderboardReady = false;
            StartCoroutine(IEInitialize(leaderboardName));
        }

        private IEnumerator IEInitialize(string leaderboardName)
        {
            yield return new WaitUntil(() => SteamManager.Initialized);
            
            findLeaderboard = CallResult<LeaderboardFindResult_t>.Create(OnLeaderboardFound);
            uploadResult = CallResult<LeaderboardScoreUploaded_t>.Create(OnScoreUploaded);
            downloadTopResult = CallResult<LeaderboardScoresDownloaded_t>.Create(OnTopScoresDownloadedInternal);
            downloadAroundPlayerResult = CallResult<LeaderboardScoresDownloaded_t>.Create(OnPlayerScoresDownloadedInternal);
            
            var handle = SteamUserStats.FindLeaderboard(leaderboardName);
            findLeaderboard.Set(handle);
        }
        
        void OnLeaderboardFound(LeaderboardFindResult_t result, bool failure)
        {
            if (!failure)
            {
                leaderboard = result.m_hSteamLeaderboard;
                leaderboardReady = true;
            }
        }

        // -----------------------------
        // Upload Score
        // -----------------------------

        public void UploadScore(int score, int[] details = null)
        {
            if (!leaderboardReady) return;
            if (!SteamManager.Initialized) return;

            var handle = SteamUserStats.UploadLeaderboardScore(
                leaderboard,
                ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodKeepBest,
                score,
                details,
                details?.Length ?? 0
            );

            uploadResult.Set(handle);
        }

        void OnScoreUploaded(LeaderboardScoreUploaded_t result, bool failure)
        {
            // Optional callback
        }
        
        // -----------------------------
        // Download Top
        // -----------------------------

        public void DownloadTop(int count)
        {
            if (!leaderboardReady) return;
            if (!SteamManager.Initialized) return;

            var handle = SteamUserStats.DownloadLeaderboardEntries(
                leaderboard,
                ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobal,
                1,
                count
            );

            downloadTopResult.Set(handle);
        }

        // -----------------------------
        // Download Around Player
        // -----------------------------

        public void DownloadAroundPlayer(int range)
        {
            if (!leaderboardReady) return;
            if (!SteamManager.Initialized) return;

            var handle = SteamUserStats.DownloadLeaderboardEntries(
                leaderboard,
                ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobalAroundUser,
                -range,
                range
            );

            downloadAroundPlayerResult.Set(handle);
        }

        void OnTopScoresDownloadedInternal(LeaderboardScoresDownloaded_t result, bool failure)
        {
            var entries = ConvertScoresDownloadedInternal(result, failure);
            OnTopScoresDownloaded?.Invoke(entries);
        }

        void OnPlayerScoresDownloadedInternal(LeaderboardScoresDownloaded_t result, bool failure)
        {
            var entries = ConvertScoresDownloadedInternal(result, failure);
            OnPlayerScoresDownloaded?.Invoke(entries);
        }
        
        List<LeaderboardEntryData> ConvertScoresDownloadedInternal(LeaderboardScoresDownloaded_t result, bool failure)
        {
            if (failure) return new List<LeaderboardEntryData>();

            List<LeaderboardEntryData> entries = new List<LeaderboardEntryData>();

            for (int i = 0; i < result.m_cEntryCount; i++)
            {
                var details = new int[1];

                SteamUserStats.GetDownloadedLeaderboardEntry(
                    result.m_hSteamLeaderboardEntries,
                    i,
                    out var entry,
                    details,
                    0
                );
                
                entries.Add(new LeaderboardEntryData
                {
                    steamID = entry.m_steamIDUser,
                    rank = entry.m_nGlobalRank,
                    score = entry.m_nScore,
                    playerName = SteamFriends.GetFriendPersonaName(entry.m_steamIDUser),
                    classType = (CharacterClass)details[0]
                });
            }

            return entries;
        }
        
        // -----------------------------
        // OFFLINE SYSTEM
        // -----------------------------

        void AddOfflineScore(int score)
        {
            offlineLeaderboard.Add(new LeaderboardEntryData
            {
                playerName = "Player",
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

            int playerIndex = 0; // assume player is entry 0 for offline

            int start = Mathf.Max(0, playerIndex - range);
            int end = Mathf.Min(offlineLeaderboard.Count - 1, playerIndex + range);

            return offlineLeaderboard.GetRange(start, end - start + 1);
        }
    }
}