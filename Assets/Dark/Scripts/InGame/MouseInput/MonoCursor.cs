using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InGame
{
    public class MonoCursor : MonoBehaviour
    {
        public Image visual;
        public Transform content;
        [SerializeField] private float contentMaxScale;
        [SerializeField] private GameObject groupCooldown;
        [SerializeField] private GameObject cooldownGlow;
        [SerializeField] private Image cooldown;
        [SerializeField] private Color cooldownMaxColor;
        [SerializeField] private TextMeshProUGUI txtChargeUnitAdd;
        [SerializeField] private TextMeshProUGUI txtMax;
        [SerializeField] private TextMeshProUGUI txtReadyToCharge;
        [SerializeField] private TextMeshProUGUI txtAuto;
        
        public void UpdateCooldown(bool active, float value)
        {
            groupCooldown.SetActive(active);
            cooldown.fillAmount = value;
            cooldown.color = Color.white;
        }

        public void UpdateChargeUnitAdd(bool active, int value = 1)
        {
            txtMax.gameObject.SetActive(false);
            cooldownGlow.gameObject.SetActive(false);
            txtChargeUnitAdd.gameObject.SetActive(active);
            txtChargeUnitAdd.SetText($"+{value}");
        }

        public void UpdateMax()
        {
            txtChargeUnitAdd.gameObject.SetActive(false);
            txtMax.gameObject.SetActive(true);
            cooldownGlow.gameObject.SetActive(true);
            cooldown.color = cooldownMaxColor;
        }

        public void UpdateScale(float value)
        {
            value = Mathf.Clamp(value, 0f, 1f);
            content.transform.localScale = Vector3.one * (1 + value);
        }

        public void SetAuto(bool active)
        {
            DOTween.Kill(txtAuto);
            if (active)
            {
                txtAuto.transform.localScale = 0.3f * Vector3.one;
                txtAuto.gameObject.SetActive(true);
                txtAuto.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack).SetTarget(txtAuto);
            }
            else
            {
                txtAuto.transform.DOScale(0.3f, 0.3f).SetEase(Ease.InBack).SetTarget(txtAuto)
                    .OnComplete(() => txtAuto.gameObject.SetActive(false));
            }
        }

        public void SetReadyToCharge()
        {
            DOTween.Kill(txtReadyToCharge);
            
            txtChargeUnitAdd.SetAlpha(0f);
            txtMax.SetAlpha(0f);
            txtAuto.SetAlpha(0f);
            txtReadyToCharge.SetAlpha(1f);
            txtReadyToCharge.gameObject.SetActive(true);

            DOTween.Sequence().SetTarget(txtReadyToCharge)
                .Append(txtReadyToCharge.transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.2f))
                .AppendInterval(0.5f)
                .AppendCallback(() =>
                {
                    txtChargeUnitAdd.SetAlpha(1f);
                    txtMax.SetAlpha(1f);
                    txtAuto.SetAlpha(1f);
                    txtReadyToCharge.SetAlpha(0f);
                    txtReadyToCharge.gameObject.SetActive(false);
                });
        }
    }
}