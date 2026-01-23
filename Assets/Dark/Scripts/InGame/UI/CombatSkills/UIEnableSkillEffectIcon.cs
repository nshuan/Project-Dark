using System;
using System.Linq;
using InGame.Upgrade;
using UnityEngine;

namespace InGame.UI.CombatSkills
{
    public class UIEnableSkillEffectIcon : MonoBehaviour
    {
        [SerializeField] private PassiveTriggerType triggerType;
        [SerializeField] private UISkillPassiveIcon[] effectIcons;

        private void Awake()
        {
            foreach (var icon in effectIcons)
            {
                icon.gameObject.SetActive(false);
            }

            UpgradeManager.Instance.OnActivated += OnUpgradeBonusActivated;
        }

        private void OnDestroy()
        {
            UpgradeManager.Instance.OnActivated -= OnUpgradeBonusActivated;
        }

        private void OnUpgradeBonusActivated(UpgradeBonusInfoV2 bonusInfo)
        {
            if (effectIcons == null || effectIcons.Length == 0) return;
                
            foreach (var icon in effectIcons)
            {
                icon.gameObject.SetActive(false);
                if (LevelUtilityV2.IsUnlockedPassive(triggerType, icon.passiveType))
                {
                    icon.gameObject.SetActive(true);
                }
            }
        }
    }
}