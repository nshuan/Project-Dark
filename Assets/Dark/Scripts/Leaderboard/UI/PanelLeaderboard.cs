using System;
using InGame.CharacterClass;
using UnityEngine;

namespace Dark.Scripts.Leaderboard.UI
{
    public class PanelLeaderboard : MonoBehaviour
    {
        private void OnEnable()
        {
            LeaderboardManager.Instance.GetLeaderboard(CharacterClass.Archer).DownloadTop(10);
        }
    }
}