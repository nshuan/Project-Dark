using System;
using Economic;
using TMPro;
using UnityEngine;

namespace InGame.UI.Economic
{
    public class UIInGameVestige : MonoBehaviour
    {
        public TextMeshProUGUI txtDark;
        private int dark;
        
        private void Start()
        {
            dark = WealthManager.Instance.Dark;
            UpdateUI();
            
            WealthManager.Instance.OnDarkChanged += OnDarkChanged;
        }
        
        private void UpdateUI()
        {
            txtDark.SetText($"{dark}");
        }

        private void OnDarkChanged(int before, int after)
        {
            dark = after;
            UpdateUI();    
        }
    }
}