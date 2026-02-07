using System;
using System.Linq;
using Dark.Tools.Language.Runtime;
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
            UpgradeManager.Instance.OnActivated -= OnUpgradeBonusActivated;
            UpgradeManager.Instance.OnActivated += OnUpgradeBonusActivated;
        }

        private void OnDestroy()
        {
            UpgradeManager.Instance.OnActivated -= OnUpgradeBonusActivated;
            CombatActions.OnTowerCounter -= OnTowerCounter;
        }

        private void OnUpgradeBonusActivated(UpgradeBonusInfoV2 bonusInfo)
        {
            if (bonusInfo.bonusUnlockSkill.unlockCounterPiercing == false && bonusInfo.bonusUnlockSkill.unlockCounterSlash == false)
            {
                available = false;
                callbackHideSkill?.Invoke();
                return;
            }
            
            var showIcon = false;
            var unlockedPierce =
                bonusInfo.bonusUnlockSkill.unlockCounterPiercing;
            var unlockedSlash =
                bonusInfo.bonusUnlockSkill.unlockCounterSlash;
            if (unlockedPierce && unlockedSlash)
            {
                showIcon = true;
                SetSkillSprite(imgIconBaseSkill1, 1);
                SetSkillSprite(imgIconBaseSkill2, 2);
                SetSkillSprite(imgFillCooldown, 1);
                SetSkillSprite(imgFillCooldown2nd, 2);
                secondSkill.SetActive(true);
                groupPassiveAndArrow.anchoredPosition = new Vector2(groupPassiveTwoSkillX, groupPassiveAndArrow.anchoredPosition.y);
            }
            else if (unlockedPierce)
            {
                showIcon = true;
                SetSkillSprite(imgIconBaseSkill1, 1);
                SetSkillSprite(imgFillCooldown, 1);
                secondSkill.SetActive(false);
                groupPassiveAndArrow.anchoredPosition = new Vector2(groupPassiveOneSkillX, groupPassiveAndArrow.anchoredPosition.y);
            }
            else if (unlockedSlash)
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
                    OnSkillUsed(cooldown,
                        LanguageData.Instance.GetLocalizedString("key_notify_counter_pierce",
                            LanguageManager.Instance.CurrentLanguage));
                    break;
                // Mặc định counter piercing là skill số 1, check nếu đã unlock thì counter slash là skill số 2,
                // không thì counter slash là skill số 1
                case NodeTowerCounter.CounterType.Slash:
                    var unlockedPierce = LevelUtilityV2.BonusInfo.bonusUnlockSkill.unlockCounterPiercing;
                    if (unlockedPierce)
                        On2ndSkillUsed(cooldown, LanguageData.Instance.GetLocalizedString("key_notify_counter_slash",
                            LanguageManager.Instance.CurrentLanguage));
                    else
                        OnSkillUsed(cooldown, LanguageData.Instance.GetLocalizedString("key_notify_counter_slash",
                            LanguageManager.Instance.CurrentLanguage));
                    break;
            }
        }
        
        protected override void ShowToast(string text)
        {
            if (LevelUtilityV2.BonusInfo.bonusUnlockSkill.unlockCounterPiercing == false && LevelUtilityV2.BonusInfo.bonusUnlockSkill.unlockCounterSlash == false) return;
            
            ToastInGameManager.Instance.Register(
                message: text,
                icon: toastIcon);
        }
        
        private void SetSkillSprite(Image skillImage, int skillId)
        {
            if (skillId is 1 or 2) skillImage.sprite = iconSkills[skillId];
            else skillImage.sprite = iconSkills[0];
        }
    }
}