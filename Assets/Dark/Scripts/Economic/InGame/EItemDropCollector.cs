using System;
using System.Collections.Generic;
using System.Linq;
using Dark.Scripts.AudioV2;
using DG.Tweening;
using InGame;
using UnityEngine;

namespace Economic.InGame
{
    public class EItemDropCollector : MonoBehaviour
    {
        [SerializeField] private Collider2D collider;
        [SerializeField] private Transform visual;
        [SerializeField] private Transform shadow;
        [SerializeField] private EllipticalOrbit orbitMovement;
        [SerializeField] private FloatingEffect floatingMovement;
        [SerializeField] private ParticleSystem vfxBreak;
        [SerializeField] private ParticleSystem vfxSpawn;
        [SerializeField] private AudioPlayComponentV2 sfxCollect;

        [Header("Echoes")] 
        [SerializeField] private Transform visualEchoes;
        [SerializeField] private Transform shadowEchoes;
        
        [Header("Config")]
        [SerializeField] private float delayRespawn = 2f;
        
        [Space] [Header("Auto detect enemy")] 
        [SerializeField] private float checkEnemyInterval;
        [SerializeField] private float radiusCheckEnemy;
        [SerializeField] private LayerMask enemyLayer;
        
        private List<Transform[]> orbitCenters; // Map by tower id
        private bool activated = false;
        private bool orbitMoving;
        private float orbitTimer;

        private float checkEnemyCounter;
        private int currentPositionIndexInTower;

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
            
            // Check nếu có enemy gần thì di chuyển sang chỗ khác
            if (checkEnemyCounter < 0f)
            {
                var enemiesHit = new RaycastHit2D[1];
                var enemyCount = Physics2D.CircleCastNonAlloc(transform.position, radiusCheckEnemy, Vector2.zero, enemiesHit, 0f, enemyLayer);
                if (enemyCount > 0)
                {
                    TeleportToOtherPosition();
                }
                checkEnemyCounter = checkEnemyInterval;
            }
            else
            {
                checkEnemyCounter -= Time.deltaTime;
            }
        }

        private void OnLevelLoaded(LevelConfig level)
        {
            currentPositionIndexInTower = 0;
            orbitCenters = LevelManager.Instance.Towers.Select((tower) => tower.itemCollectorPositions).ToList();
            transform.position = LevelManager.Instance.CurrentTower.itemCollectorPositions[currentPositionIndexInTower].position;
            orbitMovement.ResetOrbit();
            orbitMovement.StartOrbit();
            orbitMovement.ResumeOrbit();
            orbitMoving = true;
            orbitTimer = 0f;
            DoSpawn().SetDelay(1f).OnComplete(() =>
            {
                activated = true;
                collider.enabled = true;
                CombatActions.OnResourceCollectorInitialized?.Invoke(this);
            });
        }

        private void OnMoveTower(float cooldown)
        {
            var id = LevelManager.Instance.CurrentTower.Id;
            if (id < 0 || id >= orbitCenters.Count) return;
            activated = false;
            DoHide().OnComplete(() =>
            {
                currentPositionIndexInTower = RandomUtil.Range(0, orbitCenters[id].Length);
                transform.position = orbitCenters[id][currentPositionIndexInTower].position;
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
                if (id >= 0 && id < orbitCenters.Count) transform.position = orbitCenters[id][currentPositionIndexInTower].position;
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

        private void TeleportToOtherPosition()
        {
            collider.enabled = false;
            DoHide().OnComplete(() =>
            {
                var id = LevelManager.Instance.CurrentTower.Id;
                if (currentPositionIndexInTower == 0 && id >= 0 && id < orbitCenters.Count)
                    currentPositionIndexInTower = RandomUtil.Range(1, orbitCenters[id].Length);
                else currentPositionIndexInTower = 0;
                transform.position = orbitCenters[id][currentPositionIndexInTower].position;
                
                orbitMovement.ResetOrbit();
                orbitMovement.StartOrbit();
                orbitMovement.ResumeOrbit();
                orbitMoving = true;
                orbitTimer = 0f;
                DoSpawn().SetDelay(delayRespawn).OnComplete(() =>
                {
                    collider.enabled = true;
                });
            });
        }
        
        public void Break()
        {
            if (!activated) return;
            collider.enabled = false;
            CombatActions.OnResourceCollectorDamaged?.Invoke(this);
            vfxBreak.Play(true);
            sfxCollect.Play();
            TeleportToOtherPosition();
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
    }
}