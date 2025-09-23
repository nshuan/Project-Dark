using TMPro;

namespace Economic.UI
{
    public class UIEchoes : UIEconomic
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
            AnimateUpdating(after);
        }
        
        public override void UpdateUI()
        {
            txtEchoes.SetText($"{current}");
        }
    }
}