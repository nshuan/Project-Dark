using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InGame
{
    public class MonoCursor : MonoBehaviour
    {
        [SerializeField] private Image cooldown;
        [SerializeField] private TextMeshProUGUI txtChargeBulletAdd;

        public void UpdateCooldown(float value)
        {
            cooldown.fillAmount = value;
        }

        public void UpdateBulletAdd(bool active, int value = 1)
        {
            txtChargeBulletAdd.gameObject.SetActive(active);
            txtChargeBulletAdd.SetText($"+{value}");
        }
    }
}