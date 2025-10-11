using System;
using Data;
using InGame.UI.InGameToast;
using InGame.Upgrade;
using UnityEngine;

namespace InGame.UI.CombatSkills
{
    public class UITowerCounterIcon : UIInGameSkillIcon
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
        
        protected override void ShowToast()
        {
            var message = "";
            switch ((CharacterClass.CharacterClass)PlayerDataManager.Instance.Data.characterClass)
            {
                case CharacterClass.CharacterClass.Archer:
                    message = "Vowpierce is ready!";
                    break;
                case CharacterClass.CharacterClass.Knight:
                    message = "Trine Severance is ready!";
                    break;
            }
            
            ToastInGameManager.Instance.Register(
                message: message,
                icon: toastIcon);
        }
    }
}