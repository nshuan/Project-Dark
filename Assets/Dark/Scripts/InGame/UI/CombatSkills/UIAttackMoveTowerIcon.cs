using System;
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

        [Space] [Header("Multiple skill")] 
        [SerializeField] private GameObject secondSkill;
        [SerializeField] private Transform groupPassiveAndArrow;
        [SerializeField] private float groupPassiveOneSkillX = 0f;
        [SerializeField] private float groupPassiveTwoSkillX = 0f;

        [Space] [Header("Skill icon")] 
        [SerializeField] private Image imgIconBaseSkill1;
        [SerializeField] private Image imgIconBaseSkill2;
        [SerializeField] private Sprite[] iconSkills;
        
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

            if (bonusInfo.unlockedMoveToTower is { Count: 2 })
            {
                SetSkillSprite(imgIconBaseSkill1, bonusInfo.unlockedMoveToTower[0]);
                SetSkillSprite(imgIconBaseSkill2, bonusInfo.unlockedMoveToTower[1]);
                SetSkillSprite(imgFillCooldown, bonusInfo.unlockedMoveToTower[0]);
                secondSkill.SetActive(true);
                groupPassiveAndArrow.localPosition = new Vector3(groupPassiveTwoSkillX, groupPassiveAndArrow.localPosition.y, groupPassiveAndArrow.localPosition.z);
            }
            else if (bonusInfo.unlockedMoveToTower is { Count: 1 })
            {
                SetSkillSprite(imgIconBaseSkill1, bonusInfo.unlockedMoveToTower[0]);
                SetSkillSprite(imgFillCooldown, bonusInfo.unlockedMoveToTower[0]);
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
            
            available = true;
            callbackShowSkill?.Invoke();
            CombatActions.OnMoveTower -= OnSkillUsed;
            CombatActions.OnMoveTower += OnSkillUsed;
        }
        
        protected override void ShowToast()
        {
            // Nếu mới unlock 1 loại thì dùng tên loại đó
            // Nếu đã unlock cả 2 loại thì dùng tên loại unlock trước
            var message = "";
            if (LevelUtility.BonusInfo.unlockedMoveToTower == null || LevelUtility.BonusInfo.unlockedMoveToTower.Count == 0)
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

        private void SetSkillSprite(Image skillImage, int skillId)
        {
            if (skillId is 1 or 2) skillImage.sprite = iconSkills[skillId];
            skillImage.sprite = iconSkills[0];
        }
    }
}