using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InGame
{
    public class MonoCursor : MonoBehaviour
    {
        public Image visual;
        [SerializeField] private GameObject groupCooldown;
        [SerializeField] private GameObject cooldownGlow;
        [SerializeField] private Image cooldown;
        [SerializeField] private Color cooldownMaxColor;
        [SerializeField] private TextMeshProUGUI txtChargeBulletAdd;
        [SerializeField] private TextMeshProUGUI txtMax;
        
        public void UpdateCooldown(bool active, float value)
        {
            groupCooldown.SetActive(active);
            cooldown.fillAmount = value;
            cooldown.color = Color.white;
        }

        public void UpdateBulletAdd(bool active, int value = 1)
        {
            txtMax.gameObject.SetActive(false);
            cooldownGlow.gameObject.SetActive(false);
            txtChargeBulletAdd.gameObject.SetActive(active);
            txtChargeBulletAdd.SetText($"+{value}");
        }

        public void UpdateMax()
        {
            txtChargeBulletAdd.gameObject.SetActive(false);
            txtMax.gameObject.SetActive(true);
            cooldownGlow.gameObject.SetActive(true);
            cooldown.color = cooldownMaxColor;
        }
    }
}