using TMPro;

namespace Economic.UI
{
    public class UIEchoesInGame : UIEconomic
    {
        public TextMeshProUGUI txtEchoes;
        
        private void Start()
        {
            current = 0;
            target = 0;
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
            AnimateUpdating(target + after - before);
        }
        
        public override void UpdateUI()
        {
            txtEchoes.SetText($"{current}");
        }
    }
}