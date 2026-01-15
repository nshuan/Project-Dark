using System;
using InGame.UI.InGameToast;
using InGame.Upgrade;
using UnityEngine;
using UnityEngine.UI;

namespace InGame.UI.CombatSkills
{
    public class UIAttackNormalIcon : UIInGameSkillIcon
    {
        [Space] [Header("Toast")]
        [SerializeField] private Sprite toastIcon;
        
        private void Start()
        {
            CombatActions.OnAttackNormal += OnSkillUsed;
        }

        private void OnDestroy()
        {
            UpgradeManager.Instance.OnActivated -= OnUpgradeBonusActivated;
            CombatActions.OnAttackNormal -= OnSkillUsed;
        }

        public override void CheckShowSkill(Action callbackShow, Action callbackHide)
        {
            callbackShow?.Invoke();
            UpgradeManager.Instance.OnActivated -= OnUpgradeBonusActivated;
            UpgradeManager.Instance.OnActivated += OnUpgradeBonusActivated;
        }
        
        private void OnUpgradeBonusActivated(UpgradeBonusInfoV2 bonusInfo)
        {
            if (bonusInfo.bonusUnlockSkill.unlockNormalAttackPiercing && bonusInfo.bonusUnlockSkill.unlockNormalAttackBullet)
            {
                SetSkillSprite(imgIconBaseSkill1, 1);
                SetSkillSprite(imgIconBaseSkill2, 2);
                SetSkillSprite(imgFillCooldown, 1);
                SetSkillSprite(imgFillCooldown2nd, 2);
                secondSkill.SetActive(true);
                groupPassiveAndArrow.localPosition = new Vector3(groupPassiveTwoSkillX, groupPassiveAndArrow.localPosition.y, groupPassiveAndArrow.localPosition.z);
            }
            else if (bonusInfo.bonusUnlockSkill.unlockNormalAttackPiercing)
            {
                SetSkillSprite(imgIconBaseSkill1, 1);
                SetSkillSprite(imgFillCooldown, 1);
                secondSkill.SetActive(false);
                groupPassiveAndArrow.localPosition = new Vector3(groupPassiveOneSkillX, groupPassiveAndArrow.localPosition.y,
                    groupPassiveAndArrow.localPosition.z);
            }
            else if (bonusInfo.bonusUnlockSkill.unlockNormalAttackBullet)
            {
                SetSkillSprite(imgIconBaseSkill1, 2);
                SetSkillSprite(imgFillCooldown, 2);
                secondSkill.SetActive(false);
                groupPassiveAndArrow.localPosition = new Vector3(groupPassiveOneSkillX, groupPassiveAndArrow.localPosition.y,
                    groupPassiveAndArrow.localPosition.z);
            }
            else
            {
                SetSkillSprite(imgIconBaseSkill1, 0);
                SetSkillSprite(imgFillCooldown, 0);
                secondSkill.SetActive(false);
                groupPassiveAndArrow.localPosition = new Vector3(groupPassiveOneSkillX, groupPassiveAndArrow.localPosition.y,
                    groupPassiveAndArrow.localPosition.z);
            }
        }

        protected override void ShowToast()
        {
            return;
            ToastInGameManager.Instance.Register(
                message: "Attack is ready!",
                icon: toastIcon);
        }
        
        private void SetSkillSprite(Image skillImage, int skillId)
        {
            if (skillId is 1 or 2) skillImage.sprite = iconSkills[skillId];
            else skillImage.sprite = iconSkills[0];
        }
    }
}