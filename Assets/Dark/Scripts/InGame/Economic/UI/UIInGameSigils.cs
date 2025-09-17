using System;
using Economic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace InGame.UI.Economic
{
    public class UIInGameSigils : UIInGameEconomic
    {
        public TextMeshProUGUI txtSigils;
        
        private void Start()
        {
            current = WealthManager.Instance.BossPoint;
            target = WealthManager.Instance.BossPoint;
            UpdateUI();
            
            WealthManager.Instance.OnBossPointChanged += OnBossPointChanged;
        }
        
        private void OnDestroy()
        {
            WealthManager.Instance.OnBossPointChanged -= OnBossPointChanged;
        }

        private void OnBossPointChanged(int before, int after)
        {
            if (before == after) return;
            AnimateIncreasing(after);
        }
        
        public override void UpdateUI()
        {
            txtSigils.SetText($"{current}");
        }
    }
}