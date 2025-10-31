using System;
using InGame.UI.InGameToast;
using InGame.Upgrade;
using UnityEngine;
using UnityEngine.UI;

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

            var showIcon = false;
            
            if (bonusInfo.skillBonus.unlockedChargeSize && bonusInfo.skillBonus.unlockedChargeBullet)
            {
                showIcon = true;
                SetSkillSprite(imgIconBaseSkill1, 1);
                SetSkillSprite(imgIconBaseSkill2, 2);
                SetSkillSprite(imgFillCooldown, 1);
                SetSkillSprite(imgFillCooldown, 2);
                secondSkill.SetActive(true);
                groupPassiveAndArrow.localPosition = new Vector3(groupPassiveTwoSkillX, groupPassiveAndArrow.localPosition.y, groupPassiveAndArrow.localPosition.z);
            }
            else if (bonusInfo.skillBonus.unlockedChargeSize)
            {
                showIcon = true;
                SetSkillSprite(imgIconBaseSkill1, 1);
                SetSkillSprite(imgFillCooldown, 1);
                secondSkill.SetActive(false);
                groupPassiveAndArrow.localPosition = new Vector3(groupPassiveOneSkillX, groupPassiveAndArrow.localPosition.y,
                    groupPassiveAndArrow.localPosition.z);
            }
            else if (bonusInfo.skillBonus.unlockedChargeBullet)
            {
                showIcon = true;
                SetSkillSprite(imgIconBaseSkill1, 2);
                SetSkillSprite(imgFillCooldown, 2);
                secondSkill.SetActive(false);
                groupPassiveAndArrow.localPosition = new Vector3(groupPassiveOneSkillX, groupPassiveAndArrow.localPosition.y,
                    groupPassiveAndArrow.localPosition.z);
            }
            else
            {
                showIcon = false;
                SetSkillSprite(imgIconBaseSkill1, 0);
                SetSkillSprite(imgFillCooldown, 0);
                secondSkill.SetActive(false);
                groupPassiveAndArrow.localPosition = new Vector3(groupPassiveOneSkillX, groupPassiveAndArrow.localPosition.y,
                    groupPassiveAndArrow.localPosition.z);
            }
            
            if (showIcon)
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
            // Nếu mới unlock 1 loại thì dùng tên loại đó
            // Nếu đã unlock cả 2 loại thì dùng tên cả 2 loại
            var message = "";
            if (LevelUtility.BonusInfo.skillBonus.unlockedChargeSize && LevelUtility.BonusInfo.skillBonus.unlockedChargeBullet)
            {
                message = "Splitting Echo and Wanderfang are ready!";
            }
            else if (LevelUtility.BonusInfo.skillBonus.unlockedChargeSize)
            {
                message = "Splitting Echo is ready!";
            }
            else if (LevelUtility.BonusInfo.skillBonus.unlockedChargeBullet)
            {
                message = "Wanderfang is ready!";
            }
            
            ToastInGameManager.Instance.Register(
                message: message,
                icon: toastIcon);
        }
        
        private void SetSkillSprite(Image skillImage, int skillId)
        {
            if (skillId is 1 or 2) skillImage.sprite = iconSkills[skillId];
            skillImage.sprite = iconSkills[0];
        }
    }
}