using Coffee.UIExtensions;
using Economic.InGame.DropItems;
using TMPro;

namespace Economic.UI
{
    public class UIVestigeInGame : UIEconomic
    {
        public TextMeshProUGUI txtVestige;
        public UIParticle fxClaim;
        
        private void Start()
        {
            current = 0;
            target = 0;
            UpdateUI();
            
            EItemDropManager.Instance.CollectedData.onVestigeChanged += OnDarkChanged;
        }
        
        private void OnDestroy()
        {
            EItemDropManager.Instance.CollectedData.onVestigeChanged -= OnDarkChanged;
        }

        private void OnDarkChanged(int before, int after)
        {
            if (before == after) return;
            AnimateUpdating(target + after - before);
            fxClaim.Play();
        }
        
        public override void UpdateUI()
        {
            txtVestige.SetText($"{current}");
        }
    }
}