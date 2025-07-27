using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace InGame.UI.CombatSkills
{
    public class UISkillPassiveIcon : MonoBehaviour
    {
        [SerializeField] private Image imgFillCooldown;
        [SerializeField] private PassiveTriggerType triggerType;
        [FormerlySerializedAs("effectType")] public PassiveType passiveType;

        private void Start()
        {
            CombatActions.OnEffectTriggered += OnEffectTriggered;
        }

        private void OnDestroy()
        {
            CombatActions.OnEffectTriggered -= OnEffectTriggered;
        }

        private void Awake()
        {
            imgFillCooldown.fillAmount = 1f;
        }

        private void OnEffectTriggered(PassiveTriggerType triggerType, PassiveType passiveType, float cooldown)
        {
            if (triggerType == this.triggerType && passiveType == this.passiveType)
            {
                StartCoroutine(IECooldown(cooldown));
            }
        }
        
        private IEnumerator IECooldown(float cooldown)
        {
            imgFillCooldown.gameObject.SetActive(true);
            
            var cooldownTimer = cooldown;
            
            while (cooldownTimer > 0)
            {
                cooldownTimer -= Time.deltaTime;
                imgFillCooldown.fillAmount = 1 - cooldownTimer / cooldown;
                yield return null;
            }
        }
    }
}