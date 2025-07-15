using Economic;
using TMPro;
using UnityEngine;

namespace InGame.UI.Economic
{
    public class UIInGameEchoes : MonoBehaviour
    {
        public TextMeshProUGUI txtLevelPoint;
        private int levelPoint;
        
        private void Start()
        {
            levelPoint = WealthManager.Instance.LevelPoint;
            UpdateUI();
            
            WealthManager.Instance.OnLevelPointChanged += OnLevelPointChanged;
        }
        
        private void UpdateUI()
        {
            txtLevelPoint.SetText($"{levelPoint}");
        }

        private void OnLevelPointChanged(int before, int after)
        {
            levelPoint = after;
            UpdateUI();    
        }
    }
}