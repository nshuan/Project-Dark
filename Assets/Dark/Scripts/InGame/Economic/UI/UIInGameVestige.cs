using System;
using System.Collections;
using Economic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace InGame.UI.Economic
{
    public class UIInGameVestige : UIInGameEconomic
    {
        public TextMeshProUGUI txtVestige;
        
        private void Start()
        {
            current = WealthManager.Instance.Dark;
            target = WealthManager.Instance.Dark;
            UpdateUI();
            
            WealthManager.Instance.OnDarkChanged += OnDarkChanged;
        }
        
        private void OnDestroy()
        {
            WealthManager.Instance.OnDarkChanged -= OnDarkChanged;
        }

        private void OnDarkChanged(int before, int after)
        {
            if (before == after) return;
            AnimateIncreasing(after);
        }
        
        public override void UpdateUI()
        {
            txtVestige.SetText($"{current}");
        }
    }
}