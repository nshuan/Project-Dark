using System.Collections.Generic;
using Dark.Tools.Language.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dark.Scripts.Leaderboard.UI
{
    public sealed class LeaderboardItemView : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI rankText;
        [SerializeField] TextMeshProUGUI nameText;
        [SerializeField] TextMeshProUGUI scoreText;
        [SerializeField] private List<GameObject> classIcons;
        [SerializeField] private Image imgHighlightCurrentPlayer;
        
        public bool IsEndlessLeaderboard { get; set; }

        public void SetData(LeaderboardEntryData data)
        {
            if (data == null)
            {
                rankText.SetText("##");
                nameText.SetText("");
                scoreText.SetText("");
                for (var i = 0; i < classIcons.Count; i++)
                {
                    classIcons[i].SetActive(false);
                }
                return;    
            }
            
            if (rankText != null) rankText.SetText((data.rank).ToString());
            if (nameText != null) nameText.SetText(data.playerName);
            if (scoreText != null)
            {
                if (IsEndlessLeaderboard)
                {
                    scoreText.SetTextLanguage("key_wave", ("%{value}", data.score.ToString()));
                }
                else
                {
                    var time = System.TimeSpan.FromMilliseconds(data.score);

                    if (time.TotalHours >= 1)
                    {
                        scoreText.SetText(string.Format("{0:D2}:{1:D2}:{2:D2}", (int)time.TotalHours, time.Minutes, time.Seconds));
                    }
                    else
                    {
                        scoreText.SetText(string.Format("{0:D2}:{1:D2}", (int)time.TotalMinutes, time.Seconds));
                    }
                }
            }
            if (classIcons != null)
            {
                for (var i = 0; i < classIcons.Count; i++)
                {
                    classIcons[i].SetActive(i == (int)data.classType);
                }
            }
            
            imgHighlightCurrentPlayer.gameObject.SetActive(false);
        }
    }
}
