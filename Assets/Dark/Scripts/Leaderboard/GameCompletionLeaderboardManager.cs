using System;
using Core;
using System.Collections.Generic;
using Dark.Scripts.Utils;
using InGame.CharacterClass;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Dark.Scripts.Leaderboard
{
    public class GameCompletionLeaderboardManager : MonoBehaviour
    {
        public string leaderboardName;
        
        public event Action<List<LeaderboardEntryData>> OnTopScoresDownloaded;
        public event Action<List<LeaderboardEntryData>> OnPlayerScoresDownloaded;
        
        // -----------------------------
        // Initialize
        // -----------------------------

        public void Initialize()
        {

        }

        // -----------------------------
        // Upload Score
        // -----------------------------

        public void UploadScore(int score, int[] details = null)
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