using TMPro;

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
    }
}