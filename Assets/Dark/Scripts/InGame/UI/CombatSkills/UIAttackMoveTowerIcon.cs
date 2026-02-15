using System;
using Dark.Tools.Language.Runtime;
using InGame.UI.InGameToast;
using InGame.Upgrade;
using UnityEngine;
using UnityEngine.UI;

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
            UpgradeManager.Instance.OnActivated -= OnUpgradeBonusActivated;
            UpgradeManager.Instance.OnActivated += OnUpgradeBonusActivated;
        }

        private void OnDestroy()
        {
            UpgradeManager.Instance.OnActivated -= OnUpgradeBonusActivated;
            CombatActions.OnMoveTowerComplete -= OnMoveUsed;
        }

        private void OnUpgradeBonusActivated(UpgradeBonusInfoV2 bonusInfo)
        {
            if (bonusInfo.bonusUnlockSkill.unlockMoveDash && bonusInfo.bonusUnlockSkill.unlockMoveFlash)
            {
                SetSkillSprite(imgIconBaseSkill1, 2);
                SetSkillSprite(imgIconBaseSkill2, 1);
                SetSkillSprite(imgFillCooldown, 2);
                SetSkillSprite(imgFillCooldown2nd, 1);
                secondSkill.SetActive(true);
                groupPassiveAndArrow.anchoredPosition =
                    new Vector2(groupPassiveTwoSkillX, groupPassiveAndArrow.anchoredPosition.y);
            }
            else if (bonusInfo.bonusUnlockSkill.unlockMoveFlash)
            {
                SetSkillSprite(imgIconBaseSkill1, 1);
                SetSkillSprite(imgFillCooldown, 1);
                secondSkill.SetActive(false);
                groupPassiveAndArrow.anchoredPosition = new Vector2(groupPassiveOneSkillX, groupPassiveAndArrow.anchoredPosition.y);
            }
            else if (bonusInfo.bonusUnlockSkill.unlockMoveDash)
            {
                SetSkillSprite(imgIconBaseSkill1, 2);
                SetSkillSprite(imgFillCooldown, 2);
                secondSkill.SetActive(false);
                groupPassiveAndArrow.anchoredPosition = new Vector2(groupPassiveOneSkillX, groupPassiveAndArrow.anchoredPosition.y);
            }
            else
            {
                SetSkillSprite(imgIconBaseSkill1, 0);
                SetSkillSprite(imgFillCooldown, 0);
                secondSkill.SetActive(false);
                groupPassiveAndArrow.anchoredPosition = new Vector2(groupPassiveOneSkillX, groupPassiveAndArrow.anchoredPosition.y);
            }
            
            available = true;
            callbackShowSkill?.Invoke();
            CombatActions.OnMoveTowerComplete -= OnMoveUsed;
            CombatActions.OnMoveTowerComplete += OnMoveUsed;
        }

        private void OnMoveUsed(float cooldown)
        {
            OnSkillUsed(cooldown, string.Empty);
        }
        
        protected override void ShowToast(string text)
        {
            // Nếu mới unlock 1 loại thì dùng tên loại đó
            // Nếu đã unlock cả 2 loại thì dùng tên loại unlock trước
            var message = "";
            if (!LevelUtilityV2.BonusInfo.bonusUnlockSkill.unlockMoveFlash && !LevelUtilityV2.BonusInfo.bonusUnlockSkill.unlockMoveDash)
            {
                message = LanguageData.Instance.GetLocalizedString("key_notify_move_tele",
                    LanguageManager.Instance.CurrentLanguage);
            }
            else
            {
                if (LevelUtilityV2.BonusInfo.bonusUnlockSkill.unlockMoveFlash)
                {
                    if (LevelUtilityV2.BonusInfo.bonusUnlockSkill.unlockMoveDash)
                        message = LanguageData.Instance.GetLocalizedString("key_notify_move_both",
                            LanguageManager.Instance.CurrentLanguage);
                    else
                        message = LanguageData.Instance.GetLocalizedString("key_notify_move_flash",
                            LanguageManager.Instance.CurrentLanguage);
                }
                else if (LevelUtilityV2.BonusInfo.bonusUnlockSkill.unlockMoveDash)
                    message = LanguageData.Instance.GetLocalizedString("key_notify_move_dash",
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