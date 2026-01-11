using TMPro;

namespace Economic.UI
{
    public class UISigilsInGame : UIEconomic
    {
        public TextMeshProUGUI txtSigils;
        
        private void Start()
        {
            current = 0;
            target = 0;
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
            AnimateUpdating(target + after - before);
        }
        
        public override void UpdateUI()
        {
            txtSigils.SetText($"{current}");
        }
    }
}