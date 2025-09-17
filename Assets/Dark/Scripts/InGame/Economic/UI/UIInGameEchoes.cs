using Economic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace InGame.UI.Economic
{
    public class UIInGameEchoes : UIInGameEconomic
    {
        public TextMeshProUGUI txtEchoes;
        
        private void Start()
        {
            current = WealthManager.Instance.LevelPoint;
            target = WealthManager.Instance.LevelPoint;
            UpdateUI();
            
            WealthManager.Instance.OnLevelPointChanged += OnLevelPointChanged;
        }
        
        private void OnDestroy()
        {
            WealthManager.Instance.OnLevelPointChanged -= OnLevelPointChanged;
        }
        
        private void OnLevelPointChanged(int before, int after)
        {
            if (before == after) return;
            UpdateUI();    
        }
        
        private void UpdateUI()
        {
            txtEchoes.SetText($"{current}");
        }
    }
}