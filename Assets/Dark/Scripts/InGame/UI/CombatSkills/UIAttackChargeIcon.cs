using System;
using Dark.Tools.Language.Runtime;
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
            UpgradeManager.Instance.OnActivated -= OnUpgradeBonusActivated;
            UpgradeManager.Instance.OnActivated += OnUpgradeBonusActivated;
        }

        private void OnDestroy()
        {
            CombatActions.OnAttackCharge -= OnChargeUsed;
            UpgradeManager.Instance.OnActivated -= OnUpgradeBonusActivated;
        }
        
        private void OnUpgradeBonusActivated(UpgradeBonusInfoV2 bonusInfo)
        {
            var showIcon = false;
            
            if (bonusInfo.bonusUnlockSkill.unlockChargeAttackSize && bonusInfo.bonusUnlockSkill.unlockChargeAttackBullet)
            {
                showIcon = true;
                SetSkillSprite(imgIconBaseSkill1, 1);
                SetSkillSprite(imgIconBaseSkill2, 2);
                SetSkillSprite(imgFillCooldown, 1);
                SetSkillSprite(imgFillCooldown, 2);
                secondSkill.SetActive(true);
                groupPassiveAndArrow.anchoredPosition =
                    new Vector2(groupPassiveTwoSkillX, groupPassiveAndArrow.anchoredPosition.y);
            }
            else if (bonusInfo.bonusUnlockSkill.unlockChargeAttackSize)
            {
                showIcon = true;
                SetSkillSprite(imgIconBaseSkill1, 1);
                SetSkillSprite(imgFillCooldown, 1);
                secondSkill.SetActive(false);
                groupPassiveAndArrow.anchoredPosition = new Vector2(groupPassiveOneSkillX, groupPassiveAndArrow.anchoredPosition.y);
            }
            else if (bonusInfo.bonusUnlockSkill.unlockChargeAttackBullet)
            {
                showIcon = true;
                SetSkillSprite(imgIconBaseSkill1, 2);
                SetSkillSprite(imgFillCooldown, 2);
                secondSkill.SetActive(false);
                groupPassiveAndArrow.anchoredPosition = new Vector2(groupPassiveOneSkillX, groupPassiveAndArrow.anchoredPosition.y);
            }
            else
            {
                showIcon = false;
                SetSkillSprite(imgIconBaseSkill1, 0);
                SetSkillSprite(imgFillCooldown, 0);
                secondSkill.SetActive(false);
                groupPassiveAndArrow.anchoredPosition = new Vector2(groupPassiveOneSkillX, groupPassiveAndArrow.anchoredPosition.y);
            }
            
            if (showIcon)
            {
                available = true;
                callbackShowSkill?.Invoke();
                CombatActions.OnAttackCharge -= OnChargeUsed;
                CombatActions.OnAttackCharge += OnChargeUsed;
            }
            else
            {
                available = false;
                callbackHideSkill?.Invoke();
            }
        }
        
        private void OnChargeUsed(float cooldown)
        {
            OnSkillUsed(cooldown, string.Empty);
        }
        
        protected override void ShowToast(string text)
        {
            // Nếu mới unlock 1 loại thì dùng tên loại đó
            // Nếu đã unlock cả 2 loại thì dùng tên cả 2 loại
            var message = "";
            if (LevelUtilityV2.BonusInfo.bonusUnlockSkill.unlockChargeAttackSize && LevelUtilityV2.BonusInfo.bonusUnlockSkill.unlockChargeAttackBullet)
            {
                message = LanguageData.Instance.GetLocalizedString("key_notify_charge_both",
                    LanguageManager.Instance.CurrentLanguage);
            }
            else if (LevelUtilityV2.BonusInfo.bonusUnlockSkill.unlockChargeAttackSize)
            {
                message = LanguageData.Instance.GetLocalizedString("key_notify_charge_size",
                    LanguageManager.Instance.CurrentLanguage);
            }
            else if (LevelUtilityV2.BonusInfo.bonusUnlockSkill.unlockChargeAttackBullet)
            {
                message = LanguageData.Instance.GetLocalizedString("key_notify_charge_bullet",
                    LanguageManager.Instance.CurrentLanguage);
            }
            
            ToastInGameManager.Instance.Register(
                message: message,
                icon: toastIcon);
        }
        
        private void SetSkillSprite(Image skillImage, int skillId)
        {
            if (skillId is 1 or 2) skillImage.sprite = iconSkills[skillId];
            else skillImage.sprite = iconSkills[0];
        }
    }
}