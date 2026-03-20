using Data;
using InGame.Upgrade;
using TMPro;
using UnityEngine.EventSystems;

namespace Economic.UI
{
    public class UIResetPoint : UIEconomic
    {
        public TextMeshProUGUI txtResetPoint;
        
        private void Start()
        {
            current = PlayerDataManager.Instance.Data.resetPoint;
            target = PlayerDataManager.Instance.Data.resetPoint;
            UpdateUI();
            
            UpgradeManager.Instance.OnResetPointChanged += OnResetPointChanged;
        }
        
        private void OnDestroy()
        {
            UpgradeManager.Instance.OnResetPointChanged -= OnResetPointChanged;
        }

        private void OnResetPointChanged(int before, int after)
        {
            if (before == after) return;
            AnimateUpdating(after);
        }
        
        public override void UpdateUI()
        {
            txtResetPoint.SetText($"{current}");
        }
        
        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (showInstruction)
            {
                panelInstruction.SetActive(true);
            }
        }
    }
}