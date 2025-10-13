using System;
using System.Collections;
using Coffee.UIExtensions;
using DG.Tweening;
using InGame.UI.InGameToast;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InGame.UI.CombatSkills
{
    public abstract class UIInGameSkillIcon : MonoBehaviour
    {
        [SerializeField] private Image imgFillCooldown;
        [SerializeField] private Button btnTogglePassive;
        [SerializeField] private Image iconTogglePassive;
        [SerializeField] private Image imgPassiveLine;
        [SerializeField] private Transform groupPassive;
        [SerializeField] private Transform groupIcon;
        [SerializeField] private UIParticle vfxCooldownComplete;

        [Space] [Header("Config")] 
        [SerializeField] private float buttonXLocalOnShow = 106f;
        [SerializeField] private float buttonXLocalOnHide = 80f;
        [SerializeField] private float passiveYLocalOnShow = 0f;
        [SerializeField] private float passiveYLocalOnHide = 32f;
        
        protected virtual void Awake()
        {
            imgFillCooldown.fillAmount = 1f;
            SetupPassive();
        }

        public abstract void CheckShowSkill(Action callbackShow, Action callbackHide);
        
        protected virtual void OnSkillUsed(float cooldown)
        {
            StartCoroutine(IECooldown(cooldown));
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

            ShowToast();
            groupIcon.DOPunchScale(0.1f * Vector3.one, 0.2f);
            vfxCooldownComplete.Play();
        }

        private bool isShowPassive = true;
        
        private void SetupPassive()
        {
            btnTogglePassive.onClick.RemoveAllListeners();
            btnTogglePassive.onClick.AddListener(() =>
            {
                DOTween.Kill(groupIcon, complete:true);
                groupIcon.DOPunchScale(-0.2f * Vector3.one, 0.2f).SetTarget(groupIcon);
                btnTogglePassive.interactable = false;
                isShowPassive = !isShowPassive;
                TogglePassive(isShowPassive);
            });
        }

        private void TogglePassive(bool show)
        {
            if (show)
            {
                DoShowPassive().OnComplete(() =>
                {
                    btnTogglePassive.interactable = true;
                });
            }
            else
            {
                DoHidePassive().OnComplete(() =>
                {
                    btnTogglePassive.interactable = true;
                });
            }
        }

        private Tween DoShowPassive()
        {
            DOTween.Kill(this);
            
            iconTogglePassive.transform.localPosition = new Vector3(buttonXLocalOnShow, iconTogglePassive.transform.localPosition.y, iconTogglePassive.transform.localPosition.z);
            imgPassiveLine.fillAmount = 0f;
            groupPassive.localPosition = new Vector3(groupPassive.localPosition.x, passiveYLocalOnHide, groupPassive.localPosition.z);

            return DOTween.Sequence(this)
                .Append(iconTogglePassive.transform.DOLocalMoveX(buttonXLocalOnHide, 0.2f))
                .Append(imgPassiveLine.DOFillAmount(1f, 0.2f))
                .Append(groupPassive.DOLocalMoveY(passiveYLocalOnShow, 0.2f));
        }

        private Tween DoHidePassive()
        {
            DOTween.Kill(this);

            iconTogglePassive.transform.localPosition = new Vector3(buttonXLocalOnHide, iconTogglePassive.transform.localPosition.y, iconTogglePassive.transform.localPosition.z);
            imgPassiveLine.fillAmount = 1f;
            groupPassive.localPosition = new Vector3(groupPassive.localPosition.x, passiveYLocalOnShow, groupPassive.localPosition.z);
            
            return DOTween.Sequence()
                .Append(groupPassive.DOLocalMoveY(passiveYLocalOnHide, 0.2f))
                .Append(imgPassiveLine.DOFillAmount(0f, 0.2f))
                .Append(iconTogglePassive.transform.DOLocalMoveX(buttonXLocalOnShow, 0.2f));
        }

        protected virtual void ShowToast()
        {
            ToastInGameManager.Instance.Register(string.Empty, null);
        }
    }
}