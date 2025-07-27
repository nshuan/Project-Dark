using System;
using System.Collections;
using DG.Tweening;
using InGame.Effects;
using UnityEngine;

namespace InGame
{
    public class PlayerCharacter : MonoBehaviour
    {
        [SerializeField] private PlayerAnimController animController;
        [SerializeField] private PlayerDashEffect dashEffect;
        [SerializeField] private PLayerFlashEffect flashEffect;
        [SerializeField] private GameObject chargeEffect;
        [SerializeField] private SpriteRenderer spriteRenderer;

        [Space] [SerializeField] private WeaponSupporter weapon;
        public WeaponSupporter Weapon => weapon;

        [Space] [Header("Config")] 
        [SerializeField] private Vector2 offset;

        private bool blockRotate;
        
        // Return the duration to finish the 1st animation phase, when the skill is actually strike
        public float PlayShoot(Vector2 target)
        {
            // transform.localScale =
            //     new Vector3(Mathf.Sign(target.x - transform.position.x), 1f, 1f);
            var attackDuration = animController.PlayAttack();
            blockRotate = true;
            StartCoroutine(IEBlockRotate(attackDuration.Item2));
            return attackDuration.Item1;
        }

        public void PlayCharge()
        {
            StartCoroutine(IEChargeEffect(animController.PlayCharge()));
        }

        private IEnumerator IEChargeEffect(float delay)
        {
            yield return new WaitForSeconds(delay);
            chargeEffect.SetActive(true);
        }

        public void EndChargeAndShoot()
        {
            chargeEffect.SetActive(false);
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

        public void PlayDashEffect(Vector2 direction)
        {
            dashEffect.PLayStart(direction);
            spriteRenderer.gameObject.SetActive(false);
        }

        public void StopDashEffect()
        {
            spriteRenderer.gameObject.SetActive(true);
            dashEffect.PLayEnd();
        }

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
            return flashSequence.AppendInterval(Mathf.Max(0f, 0.15f))
                .AppendCallback(() =>
                {
                    flashEffect.PlayAoe();
                    onLanded?.Invoke();
                })
                .Append(spriteRenderer.DOFade(1f, 0.2f));
        }
    }
}