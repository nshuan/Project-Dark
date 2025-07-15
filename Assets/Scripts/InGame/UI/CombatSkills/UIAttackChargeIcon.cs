using InGame.Upgrade;
using UnityEngine;

namespace InGame.UI.CombatSkills
{
    public class UIAttackChargeIcon : UIInGameSkillIcon
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
            if (bonusInfo.skillBonus.unlockedChargeDame ||
                bonusInfo.skillBonus.unlockedChargeBullet ||
                bonusInfo.skillBonus.unlockedChargeSize ||
                bonusInfo.skillBonus.unlockedChargeRange)
            {
                available = true;
                skillIcon.SetActive(true);
                effectIconParent.SetActive(true);
                CombatActions.OnAttackCharge -= OnSkillUsed;
                CombatActions.OnAttackCharge += OnSkillUsed;
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
            CombatActions.OnAttackCharge -= OnSkillUsed;
        }
    }
}