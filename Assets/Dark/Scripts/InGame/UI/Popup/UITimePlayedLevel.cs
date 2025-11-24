using System;
using TMPro;
using UnityEngine;

namespace InGame.UI
{
    public class UITimePlayedLevel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI txtTimePlayed;

        private void OnEnable()
        {
            var timePlayed = LevelManager.Instance.TimePlayed;
            txtTimePlayed.SetText(timePlayed.Hours > 0
                ? timePlayed.ToString(@"hh\:mm\:ss")
                : timePlayed.ToString(@"mm\:ss"));
        }
    }
}