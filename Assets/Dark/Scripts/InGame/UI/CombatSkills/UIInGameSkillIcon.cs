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
        [SerializeField] protected Image imgFillCooldown;
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
        
        [Space] [Header("2nd skill")] 
        [SerializeField] protected GameObject secondSkill;
        [SerializeField] protected Image imgFillCooldown2nd;
        [SerializeField] private UIParticle vfx2ndCooldownComplete;
        [SerializeField] protected Transform groupPassiveAndArrow;
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
                DoHidePassive().OnComplete(() =>
                {
                    btnTogglePassive.interactable = true;
                });
        }

        public abstract void CheckShowSkill(Action callbackShow, Action callbackHide);
        
        protected virtual void OnSkillUsed(float cooldown)
        {
            StartCoroutine(IECooldown(imgFillCooldown, groupIcon, vfxCooldownComplete, cooldown));
        }

        protected virtual void On2ndSkillUsed(float cooldown)
        {
            StartCoroutine(IECooldown(imgFillCooldown2nd, secondSkill.transform, vfx2ndCooldownComplete, cooldown));
        }
        
        private IEnumerator IECooldown(Image imgCooldown, Transform icon, UIParticle vfx, float cooldown)
        {
            imgCooldown.gameObject.SetActive(true);
            
            var cooldownTimer = cooldown;
            
            while (cooldownTimer > 0)
            {
                cooldownTimer -= Time.deltaTime;
                imgCooldown.fillAmount = 1 - cooldownTimer / cooldown;
                yield return null;
            }

            ShowToast();
            DOTween.Kill(groupIcon);
            icon.localScale = Vector3.one;
            icon.DOPunchScale(0.1f * Vector3.one, 0.2f).SetTarget(groupIcon);
            vfx.Play();
        }

        private bool isShowPassive = true;
        
        private void SetupPassive()
        {
            btnTogglePassive.onClick.RemoveAllListeners();
            btnTogglePassive.onClick.AddListener(() =>
            {
                DOTween.Kill(groupIcon, complete:true);
                groupIcon.transform.localScale = Vector3.one;
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