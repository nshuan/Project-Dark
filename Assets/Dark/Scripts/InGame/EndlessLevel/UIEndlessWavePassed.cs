using System;
using TMPro;
using UnityEngine;

namespace InGame.EndlessLevel
{
    public class UIEndlessWavePassed : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI txtPassedWave;

        private void OnEnable()
        {
            txtPassedWave.SetText(LevelEndlessManager.passedWave.ToString());
        }
    }
}