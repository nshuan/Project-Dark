using System;
using UnityEngine;

namespace Dark.Scripts.Leaderboard.UI
{
    public class PanelLeaderboard : MonoBehaviour
    {
        private void OnEnable()
        {
            GameCompletionLeaderboardManager.Instance.DownloadTop(10);
        }
    }
}