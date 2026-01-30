using Coffee.UIExtensions;
using TMPro;

namespace Economic.UI
{
    public class UIEchoesInGame : UIEconomic
    {
        public TextMeshProUGUI txtEchoes;
        public UIParticle fxClaim;
        
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
            fxClaim.Play();
        }
        
        public override void UpdateUI()
        {
            txtEchoes.SetText($"{current}");     
        }
    }
}