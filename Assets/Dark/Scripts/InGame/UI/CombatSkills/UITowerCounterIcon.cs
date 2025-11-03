using System;
using System.Linq;
using Data;
using InGame.UI.InGameToast;
using InGame.Upgrade;
using UnityEngine;
using UnityEngine.UI;

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
            CombatActions.OnTowerCounter -= OnTowerCounter;
        }

        private void OnUpgradeBonusActivated(UpgradeBonusInfo bonusInfo)
        {
            UpgradeManager.Instance.OnActivated -= OnUpgradeBonusActivated;

            if (bonusInfo.unlockedTowerCounter == null)
            {
                available = false;
                callbackHideSkill?.Invoke();
                return;
            }
            
            var showIcon = false;
            var unlockedPierce =
                bonusInfo.unlockedTowerCounter.TryGetValue(NodeTowerCounter.CounterType.Pierce,
                    out var unlocked) && unlocked;
            var unlockedSlash =
                bonusInfo.unlockedTowerCounter.TryGetValue(NodeTowerCounter.CounterType.Slash,
                    out unlocked) && unlocked;
            if (unlockedPierce && unlockedSlash)
            {
                showIcon = true;
                SetSkillSprite(imgIconBaseSkill1, 1);
                SetSkillSprite(imgIconBaseSkill2, 2);
                SetSkillSprite(imgFillCooldown, 1);
                SetSkillSprite(imgFillCooldown2nd, 2);
                secondSkill.SetActive(true);
                groupPassiveAndArrow.localPosition = new Vector3(groupPassiveTwoSkillX, groupPassiveAndArrow.localPosition.y, groupPassiveAndArrow.localPosition.z);
            }
            else if (unlockedPierce)
            {
                showIcon = true;
                SetSkillSprite(imgIconBaseSkill1, 1);
                SetSkillSprite(imgFillCooldown, 1);
                secondSkill.SetActive(false);
                groupPassiveAndArrow.localPosition = new Vector3(groupPassiveOneSkillX, groupPassiveAndArrow.localPosition.y,
                    groupPassiveAndArrow.localPosition.z);
            }
            else if (unlockedSlash)
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
                CombatActions.OnTowerCounter -= OnTowerCounter;
                CombatActions.OnTowerCounter += OnTowerCounter;
            }
            else
            {
                available = false;
                callbackHideSkill?.Invoke();
            }
        }

        private void OnTowerCounter(NodeTowerCounter.CounterType counterType, float cooldown)
        {
            switch (counterType)
            {
                case NodeTowerCounter.CounterType.Pierce:
                    OnSkillUsed(cooldown);
                    break;
                case NodeTowerCounter.CounterType.Slash:
                    On2ndSkillUsed(cooldown);
                    break;
            }
        }
        
        protected override void ShowToast()
        {
            if (LevelUtility.BonusInfo.unlockedTowerCounter == null) return;
            var unlockedPierce =
                LevelUtility.BonusInfo.unlockedTowerCounter.TryGetValue(NodeTowerCounter.CounterType.Pierce,
                    out var unlocked) && unlocked;
            var unlockedSlash =
                LevelUtility.BonusInfo.unlockedTowerCounter.TryGetValue(NodeTowerCounter.CounterType.Slash,
                    out unlocked) && unlocked;
            
            // Nếu mới unlock 1 loại thì dùng tên loại đó
            // Nếu đã unlock cả 2 loại thì dùng tên cả 2 loại
            var message = "";
            if (unlockedPierce && unlockedSlash)
            {
                message = "Vowpierce and Trine Severance are ready!";
            }
            else if (unlockedPierce)
            {
                message = "Vowpierce is ready!";
            }
            else if (unlockedSlash)
            {
                message = "Trine Severance is ready!";
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