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
            CombatActions.OnMoveTower -= OnSkillUsed;
            UpgradeManager.Instance.OnActivated -= OnUpgradeBonusActivated;
        }

        private void OnUpgradeBonusActivated(UpgradeBonusInfo bonusInfo)
        {
            if (bonusInfo.unlockedMoveToTower != null && bonusInfo.unlockedMoveToTower.Count > 0)
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