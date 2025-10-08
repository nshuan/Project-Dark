using System;
using InGame.Upgrade;
using UnityEngine;

namespace InGame.UI.CombatSkills
{
    public class UIAttackMoveTowerIcon : UIInGameSkillIcon
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
            CombatActions.OnMoveTower -= OnSkillUsed;
        }

        private void OnUpgradeBonusActivated(UpgradeBonusInfo bonusInfo)
        {
            UpgradeManager.Instance.OnActivated -= OnUpgradeBonusActivated;
            
            // if (bonusInfo.unlockedMoveToTower is { Count: > 0 })
            {
                available = true;
                callbackShowSkill?.Invoke();
                CombatActions.OnMoveTower -= OnSkillUsed;
                CombatActions.OnMoveTower += OnSkillUsed;
            }
            // else
            // {
            //     available = false;
            //     callbackHideSkill?.Invoke();
            // }
        }
    }
}