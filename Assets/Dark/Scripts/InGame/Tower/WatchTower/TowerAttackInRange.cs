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
        private static readonly int LinearProgress = Shader.PropertyToID("_LinearProgress");
        
        [SerializeField] protected TowerEntity tower;
        [SerializeField] protected ProjectileEntity projectilePrefab;
        [SerializeField] protected GameObject vfxActivateCounterPierce;
        [SerializeField] protected GameObject vfxActivateCounterSlash;
        [SerializeField] protected Transform visual;
        [SerializeField] protected SpriteRenderer visualBase;
        [SerializeField] protected SpriteRenderer visualFill;
        [SerializeField] protected float vfxActivateCounterDuration;
        [SerializeField] protected float bulletSpeedScale = 2f;
        [SerializeField] protected float yOffsetWhenEnemyStay = 1.5f;
        [SerializeField] private LayerMask hitLayer;
        [SerializeField] private GameObject vfxCooldownComplete;
        [SerializeField] private GameObject vfxCooldownCompleteLoop;

        [Space] [Header("Range")] 
        [SerializeField] protected Transform detectRange;
        [SerializeField] protected CircleCollider2D detectCollider;

        [Space] [Header("Visual")]
        [SerializeField] protected SpriteRenderer spriteFill;
        [SerializeField] protected SpriteRenderer fillFull;
        [SerializeField] protected SpriteRenderer fillFullGlow;
        [SerializeField] protected Sprite spriteArcherBase;
        [SerializeField] protected Sprite spriteArcherFill;
        [SerializeField] protected Sprite spriteKnightBase;
        [SerializeField] protected Sprite spriteKnightFill;
        [SerializeField] protected Sprite spriteBothBase;
        [SerializeField] protected Sprite spriteBothFill;

        protected bool counterCooldown;
        private float visualBaseLocalY;

        [Space] [Header("Config")] 
        public bool canCounterPierce;
        public bool canCounterSlash;

        protected bool CanCounter => canCounterPierce || canCounterSlash;

        protected int DamageArcher => GetCounterDamage(NodeTowerCounter.CounterType.Pierce);
        protected int DamageKnight => GetCounterDamage(NodeTowerCounter.CounterType.Slash);
        protected float Cooldown { get; set; }
        protected float DetectRangeArcher => GetRangeRadius(NodeTowerCounter.CounterType.Pierce);
        protected float DetectRangeKnight => GetRangeRadius(NodeTowerCounter.CounterType.Slash);
        protected float DelayOnDetected { get; set; }

        protected Vector2 counterDirection = Vector2.zero;

        protected Coroutine coroutineCounter;
        protected Coroutine coroutineCooldown;
        protected List<Transform> inRangeEnemies;
        protected Material fillMaterial;
        protected Material iconFillMaterial;
        private RaycastHit2D[] slashHits = new RaycastHit2D[20];
        private EnemyEntity slashCacheEnemy;
        private GameObject vfxActivateCounter;
        
        private void Awake()
        {
            visualBaseLocalY = visual.localPosition.y;
            fillMaterial = new Material(spriteFill.sharedMaterial);
            spriteFill.material = fillMaterial;
            iconFillMaterial = new Material(visualFill.sharedMaterial);
            visualFill.material = iconFillMaterial;
            vfxActivateCounterPierce.SetActive(false);
            vfxActivateCounterSlash.SetActive(false);
            
            LevelManager.Instance.OnInitTowers += OnInitTowers;
            LevelManager.Instance.OnLose += OnLose;
            tower.OnDestroyed += OnTowerDestroyed;
            var range = Mathf.Max(DetectRangeArcher, DetectRangeKnight);
            detectRange.localScale = range * Vector3.one;
            detectCollider.radius = range;
            visual.gameObject.SetActive(false);
            SetRingFillFull(false);
            fillMaterial.SetFloat(RadialProgress, 0f);
            iconFillMaterial.SetFloat(LinearProgress, 1f);
            inRangeEnemies = new List<Transform>();
            vfxCooldownComplete?.SetActive(false);
            vfxCooldownCompleteLoop?.SetActive(false);

            LevelManager.Instance.OnChangeTower += OnChangeTower;
        }

        private void OnDestroy()
        {
            LevelManager.Instance.OnInitTowers -= OnInitTowers;
        }

        #region Config
        
        private int GetCounterDamage(NodeTowerCounter.CounterType counterType)
        {
            return counterType switch
            {
                NodeTowerCounter.CounterType.Pierce => LevelUtilityV2.GetCounterPiercingDamage(),
                NodeTowerCounter.CounterType.Slash => LevelUtilityV2.GetCounterSlashDamage(),
                _ => 1
            };
        }

        private float GetCounterCooldown(NodeTowerCounter.CounterType counterType)
        {
            return counterType switch
            {
                NodeTowerCounter.CounterType.Pierce => LevelUtilityV2.GetCounterPiercingCooldown(),
                NodeTowerCounter.CounterType.Slash => LevelUtilityV2.GetCounterSlashCooldown(),
                _ => 1
            };
        }

        private float GetRangeRadius(NodeTowerCounter.CounterType counterType)
        {
            return counterType switch
            {
                NodeTowerCounter.CounterType.Pierce => LevelUtilityV2.GetCounterPiercingDetectRange(),
                NodeTowerCounter.CounterType.Slash => LevelUtilityV2.GetCounterSlashDetectRange(),
                _ => 1
            };
        }
        
        private float GetDelayOnDetectedEnemy(NodeTowerCounter.CounterType counterType)
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
            canCounterPierce = bonusInfo.bonusUnlockSkill.unlockCounterPiercing;
            canCounterSlash = bonusInfo.bonusUnlockSkill.unlockCounterSlash;
            
            detectRange.gameObject.SetActive(CanCounter);
            visual.gameObject.SetActive(CanCounter);
            Cooldown = Mathf.Min(GetCounterCooldown(NodeTowerCounter.CounterType.Pierce),
                GetCounterCooldown(NodeTowerCounter.CounterType.Slash));
            DelayOnDetected = Mathf.Min(LevelUtilityV2.GetCounterPiercingDelayAfterDetected(),
                LevelUtilityV2.GetCounterSlashDelayAfterDetected());
            if (canCounterSlash) vfxActivateCounter = vfxActivateCounterSlash;
            else vfxActivateCounter = vfxActivateCounterPierce;

            SetVisual();
            iconFillMaterial.SetFloat(LinearProgress, 1f);
            vfxCooldownComplete?.SetActive(true);
            vfxCooldownCompleteLoop?.SetActive(true);
        }
        
        private void OnChangeTower(TowerEntity t)
        {
            if (t.Id == tower.Id) visual.localPosition = new Vector3(visual.localPosition.x, visualBaseLocalY + yOffsetWhenEnemyStay, visual.localPosition.z);
            else visual.localPosition = new Vector3(visual.localPosition.x, visualBaseLocalY, visual.localPosition.z);
        }

        private void OnTowerDestroyed(TowerEntity destroyedTower)
        {
            canCounterPierce = false;
            canCounterSlash = false;
        }
        
        private void OnLose()
        {
            LevelManager.Instance.OnLose -= OnLose;
            canCounterPierce = false;
            canCounterSlash = false;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!CanCounter) return;
            if (!other.CompareTag("Enemy")) return;
            if (!other.transform.TryGetComponent<EnemyEntity>(out var enemy)) return;
            var triggerDirection = other.transform.position - transform.position;
            if (LevelUtilityV2.GetRelativeRange(
                    DetectRangeArcher >= DetectRangeKnight ? DetectRangeArcher : DetectRangeKnight, 
                    triggerDirection) < triggerDirection.magnitude) return;

            enemy.OnStartDead += OnEnemyDead;
            inRangeEnemies.Add(enemy.transform);
            counterDirection.x = other.transform.position.x - transform.position.x;
            counterDirection.y = other.transform.position.y - transform.position.y;
            if (counterCooldown) return;
            if (coroutineCounter == null) coroutineCounter = StartCoroutine(IECounter(Cooldown));
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (inRangeEnemies.Contains(other.transform)) inRangeEnemies.Remove(other.transform);
            if (inRangeEnemies.Count == 0 && coroutineCounter != null)
            {
                TerminateCounter();
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            OnTriggerEnter2D(other);
        }

        private void OnEnemyDead(EnemyEntity enemy)
        {
            if (inRangeEnemies.Contains(enemy.transform)) inRangeEnemies.Remove(enemy.transform);
            if (inRangeEnemies.Count == 0 && coroutineCounter != null)
            {
                TerminateCounter();
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
                fillMaterial.SetFloat(RadialProgress, (1f - delayOnDetectTimer / DelayOnDetected) * -360f);
                yield return null;
            }
            if (inRangeEnemies.Count <= 0)
                yield break;
            
            if (coroutineCooldown != null) StopCoroutine(coroutineCooldown);
            coroutineCooldown = StartCoroutine(IECooldown(cooldown));
                
            vfxActivateCounter.transform.rotation = Quaternion.Euler(0f, 0f,  Mathf.Atan2(counterDirection.y, counterDirection.x) * Mathf.Rad2Deg);
            vfxActivateCounter.SetActive(true);
            vfxCooldownComplete?.SetActive(false);
            vfxCooldownCompleteLoop?.SetActive(false);
            
            if (canCounterPierce)
                Counter(transform.position, counterDirection, DamageArcher, bulletSpeedScale);
            if (canCounterSlash)
                CounterSlash(transform.position, counterDirection, DamageKnight, bulletSpeedScale);
            
            DOTween.Kill(fillFull);
            fillFull.color = new Color(fillFull.color.r, fillFull.color.g, fillFull.color.b, 1f);
            fillFullGlow.color = new Color(fillFullGlow.color.r, fillFullGlow.color.g, fillFullGlow.color.b, 1f);
            SetRingFillFull(true);
            DOTween.Sequence(fillFull)
                .Append(fillFull.DOFade(0f, 0.5f).SetEase(Ease.InQuad))
                .Join(fillFullGlow.DOFade(0f, 0.5f).SetEase(Ease.InQuad))
                .AppendCallback(() => SetRingFillFull(false));
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
            var stagger = LevelUtilityV2.StatsCounterPiercing.stagger;
            var maxHit = 20;
            var size = LevelUtilityV2.GetCounterPiercingSize();
            projectile.transform.position = towerAttackPos;
            projectile.transform.rotation = Quaternion.Euler(0f, 0f,  Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
            projectile.Init(towerAttackPos, direction.normalized, 8, size, speedScale, damage, damage, 0f, stagger, false, maxHit, null, null, ProjectileType.TowerProjectile);
            projectile.BlockDestroy = true;
            projectile.Activate(0f);
        }
        
        public virtual void CounterSlash(Vector2 towerAttackPos, Vector2 direction, int damage, float speedScale)
        {
            var slashRange = LevelUtilityV2.GetCounterSlashRange();
            var hitCount = Physics2D.CircleCastNonAlloc(transform.position, slashRange, direction, slashHits, 0f, hitLayer);
            if (hitCount > 0)
            {
                var halfAngle = LevelUtilityV2.StatsCounterSlash.size / 2;
                for (var i = 0; i < hitCount; i++)
                {
                    var dirTo = (slashHits[i].point - (Vector2)transform.position).normalized;
                    // Check những enemy va chạm, nếu nằm trong góc damageAngle thì mới gây dame
                    if (Vector2.Angle(direction, dirTo) <= halfAngle)
                    {
                        if (slashHits[i].transform.TryGetComponent<EnemyEntity>(out slashCacheEnemy))
                        {
                            slashCacheEnemy.Damage(DamageKnight, transform.position, LevelUtilityV2.StatsCounterSlash.stagger, DamageType.Normal);
                            PassiveEffectManager.Instance.TriggerEffect(PassiveTriggerType.TowerTakeDame, slashCacheEnemy);
                        }
                    }
                }
            }
        }

        private IEnumerator IECooldown(float cooldown)
        {
            counterCooldown = true;
            var cooldownTimer = 0f;
            while (cooldownTimer < cooldown)
            {
                cooldownTimer += Time.deltaTime;
                iconFillMaterial.SetFloat(LinearProgress, cooldownTimer / cooldown);
                yield return null;
            }
            counterCooldown = false;
            DOTween.Kill(visual);
            vfxCooldownComplete?.SetActive(true);
            vfxCooldownCompleteLoop?.SetActive(true);
            visual.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.2f).SetTarget(visual);
            yield return new WaitForSeconds(1f);
            vfxCooldownComplete?.SetActive(false);
        }

        private void TerminateCounter()
        {
            StopCoroutine(coroutineCounter);
            coroutineCounter = null;
            fillMaterial.SetFloat(RadialProgress, 0f);
        }

        private void SetRingFillFull(bool show)
        {
            fillFull.gameObject.SetActive(show);
        }

        private void SetVisual()
        {
            if (canCounterPierce && !canCounterSlash)
            {
                visualBase.sprite = spriteArcherBase;
                visualFill.sprite = spriteArcherFill;
                return;
            }

            if (!canCounterPierce && canCounterSlash)
            {
                visualBase.sprite = spriteKnightBase;
                visualFill.sprite = spriteKnightFill;
                return;
            }

            if (canCounterPierce && canCounterSlash)
            {
                visualBase.sprite = spriteBothBase;
                visualFill.sprite = spriteBothFill;
            }
        }
        
        // Phải tìm cách khác, cách này rất lag
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