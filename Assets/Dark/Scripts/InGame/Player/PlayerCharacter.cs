using System;
using System.Collections;
using Dark.Scripts.AudioV2;
using DG.Tweening;
using InGame.Effects;
using UnityEngine;

namespace InGame
{
    public class PlayerCharacter : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private PlayerAnimController animController;
        [SerializeField] private PlayerDashEffect dashEffect;
        [SerializeField] private PLayerFlashEffect flashEffect;
        [SerializeField] private PlayerTeleEffect teleEffect;
        [SerializeField] private AudioPlayComponentV2 sfxAttack;

        [Space] [SerializeField] private WeaponSupporter weapon;
        public WeaponSupporter Weapon => weapon;

        [Space] [Header("Config")] 
        [SerializeField] private Vector2 offset;
        
        private bool blockRotate;

        private void Awake()
        {
            shotRadius.SetParent(null);
            shotRadius.localScale = Vector3.zero;
        }

        // Return the duration to finish the 1st animation phase, when the skill is actually strike
        public float PlayShoot(Vector2 target)
        {
            // transform.localScale =
            //     new Vector3(Mathf.Sign(target.x - transform.position.x), 1f, 1f);
            var attackDuration = animController.PlayAttack();
            sfxAttack?.Play();
            blockRotate = true;
            StartCoroutine(IEBlockRotate(attackDuration.Item2));
            return attackDuration.Item1;
        }

        public void PlayCharge()
        {
            animController.PlayCharge();
        }

        public void UpdateChargeScale(float scale)
        {
            animController.UpdateChargeScale(scale);
        }

        public void EndChargeAndShoot()
        {
            animController.EndChargeAndShoot();
        }

        private IEnumerator IEBlockRotate(float duration)
        {
            yield return new WaitForSeconds(duration);
            blockRotate = false;
        }

        public void SetDirection(Vector3 target)
        {
            if (blockRotate) return;
            animController.SetDirection(target - transform.position);
        }

        #region Tele

        private Sequence teleSequence;
        public Tween PLayTeleEffect(Vector2 endPos)
        {
            teleSequence.Kill();
            teleSequence = DOTween.Sequence();
            teleEffect.PlayStartCharging();
            teleEffect.PlayEndCharging(endPos);
            return teleSequence.AppendInterval(0.2f)
                .AppendCallback(() =>
                {
                    teleEffect.PLayStart();
                    spriteRenderer.DOFade(0f, 0.1f);
                })
                .AppendInterval(Mathf.Max(0f, teleEffect.startDuration - 0.2f));
        }

        public Tween StopTeleEffect()
        {
            teleSequence.Kill();
            teleSequence = DOTween.Sequence();

            return teleSequence.AppendCallback(() =>
            {
                teleEffect.PLayEnd();
                spriteRenderer.DOFade(1f, 0.1f);
            });
        }

        #endregion
        
        #region Dash

        public void PlayDashEffect(Vector2 direction)
        {
            dashEffect.PLayStart(direction);
            spriteRenderer.gameObject.SetActive(false);
        }

        public void StopDashEffect()
        {
            spriteRenderer.gameObject.SetActive(true);
            dashEffect.PLayEnd();
            dashEffect.transform.position = transform.position;
            dashEffect.transform.SetParent(transform);
        }

        public Transform PlayOnlyDashEffect(Vector2 startPos, Vector2 direction)
        {
            dashEffect.transform.SetParent(null);
            dashEffect.transform.position = startPos;
            dashEffect.PLayStart(direction);
            return dashEffect.transform;
        }

        #endregion

        #region Flash
        
        private Sequence flashSequence;
        public Tween PLayFlashEffect()
        {
            flashSequence.Kill();
            flashSequence = DOTween.Sequence();
            flashEffect.PLayStart();
            return flashSequence.Append(spriteRenderer.DOFade(0f, 0.2f))
                .AppendInterval(Mathf.Max(0f, flashEffect.startDuration - 0.2f));
        }

        public Tween StopFlashEffect(Action onLanded)
        {
            flashSequence.Kill();
            flashSequence = DOTween.Sequence();
            flashEffect.PLayEnd();
            return flashSequence.AppendInterval(0.15f)
                .AppendCallback(() =>
                {
                    PlayAoe();
                    onLanded?.Invoke();
                })
                .Append(spriteRenderer.DOFade(1f, 0.2f));
        }

        public void PlayAoe()
        {
            flashEffect.PlayAoe();
        }
        
        #endregion

        #region Motion Blur

        [Space] [Header("Motion Blur")] 
        [SerializeField] private string defaultSortingLayerName;
        [SerializeField] private string motionSortingLayerName;
        
        public void OnMotionBlur()
        {
            spriteRenderer.sortingLayerName = motionSortingLayerName;
        }

        public void OnEndMotionBlur()
        {
            spriteRenderer.sortingLayerName = defaultSortingLayerName;
        }

        #endregion
        
        [Space] [Header("Debug")] public Transform shotRadius;
        public void UpdateShotRadius(float radius, bool immediatelly = true)
        {
            if (immediatelly)
            {
                shotRadius.localScale = new Vector3(radius, radius, radius);
            }
            else
            {
                DOTween.Kill(shotRadius);
                shotRadius.DOScale(radius, 0.2f).SetTarget(shotRadius).SetDelay(0.3f);
            }
        }

        public void HideShotRadius()
        {
            DOTween.Kill(shotRadius);
            shotRadius.DOScale(0f, 0.2f).SetTarget(shotRadius).SetEase(Ease.InQuad);
        }

        public void ShowShotRadius(Vector2 position, float radius)
        {
            DOTween.Kill(shotRadius);
            shotRadius.localScale = Vector3.zero;
            shotRadius.position = position;
            UpdateShotRadius(radius, false);
        }
    }
}