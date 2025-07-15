using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InGame.UI.CombatSkills
{
    public class UISkillEffectIcon : MonoBehaviour
    {
        [SerializeField] private Image imgFillCooldown;
        [SerializeField] private EffectTriggerType triggerType;
        public EffectType effectType;

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
            imgFillCooldown.gameObject.SetActive(false);
        }

        private void OnEffectTriggered(EffectTriggerType triggerType, EffectType effectType, float cooldown)
        {
            if (triggerType == this.triggerType && effectType == this.effectType)
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
                imgFillCooldown.fillAmount = cooldownTimer / cooldown;
                yield return null;
            }
            
            imgFillCooldown.gameObject.SetActive(false);
        }
    }
}