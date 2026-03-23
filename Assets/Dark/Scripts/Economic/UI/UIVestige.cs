using TMPro;
using UnityEngine.EventSystems;

namespace Economic.UI
{
    public class UIVestige : UIEconomic
    {
        public TextMeshProUGUI txtVestige;
        
        private void Start()
        {
            current = WealthManager.Instance.Vestige;
            target = WealthManager.Instance.Vestige;
            UpdateUI();
            
            WealthManager.Instance.OnVestigeChanged += OnDarkChanged;
        }
        
        private void OnDestroy()
        {
            WealthManager.Instance.OnVestigeChanged -= OnDarkChanged;
        }

        private void OnDarkChanged(int before, int after)
        {
            if (before == after) return;
            AnimateUpdating(after);
        }
        
        public override void UpdateUI()
        {
            txtVestige.SetText($"{current}");
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (showInstruction && !WealthManager.Instance.hasShowInstructionVestige)
            {
                panelInstruction.SetActive(true);
                WealthManager.Instance.SetShownInstruction(WealthType.Vestige);
            }
            else if (showInstruction)
            {
                txtHintInstruction.SetActive(true);
            }
            OnEconomicIconHoverIn?.Invoke(WealthType.Vestige);
        }
    }
}