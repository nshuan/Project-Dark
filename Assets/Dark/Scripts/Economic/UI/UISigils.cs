using TMPro;

namespace Economic.UI
{
    public class UISigils : UIEconomic
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
            AnimateUpdating(after);
        }
        
        public override void UpdateUI()
        {
            txtSigils.SetText($"{current}");
        }
    }
}