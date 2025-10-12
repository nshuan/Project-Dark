using System;
using InGame.UI.InGameToast;
using InGame.Upgrade;
using UnityEngine;

namespace InGame.UI.CombatSkills
{
    public class UIAttackMoveTowerIcon : UIInGameSkillIcon
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
        
        protected override void ShowToast()
        {
            // Nếu mới unlock 1 loại thì dùng tên loại đó
            // Nếu đã unlock cả 2 loại thì dùng tên loại unlock trước
            var message = "";
            if (LevelUtility.BonusInfo.unlockedMoveToTower is { Count: 0 })
            {
                message = "Ready to move!";
            }
            else
            {
                if (LevelUtility.BonusInfo.unlockedMoveToTower[0] == 1)
                    message = "Echofall is ready!";
                else if (LevelUtility.BonusInfo.unlockedMoveToTower[0] == 2)
                    message = "Vanguard’s Line is ready";
            }
            
            ToastInGameManager.Instance.Register(
                message: message,
                icon: toastIcon);
        }
    }
}