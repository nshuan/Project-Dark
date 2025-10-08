using System;
using InGame.Upgrade;
using UnityEngine;

namespace InGame.UI.CombatSkills
{
    public class UITowerCounterIcon : UIInGameSkillIcon
    {
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
            CombatActions.OnTowerCounter -= OnSkillUsed;
        }

        private void OnUpgradeBonusActivated(UpgradeBonusInfo bonusInfo)
        {
            UpgradeManager.Instance.OnActivated -= OnUpgradeBonusActivated;
            
            if (bonusInfo.unlockedTowerCounter)
            {
                available = true;
                callbackShowSkill?.Invoke();
                CombatActions.OnTowerCounter -= OnSkillUsed;
                CombatActions.OnTowerCounter += OnSkillUsed;
            }
            else
            {
                available = false;
                callbackHideSkill?.Invoke();
            }
        }
    }
}