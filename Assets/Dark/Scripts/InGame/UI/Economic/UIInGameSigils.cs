using System;
using Economic;
using TMPro;
using UnityEngine;

namespace InGame.UI.Economic
{
    public class UIInGameSigils : MonoBehaviour
    {
        public TextMeshProUGUI txtBossPoint;
        
        private int bossPoint;
        
        private void Start()
        {
            bossPoint = WealthManager.Instance.BossPoint;
            UpdateUI();

            WealthManager.Instance.OnBossPointChanged += OnBossPointChanged;
        }

        private void OnDestroy()
        {
            WealthManager.Instance.OnBossPointChanged -= OnBossPointChanged;
        }

        private void UpdateUI()
        {
            txtBossPoint.SetText($"{bossPoint}");
        }

        private void OnBossPointChanged(int before, int after)
        {
            bossPoint = after;
            UpdateUI();
        }
    }
}