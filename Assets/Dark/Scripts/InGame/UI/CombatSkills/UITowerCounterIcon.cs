using InGame.Upgrade;
using UnityEngine;

namespace InGame.UI.CombatSkills
{
    public class UITowerCounterIcon : UIInGameSkillIcon
    {
        [SerializeField] private GameObject skillIcon;
        [SerializeField] private GameObject effectIconParent;

        private bool available;
        
        protected override void Awake()
        {
            base.Awake();
            
            UpgradeManager.Instance.OnActivated += OnUpgradeBonusActivated;
        }

        private void OnUpgradeBonusActivated(UpgradeBonusInfo bonusInfo)
        {
            if (bonusInfo.unlockedTowerCounter != null)
            {
                available = true;
                skillIcon.SetActive(true);
                effectIconParent.SetActive(true);
                CombatActions.OnTowerCounter -= OnSkillUsed;
                CombatActions.OnTowerCounter += OnSkillUsed;
            }
            else
            {
                available = false;
                skillIcon.SetActive(false);
                effectIconParent.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            if (!available) return;
            CombatActions.OnTowerCounter -= OnSkillUsed;
        }
    }
}