using System;
using DG.Tweening;
using Economic.InGame.DropItems;
using InGame;
using UnityEngine;

namespace Economic.InGame
{
    public class EItemDropCollector : MonoBehaviour, IDamageable
    {
        [SerializeField] private PlayerCharacter player;
        [SerializeField] private Collider2D collider;
        [SerializeField] private Transform visual;
        [SerializeField] private Transform shadow;
        [SerializeField] private ParticleSystem vfxBreak;

        [Header("Config")]
        [SerializeField] private float delayRespawn = 2f;
        
        public float HitDirectionX { get; set; }
        public float HitDirectionY { get; set; }

        public void Damage(int damage, Vector2 dealerPosition, float stagger, DamageType dmgType)
        {
            collider.enabled = false;
            CombatActions.OnResourceCollectorDamaged?.Invoke(this);
            vfxBreak.Play(true);
            DoHide().OnComplete(() =>
            {
                DoSpawn().SetDelay(delayRespawn).OnComplete(() => collider.enabled = true);
            });
        }
        
        public Tween DoSpawn()
        {
            DOTween.Kill(this);
            return DOTween.Sequence(this)
                .AppendCallback(() =>
                {
                    visual.localScale = 0.2f * Vector3.one;
                    shadow.localScale = 0.2f * Vector3.one;
                    visual.gameObject.SetActive(true);
                    shadow.gameObject.SetActive(true);
                })
                .Append(visual.DOScale(1f, 0.3f).SetEase(Ease.OutQuad))
                .Join(shadow.DOScale(1f, 0.3f).SetEase(Ease.OutQuad));
        }

        public Tween DoHide()
        {
            DOTween.Kill(this);
            return DOTween.Sequence(this)
                .Append(visual.DOScale(0f, 0.5f).SetEase(Ease.OutBack))
                .Join(shadow.DOScale(0f, 0.5f).SetEase(Ease.OutBack))
                .AppendCallback(() =>
                {
                    visual.gameObject.SetActive(false);
                    shadow.gameObject.SetActive(false);
                });
        }

        public bool IsDestroyed { get; set; }
        public Action<int, DamageType> OnHit { get; set; }
    }
}