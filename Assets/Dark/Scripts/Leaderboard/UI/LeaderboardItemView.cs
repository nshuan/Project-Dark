using System.Collections.Generic;
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
            if (rankText != null) rankText.SetText((data.rank + 1).ToString());
            if (nameText != null) nameText.SetText(data.playerName);
            if (scoreText != null)
            {
                var time = System.TimeSpan.FromMilliseconds(data.score);

                if (time.TotalHours >= 1)
                {
                    scoreText.SetText($"{time.TotalHours:D2}:{time.Minutes:D2}:{time.Seconds:D2}");
                }
                else
                {
                    scoreText.SetText($"{time.TotalMinutes:D2}:{time.Seconds:D2}");
                }
            }
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
