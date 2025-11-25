using System;
using System.Linq;
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
        [SerializeField] private FloatingEffect floatingMovement;
        [SerializeField] private ParticleSystem vfxBreak;
        [SerializeField] private ParticleSystem vfxSpawn;

        [Header("Echoes")] 
        [SerializeField] private Transform visualEchoes;
        [SerializeField] private Transform shadowEchoes;
        
        [Header("Config")]
        [SerializeField] private float delayRespawn = 2f;
        [SerializeField] private float heightFromPlayer = 2f;
        
        private Transform[] orbitCenters; // Map by tower id
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
            CombatActions.OnMoveTowerComplete += OnMoveTower;
            WealthManager.Instance.OnUpGrade += OnCharacterLevelUp;
        }

        private void OnDestroy()
        {
            CombatActions.OnMoveTowerComplete -= OnMoveTower;
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
                    floatingMovement.ResumeFloat();
                    orbitTimer = RandomUtil.Range(2f, 4f);
                    orbitMoving = false;
                }
                else
                {
                    orbitMovement.ResumeOrbit();
                    floatingMovement.PauseFloat();
                    orbitTimer = RandomUtil.Range(3f, 5f);
                    orbitMoving = true;
                }
            }
        }

        private void OnLevelLoaded(LevelConfig level)
        {
            orbitCenters = LevelManager.Instance.Towers.Select((tower) => tower.itemCollectorPosition).ToArray();
            transform.position = LevelManager.Instance.CurrentTower.itemCollectorPosition.position;
            orbitMovement.ResetOrbit();
            orbitMovement.StartOrbit();
            orbitMovement.ResumeOrbit();
            orbitMoving = true;
            orbitTimer = 0f;
            DoSpawn().SetDelay(1f).OnComplete(() =>
            {
                activated = true;
                collider.enabled = true;
            });
        }

        private void OnMoveTower(float cooldown)
        {
            var id = LevelManager.Instance.CurrentTower.Id;
            if (id < 0 || id >= orbitCenters.Length) return;
            activated = false;
            DoHide().OnComplete(() =>
            {
                transform.position = orbitCenters[id].position;
                orbitMovement.ResetOrbit();
                orbitMovement.StartOrbit();
                orbitMovement.ResumeOrbit();
                orbitMoving = true;
                orbitTimer = 0.001f;
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
                var id = LevelManager.Instance.CurrentTower.Id;
                if (id >= 0 && id < orbitCenters.Length) transform.position = orbitCenters[id].position;
                orbitMovement.ResetOrbit();
                orbitMovement.StartOrbit();
                orbitMovement.ResumeOrbit();
                orbitMoving = true;
                orbitTimer = 0f;
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
                    visualEchoes.gameObject.SetActive(false);
                    shadowEchoes.gameObject.SetActive(false);
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
                    visualEchoes.gameObject.SetActive(false);
                    shadowEchoes.gameObject.SetActive(false);
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
                    shadow.gameObject.SetActive(false);
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