using System.Linq;
using InGame.Upgrade;
using UnityEngine;

namespace InGame.UI.CombatSkills
{
    public class UIEnableSkillEffectIcon : MonoBehaviour
    {
        [SerializeField] private EffectTriggerType triggerType;
        [SerializeField] private UISkillEffectIcon[] effectIcons;

        private void Awake()
        {
            foreach (var icon in effectIcons)
            {
                icon.gameObject.SetActive(false);
            }

            UpgradeManager.Instance.OnActivated += OnUpgradeBonusActivated;
        }

        private void OnUpgradeBonusActivated(UpgradeBonusInfo bonusInfo)
        {
            if (bonusInfo.effectsMapByTriggerType.TryGetValue(triggerType, out var effectTypes))
            {
                if (effectTypes == null || effectIcons.Length == 0) return;
                
                foreach (var icon in effectIcons)
                {
                    icon.gameObject.SetActive(false);
                    if (effectTypes.Any(effectType => icon.effectType == effectType))
                    {
                        icon.gameObject.SetActive(true);
                    }
                }
            }
        }
    }
}