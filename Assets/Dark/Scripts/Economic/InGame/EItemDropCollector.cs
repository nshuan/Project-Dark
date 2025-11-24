using System;
using Dark.Scripts.Utils;
using DG.Tweening;
using Economic.InGame.DropItems;
using InGame;
using UnityEngine;
using UnityEngine.Serialization;

namespace Economic.InGame
{
    public class EItemDropCollector : MonoBehaviour, IDamageable
    {
        [SerializeField] private Collider2D collider;
        [SerializeField] private Transform visual;
        [SerializeField] private Transform shadow;
        [SerializeField] private EllipticalOrbit orbitMovement;
        [SerializeField] private ParticleSystem vfxBreak;
        [SerializeField] private ParticleSystem vfxSpawn;

        [Header("Echoes")] 
        [SerializeField] private Transform visualEchoes;
        [SerializeField] private Transform shadowEchoes;
        
        [Header("Config")]
        [SerializeField] private float delayRespawn = 2f;
        [SerializeField] private float heightFromPlayer = 2f;

        private Transform player;
        private bool activated = false;
        private bool orbitMoving;
        private float orbitTimer;
        
        public float HitDirectionX { get; set; }
        public float HitDirectionY { get; set; }

        private void Awake()
        {
            visual.gameObject.SetActive(false);
            shadow.gameObject.SetActive(false);
            visualEchoes.gameObject.SetActive(false);
            shadowEchoes.gameObject.SetActive(false);
            LevelManager.Instance.OnLevelLoaded += OnLevelLoaded;
            CombatActions.OnMoveTower += OnMoveTower;
            WealthManager.Instance.OnUpGrade += OnCharacterLevelUp;
        }

        private void OnDestroy()
        {
            CombatActions.OnMoveTower -= OnMoveTower;
            WealthManager.Instance.OnUpGrade -= OnCharacterLevelUp;
        }

        private void Update()
        {
            if (!activated) return;
            if (orbitTimer > 0) orbitTimer -= Time.deltaTime;
            else
            {
                if (orbitMoving)
                {
                    orbitMovement.PauseOrbit();
                    orbitTimer = RandomUtil.Range(2f, 4f);
                    orbitMoving = false;
                }
                else
                {
                    orbitMovement.ResumeOrbit();
                    orbitTimer = RandomUtil.Range(3f, 5f);
                    orbitMoving = true;
                }
            }
        }

        private void OnLevelLoaded(LevelConfig level)
        {
            player = LevelManager.Instance.Player.transform;
            transform.position = player.position + new Vector3(0f, heightFromPlayer, 0f);
            orbitMovement.ResetOrbit();
            orbitMovement.StartOrbit();
            orbitMoving = false;
            DoSpawn().SetDelay(1f).OnComplete(() =>
            {
                activated = true;
                collider.enabled = true;
            });
        }

        private void OnMoveTower(float cooldown)
        {
            if (!player) return;
            activated = false;
            DoHide().OnComplete(() =>
            {
                transform.position = player.position + new Vector3(0f, heightFromPlayer, 0f);
                orbitMovement.ResetOrbit();
                orbitMovement.StartOrbit();
                DoSpawn().SetDelay(delayRespawn).OnComplete(() =>
                {
                    activated = true;
                    collider.enabled = true;
                });
            });
        }
        
        private void OnCharacterLevelUp(int obj)
        {
            activated = false;
            DoShowEchoes().OnComplete(() =>
            {
                transform.position = player.position + new Vector3(0f, heightFromPlayer, 0f);
                orbitMovement.ResetOrbit();
                orbitMovement.StartOrbit();
                DoSpawn().SetDelay(delayRespawn).OnComplete(() =>
                {
                    activated = true;
                    collider.enabled = true;
                });
            });
        }
        
        public void Damage(int damage, Vector2 dealerPosition, float stagger, DamageType dmgType)
        {
            if (!activated) return;
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
                    vfxSpawn.Play(true);
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

        private Tween DoShowEchoes()
        {
            DOTween.Kill(this, true);
            return DOTween.Sequence(this)
                .Append(transform.DOScale(1.1f, 0.2f).SetEase(Ease.OutQuad))
                .AppendCallback(() =>
                {
                    visual.gameObject.SetActive(false);
                    shadowEchoes.gameObject.SetActive(false);
                    visualEchoes.gameObject.SetActive(true);
                    shadowEchoes.gameObject.SetActive(true);
                    vfxSpawn.Play(true);
                })
                .Append(transform.DOScale(1f, 0.2f).SetEase(Ease.InQuad))
                .AppendInterval(1f)
                .Append(visualEchoes.DOScale(0f, 0.5f).SetEase(Ease.OutBack))
                .Join(shadowEchoes.DOScale(0f, 0.5f).SetEase(Ease.OutBack))
                .AppendCallback(() =>
                {
                    visualEchoes.gameObject.SetActive(false);
                    shadowEchoes.gameObject.SetActive(false);
                });
        }

        public bool IsDestroyed { get; set; }
        public Action<int, DamageType> OnHit { get; set; }
    }
}