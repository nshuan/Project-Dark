using System;
using InGame.UI.InGameToast;
using InGame.Upgrade;
using UnityEngine;

namespace InGame.UI.CombatSkills
{
    public class UIAttackChargeIcon : UIInGameSkillIcon
    {
        [Space] [Header("Toast")]
        [SerializeField] private Sprite toastIcon;
        
        private bool available;
        private Action callbackShowSkill;
        private Action callbackHideSkill;

        public override void CheckShowSkill(Action callbackShow, Action callbackHide)
        {
            callbackShowSkill = callbackShow;
            callbackHideSkill = callbackHide;
            UpgradeManager.Instance.OnActivated += OnUpgradeBonusActivated;
        }

        private void OnDestroy()
        {
            CombatActions.OnAttackCharge -= OnSkillUsed;
        }

        private void OnUpgradeBonusActivated(UpgradeBonusInfo bonusInfo)
        {
            UpgradeManager.Instance.OnActivated -= OnUpgradeBonusActivated;
            
            if (bonusInfo.skillBonus.unlockedChargeDame ||
                bonusInfo.skillBonus.unlockedChargeBullet ||
                bonusInfo.skillBonus.unlockedChargeSize ||
                bonusInfo.skillBonus.unlockedChargeRange)
            {
                available = true;
                callbackShowSkill?.Invoke();
                CombatActions.OnAttackCharge -= OnSkillUsed;
                CombatActions.OnAttackCharge += OnSkillUsed;
            }
            else
            {
                available = false;
                callbackHideSkill?.Invoke();
            }
        }
        
        protected override void ShowToast()
        {
            ToastInGameManager.Instance.Register(
                message: "Charge is ready!",
                icon: toastIcon);
        }
    }
}