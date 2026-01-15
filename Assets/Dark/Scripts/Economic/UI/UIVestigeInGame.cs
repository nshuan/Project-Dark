using TMPro;

namespace Economic.UI
{
    public class UIVestigeInGame : UIEconomic
    {
        public TextMeshProUGUI txtVestige;
        
        private void Start()
        {
            current = 0;
            target = 0;
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
            AnimateUpdating(target + after - before);
        }
        
        public override void UpdateUI()
        {
            txtVestige.SetText($"{current}");
        }
    }
}