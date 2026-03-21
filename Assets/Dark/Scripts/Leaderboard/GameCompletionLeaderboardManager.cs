using System;
using Core;
using System.Collections.Generic;
using UnityEngine;

namespace Dark.Scripts.Leaderboard
{
    public class GameCompletionLeaderboardManager : MonoBehaviour
    {
        public string leaderboardName;
        
        public event Action<List<LeaderboardEntryData>> OnScoresDownloaded;
        
        // -----------------------------
        // Initialize
        // -----------------------------

        public void Initialize()
        {

        }

        // -----------------------------
        // Upload Score
        // -----------------------------

        public void UploadScore(int score)
        {

        }

        // -----------------------------
        // Download Top
        // -----------------------------

        public void DownloadTop(int count)
        {

        }

        // -----------------------------
        // Download Around Player
        // -----------------------------

        public void DownloadAroundPlayer(int range)
        {

        }

        // -----------------------------
        // OFFLINE SYSTEM
        // -----------------------------

        void AddOfflineScore(int score)
        {

        }

        List<LeaderboardEntryData> GetOfflineNeighbors(int range)
        {
            return null;
        }
    }
}