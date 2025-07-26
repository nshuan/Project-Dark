using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InGame.UI.CombatSkills
{
    public abstract class UIInGameSkillIcon : MonoBehaviour
    {
        [SerializeField] private Image imgFillCooldown;
        [SerializeField] private TextMeshProUGUI txtCooldown;

        protected virtual void Awake()
        {
            imgFillCooldown.fillAmount = 0f;
            imgFillCooldown.gameObject.SetActive(false);
            txtCooldown.gameObject.SetActive(false);
        }

        protected virtual void OnSkillUsed(float cooldown)
        {
            StartCoroutine(IECooldown(cooldown));
        }
        
        private IEnumerator IECooldown(float cooldown)
        {
            imgFillCooldown.gameObject.SetActive(true);
            txtCooldown.gameObject.SetActive(true);
            
            var cooldownTimer = cooldown;
            
            while (cooldownTimer > 0)
            {
                cooldownTimer -= Time.deltaTime;
                txtCooldown.SetText(((int)cooldownTimer).ToString());
                imgFillCooldown.fillAmount = 1 - cooldownTimer / cooldown;
                yield return null;
            }
            
            imgFillCooldown.gameObject.SetActive(false);
            txtCooldown.gameObject.SetActive(false);
        }
    }
}