using Dark.Scripts.Leaderboard;
using TMPro;
using UnityEngine;

namespace Dark.Scripts.Leaderboard.UI
{
    public sealed class LeaderboardItemView : MonoBehaviour
    {
        [SerializeField] TMP_Text rankText;
        [SerializeField] TMP_Text nameText;
        [SerializeField] TMP_Text scoreText;

        public void SetData(LeaderboardEntryData data)
        {
            if (rankText != null) rankText.text = data.rank.ToString();
            if (nameText != null) nameText.text = data.playerName ?? string.Empty;
            if (scoreText != null) scoreText.text = data.score.ToString();
        }
    }
}
