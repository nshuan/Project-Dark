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
            var fakeList = new List<LeaderboardEntryData>();
            for (var i = 0; i < 10; i++)
            {
                fakeList.Add(new LeaderboardEntryData()
                {
                    rank = i,
                    score = (i + 1) * 40,
                    playerName = $"player_{i}",
                    classType = (CharacterClass)Random.Range(0, 2)
                });
            }

            this.DelayCall(1f, () => 
                OnScoresDownloaded?.Invoke(fakeList));
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