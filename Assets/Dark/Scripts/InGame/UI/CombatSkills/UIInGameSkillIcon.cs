using System;
using System.Collections;
using Coffee.UIExtensions;
using Dark.Scripts.Common;
using DG.Tweening;
using InGame.UI.InGameToast;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InGame.UI.CombatSkills
{
    public abstract class UIInGameSkillIcon : MonoBehaviour
    {
        [SerializeField] protected Image imgFillCooldown;
        [SerializeField] private UIInteractiveHoverButton btnTogglePassive;
        [SerializeField] private Image iconTogglePassive;
        [SerializeField] private Transform groupPassive;
        [SerializeField] private CanvasGroup cvgGroupPassive;
        [SerializeField] private Transform groupIcon;
        [SerializeField] private UIParticle vfxCooldownComplete;
        [SerializeField] private Image imgBackFadeActive;
        [SerializeField] private RectTransform rectContent;
        [SerializeField] private CanvasGroup cvgSkillTitle;

        [Space] [Header("Config")] 
        [SerializeField] private float passiveXLocalOnShow = 135f;
        [SerializeField] private float passiveXLocalOnHide = 90f;
        [SerializeField] private float rectWidthOnShow = 210f;
        [SerializeField] private float rectWidthOnHide = 100f;
        
        [Space] [Header("2nd skill")] 
        [SerializeField] protected GameObject secondSkill;
        [SerializeField] protected Image imgFillCooldown2nd;
        [SerializeField] private UIParticle vfx2ndCooldownComplete;
        [SerializeField] protected RectTransform groupPassiveAndArrow;
        [SerializeField] protected float groupPassiveOneSkillX = 0f;
        [SerializeField] protected float groupPassiveTwoSkillX = 0f;
        
        [Space] [Header("Skill icon")] 
        [SerializeField] protected Image imgIconBaseSkill1;
        [SerializeField] protected Image imgIconBaseSkill2;
        [SerializeField] protected Sprite[] iconSkills;
        
        protected virtual void Awake()
        {
            imgFillCooldown.fillAmount = 1f;
            imgFillCooldown2nd.fillAmount = 1f;
            SetupPassive();
            if (GameConst.DefaultShowPassiveIcon == false)
            {
                DoHidePassive().OnComplete(() =>
                {
                    btnTogglePassive.clickable = true;
                });
            }
        }

        public abstract void CheckShowSkill(Action callbackShow, Action callbackHide);
        
        protected virtual void OnSkillUsed(float cooldown, string toastText)
        {
            StartCoroutine(IECooldown(imgFillCooldown, groupIcon, vfxCooldownComplete, cooldown, toastText));
        }

        protected virtual void On2ndSkillUsed(float cooldown, string toastText)
        {
            StartCoroutine(IECooldown(imgFillCooldown2nd, secondSkill.transform, vfx2ndCooldownComplete, cooldown, toastText));
        }
        
        private IEnumerator IECooldown(Image imgCooldown, Transform icon, UIParticle vfx, float cooldown, string toastText)
        {
            imgCooldown.gameObject.SetActive(true);
            imgBackFadeActive.gameObject.SetActive(false);
            
            var cooldownTimer = cooldown;
            
            while (cooldownTimer > 0)
            {
                cooldownTimer -= Time.deltaTime;
                imgCooldown.fillAmount = 1 - cooldownTimer / cooldown;
                yield return null;
            }

            ShowToast(toastText);
            DOTween.Kill(groupIcon);
            imgBackFadeActive.gameObject.SetActive(true);
            icon.localScale = Vector3.one;
            icon.DOPunchScale(0.1f * Vector3.one, 0.2f).SetTarget(groupIcon);
            vfx.Play();
        }

        private void SetupPassive()
        {
            btnTogglePassive.actionHoverIn.RemoveAllListeners();
            btnTogglePassive.actionHoverIn.AddListener(() =>
            {
                btnTogglePassive.clickable = false;
                TogglePassive(true);
            });
            
            btnTogglePassive.actionHoverOut.RemoveAllListeners();
            btnTogglePassive.actionHoverOut.AddListener(() =>
            {
                btnTogglePassive.clickable = false;
                TogglePassive(false);
            });
            
            imgBackFadeActive.gameObject.SetActive(true);
        }

        private void TogglePassive(bool show)
        {
            if (show)
            {
                DoShowPassive().OnComplete(() =>
                {
                    btnTogglePassive.clickable = true;
                });
            }
            else
            {
                DoHidePassive().OnComplete(() =>
                {
                    btnTogglePassive.clickable = true;
                });
            }
        }

        private Tween DoShowPassive()
        {
            DOTween.Kill(this);

            iconTogglePassive.SetAlpha(1f);
            groupPassive.localPosition = new Vector3(passiveXLocalOnHide, groupPassive.localPosition.y, groupPassive.localPosition.z);
            cvgGroupPassive.alpha = 0f;

            return DOTween.Sequence(this)
                .Append(iconTogglePassive.DOFade(0f, 0.2f))
                .Join(rectContent.DOSizeDelta(new Vector2(rectWidthOnShow, rectContent.sizeDelta.y), 0.2f).SetEase(Ease.OutQuad))
                // .Join(imgBackFadeActive.DOFade(1f, 0.2f))
                .Append(groupPassive.DOLocalMoveX(passiveXLocalOnShow, 0.2f))
                .Join(cvgGroupPassive.DOFade(1f, 0.2f))
                .Join(cvgSkillTitle.DOFade(1f, 0.2f));
        }

        private Tween DoHidePassive()
        {
            DOTween.Kill(this);

            iconTogglePassive.SetAlpha(0f);
            groupPassive.localPosition = new Vector3(passiveXLocalOnShow, groupPassive.localPosition.y, groupPassive.localPosition.z);

            return DOTween.Sequence()
                .Append(groupPassive.DOLocalMoveX(passiveXLocalOnHide, 0.2f))
                .Join(cvgGroupPassive.DOFade(0f, 0.2f))
                .Join(cvgSkillTitle.DOFade(0f, 0.2f))
                .Append(iconTogglePassive.DOFade(1f, 0.2f))
                .Join(rectContent.DOSizeDelta(new Vector2(rectWidthOnHide, rectContent.sizeDelta.y), 0.2f)
                    .SetEase(Ease.OutQuad));
            // .Join(imgBackFadeActive.DOFade(0f, 0.2f));
        }

        protected virtual void ShowToast(string text)
        {
            ToastInGameManager.Instance.Register(string.Empty, null);
        }
    }
}