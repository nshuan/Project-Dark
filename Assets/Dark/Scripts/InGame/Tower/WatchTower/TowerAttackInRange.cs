using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using InGame.Upgrade;
using Unity.VisualScripting;
using UnityEngine;

namespace InGame.WatchTower
{
    public class TowerAttackInRange : MonoBehaviour
    {
        private static readonly int RadialProgress = Shader.PropertyToID("_RadialProgress");
        
        [SerializeField] protected TowerEntity tower;
        [SerializeField] protected NodeTowerCounter.CounterType counterType;
        [SerializeField] protected ProjectileEntity projectilePrefab;
        [SerializeField] protected GameObject vfxActivateCounter;
        [SerializeField] protected Transform visual;
        [SerializeField] protected GameObject visualFill;
        [SerializeField] protected float vfxActivateCounterDuration;
        [SerializeField] protected float bulletSpeedScale = 2f;

        [Space] [Header("Range")] 
        [SerializeField] protected Transform detectRange;
        [SerializeField] protected CircleCollider2D detectCollider;

        [Space] [Header("Visual")]
        [SerializeField] protected SpriteRenderer spriteFill;
        [SerializeField] protected GameObject fillFull;
        
        protected bool counterCooldown;

        [Space] [Header("Config")] 
        public bool canCounter;

        protected int Damage => GetCounterDamage();
        protected float Cooldown => GetCounterCooldown();
        protected float DetectRange => GetRangeRadius();
        protected float DelayOnDetected => GetDelayOnDetectedEnemy();

        protected Vector2 counterDirection = Vector2.zero;

        protected Coroutine coroutineCounter;
        protected Coroutine coroutineCooldown;
        protected List<Transform> inRangeEnemies;
        protected Material fillMaterial;
        
        private void Awake()
        {
            fillMaterial = new Material(spriteFill.sharedMaterial);
            spriteFill.material = fillMaterial;
            
            LevelManager.Instance.OnInitTowers += OnInitTowers;
            LevelManager.Instance.OnLose += OnLose;
            tower.OnDestroyed += OnTowerDestroyed;
            detectRange.localScale = DetectRange * Vector3.one;
            detectCollider.radius = DetectRange;
            visual.gameObject.SetActive(false);
            SetRingFillFull(false);
            fillMaterial.SetFloat(RadialProgress, 0f);
            inRangeEnemies = new List<Transform>();
        }

        private void OnDestroy()
        {
            LevelManager.Instance.OnInitTowers -= OnInitTowers;
        }

        #region Config
        
        private int GetCounterDamage()
        {
            return counterType switch
            {
                NodeTowerCounter.CounterType.Pierce => LevelUtilityV2.GetCounterPiercingDamage(),
                NodeTowerCounter.CounterType.Slash => LevelUtilityV2.GetCounterSlashDamage(),
                _ => 1
            };
        }

        private float GetCounterCooldown()
        {
            return counterType switch
            {
                NodeTowerCounter.CounterType.Pierce => LevelUtilityV2.GetCounterPiercingCooldown(),
                NodeTowerCounter.CounterType.Slash => LevelUtilityV2.GetCounterSlashCooldown(),
                _ => 1
            };
        }

        private float GetRangeRadius()
        {
            return counterType switch
            {
                NodeTowerCounter.CounterType.Pierce => LevelUtilityV2.GetCounterPiercingDetectRange(),
                NodeTowerCounter.CounterType.Slash => LevelUtilityV2.GetCounterSlashDetectRange(),
                _ => 1
            };
        }
        
        private float GetDelayOnDetectedEnemy()
        {
            return counterType switch
            {
                NodeTowerCounter.CounterType.Pierce => LevelUtilityV2.GetCounterPiercingDelayAfterDetected(),
                NodeTowerCounter.CounterType.Slash => LevelUtilityV2.GetCounterSlashDelayAfterDetected(),
                _ => 1
            };
        }
        
        #endregion
        private void OnInitTowers()
        {
            var bonusInfo = LevelUtilityV2.BonusInfo;
            if (counterType == NodeTowerCounter.CounterType.Pierce)
                canCounter = bonusInfo.bonusUnlockSkill.unlockCounterPiercing;
            else if (counterType == NodeTowerCounter.CounterType.Slash)
                canCounter = bonusInfo.bonusUnlockSkill.unlockCounterSlash;
            
            detectRange.gameObject.SetActive(canCounter);
            visual.gameObject.SetActive(canCounter);
        }

        private void OnTowerDestroyed(TowerEntity destroyedTower)
        {
            canCounter = false;
        }
        
        private void OnLose()
        {
            LevelManager.Instance.OnLose -= OnLose;
            canCounter = false;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!canCounter) return;
            if (!other.CompareTag("Enemy")) return;
            if (!other.transform.TryGetComponent<EnemyEntity>(out var enemy)) return;
            var triggerDirection = other.transform.position - transform.position;
            if (LevelUtilityV2.GetRelativeRange(DetectRange, triggerDirection) < triggerDirection.magnitude) return;

            enemy.OnDead += OnEnemyDead;
            inRangeEnemies.Add(enemy.transform);
            counterDirection.x = other.transform.position.x - visual.position.x;
            counterDirection.y = other.transform.position.y - visual.position.y;
            if (counterCooldown) return;
            if (coroutineCounter == null) coroutineCounter = StartCoroutine(IECounter(Cooldown));
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (inRangeEnemies.Contains(other.transform)) inRangeEnemies.Remove(other.transform);
            if (inRangeEnemies.Count == 0 && coroutineCounter != null)
            {
                StopCoroutine(coroutineCounter);
                coroutineCounter = null;
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            OnTriggerEnter2D(other);
        }

        private void OnEnemyDead(EnemyEntity enemy, EnemyDieReason dieReason)
        {
            if (inRangeEnemies.Contains(enemy.transform)) inRangeEnemies.Remove(enemy.transform);
            if (inRangeEnemies.Count == 0 && coroutineCounter != null)
            {
                StopCoroutine(coroutineCounter);
                coroutineCounter = null;
            }
        }

        protected virtual IEnumerator IECounter(float cooldown)
        {
            DOTween.Kill(vfxActivateCounter);
            var delayOnDetectTimer = DelayOnDetected;
            while (delayOnDetectTimer > 0)
            {
                delayOnDetectTimer -= Time.deltaTime;
                // var bestTarget = FindMostCrowdedEnemy(inRangeEnemies, 2f);
                // if (bestTarget)
                // {
                //     counterDirection.x = bestTarget.position.x - visual.position.x;
                //     counterDirection.y = bestTarget.position.y - visual.position.y;
                // }
                visual.rotation = Quaternion.Euler(0f, 0f,  Mathf.Atan2(counterDirection.y, counterDirection.x) * Mathf.Rad2Deg);
                fillMaterial.SetFloat(RadialProgress, (1f - delayOnDetectTimer / DelayOnDetected) * -360f);
                yield return null;
            }
            if (inRangeEnemies.Count <= 0)
                yield break;
            
            if (coroutineCooldown != null) StopCoroutine(coroutineCooldown);
            coroutineCooldown = StartCoroutine(IECooldown(cooldown));
                
            vfxActivateCounter.transform.rotation = Quaternion.Euler(0f, 0f,  Mathf.Atan2(counterDirection.y, counterDirection.x) * Mathf.Rad2Deg);
            vfxActivateCounter.SetActive(true);
            Counter(visual.position, counterDirection, Damage, bulletSpeedScale);
            DOTween.Kill(fillFull);
            SetRingFillFull(true);
            DOVirtual.DelayedCall(0.2f, () => SetRingFillFull(false)).SetTarget(fillFull);
            fillMaterial.SetFloat(RadialProgress, 0f);
            DOVirtual.DelayedCall(vfxActivateCounterDuration, () =>
            {
                vfxActivateCounter.SetActive(false);
            }).SetTarget(vfxActivateCounter);
            
            coroutineCounter = null;
        }
        
        public virtual void Counter(Vector2 towerAttackPos, Vector2 direction, int damage, float speedScale)
        {
            var projectile = ProjectilePool.Instance.Get(projectilePrefab, null, false);
            var stagger = counterType switch
            {
                NodeTowerCounter.CounterType.Pierce => LevelUtilityV2.StatsCounterPiercing.stagger,
                NodeTowerCounter.CounterType.Slash => LevelUtilityV2.StatsCounterSlash.stagger,
                _ => 1
            };
            var maxHit = 20;
            var size = LevelUtilityV2.GetCounterPiercingSize();
            projectile.transform.position = towerAttackPos;
            projectile.transform.rotation = Quaternion.Euler(0f, 0f,  Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
            projectile.Init(towerAttackPos, direction.normalized, 8, size, speedScale, damage, damage, 0f, stagger, false, maxHit, null, null, ProjectileType.TowerProjectile);
            projectile.BlockDestroy = true;
            projectile.Activate(0f);
        }

        private IEnumerator IECooldown(float cooldown)
        {
            counterCooldown = true;
            visualFill.gameObject.SetActive(false);
            yield return new WaitForSeconds(cooldown);
            counterCooldown = false;
            visualFill.gameObject.SetActive(true);
        }

        private void SetRingFillFull(bool show)
        {
            fillFull.SetActive(show);
        }
        
        private Transform FindMostCrowdedEnemy(List<Transform> enemies, float radius)
        {
            Transform bestTarget = null;
            int maxCount = 0;

            float radiusSqr = radius * radius;

            for (int i = 0; i < enemies.Count; i++)
            {
                int count = 0;
                Vector3 center = enemies[i].position;

                for (int j = 0; j < enemies.Count; j++)
                {
                    if (i == j) continue;

                    if ((enemies[j].position - center).sqrMagnitude <= radiusSqr)
                    {
                        count++;
                    }
                }

                if (count > maxCount)
                {
                    maxCount = count;
                    bestTarget = enemies[i];
                }
            }

            return bestTarget;
        }
    }
}