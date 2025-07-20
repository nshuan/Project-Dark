using InGame.Upgrade;
using UnityEngine;

namespace InGame.UI.CombatSkills
{
    public class UIAttackMoveTowerIcon : UIInGameSkillIcon
    {
        [SerializeField] private GameObject skillIcon;
        [SerializeField] private GameObject effectIconParent;

        private bool available;
        
        protected override void Awake()
        {
            base.Awake();
            
            UpgradeManager.Instance.OnActivated += OnUpgradeBonusActivated;
        }
        
        private void OnDestroy()
        {
            if (!available) return;
            CombatActions.OnMoveTower -= OnSkillUsed;
            UpgradeManager.Instance.OnActivated -= OnUpgradeBonusActivated;
        }

        private void OnUpgradeBonusActivated(UpgradeBonusInfo bonusInfo)
        {
            if (bonusInfo.upgradedShortMoveToTower ||
                bonusInfo.unlockedLongMoveToTower)
            {
                available = true;
                skillIcon.SetActive(true);
                effectIconParent.SetActive(true);
                CombatActions.OnMoveTower -= OnSkillUsed;
                CombatActions.OnMoveTower += OnSkillUsed;
            }
            else
            {
                available = false;
                skillIcon.SetActive(false);
                effectIconParent.SetActive(false);
            }
        }
    }
}