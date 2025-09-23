using TMPro;

namespace Economic.UI
{
    public class UIVestige : UIEconomic
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