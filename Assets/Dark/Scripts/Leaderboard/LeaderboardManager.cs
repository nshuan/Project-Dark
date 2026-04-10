using System;
using System.Collections.Generic;
using Core;
using InGame.CharacterClass;
using Sirenix.Serialization;
using UnityEngine;

namespace Dark.Scripts.Leaderboard
{
    public class LeaderboardManager : SerializedMonoSingleton<LeaderboardManager>
    {
        [NonSerialized, OdinSerialize]
        private Dictionary<CharacterClass, GameCompletionLeaderboardManager> leaderboards;

        [SerializeField] private GameCompletionLeaderboardManager fullLeaderboard;
        [SerializeField] private GameCompletionLeaderboardManager endlessLeaderboard;
        
        public void Initialize()
        {
            if (leaderboards != null)
            {
                foreach (var leaderboard in leaderboards)
                {
                    leaderboard.Value.Initialize();
                }
            }

            fullLeaderboard?.Initialize();
            endlessLeaderboard?.Initialize();
        }

        public GameCompletionLeaderboardManager GetLeaderboard(CharacterClass characterClass)
        {
            if (leaderboards == null) return null;
            if (leaderboards.TryGetValue(characterClass, out var leaderboard))
                return leaderboard;
            return null;
        }

        public GameCompletionLeaderboardManager GetFullLeaderboard()
        {
            return fullLeaderboard;
        }

        public GameCompletionLeaderboardManager GetEndlessLeaderboard()
        {
            return endlessLeaderboard;
        }
    }
}