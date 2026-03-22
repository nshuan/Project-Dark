using System.Collections.Generic;
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
        [SerializeField] private List<GameObject> classIcons;

        public void SetData(LeaderboardEntryData data)
        {
            if (rankText != null) rankText.text = (data.rank + 1).ToString();
            if (nameText != null) nameText.text = data.playerName ?? string.Empty;
            if (scoreText != null) scoreText.text = data.score.ToString();
            if (classIcons != null)
            {
                for (var i = 0; i < classIcons.Count; i++)
                {
                    classIcons[i].SetActive(i == (int)data.classType);
                }
            }
        }
    }
}
