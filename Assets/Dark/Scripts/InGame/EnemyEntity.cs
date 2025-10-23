using System;
using System.Collections;
using Dark.Scripts.Audio;
using DG.Tweening;
using Economic;
using InGame.EnemyEffect;
using InGame.MapBoundary;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace InGame
{
    public class EnemyEntity : MonoBehaviour, IDamageable, IEffectTarget
    {
        [SerializeField] private Collider2D collider2d;

        private MapBoundaryManager boundaryManager;
        public Transform Target { get; set; }
        public TowerEntity TargetTower { get; set; }
        private EnemyBehaviour config;

        [SerializeField] private AudioComponent sfxHit;

        #region Stats
        public int MaxHealth { get; set; }
        private int CurrentHealth { get; set; }
        private int CurrentDamage { get; set; }
        public int Exp { get; private set; }
        public int Dark { get; private set; }
        public float DarkRatio { get; private set; }
        public int BossPoint { get; private set; }

        #endregion

        public float PercentageHpLeft => CurrentHealth / MaxHealth * 100f;
        public Action<int> OnHit { get; set; }
        public Action OnDead { get; set; }
        public EnemyState State { get; set; }
        public int UniqueId { get; set; }
        private Vector3 direction = new Vector3();
        private Vector2 directionAddition = new Vector2();
        private float staggerDuration;
        private Vector2 staggerTargetPos;

        [Space, Header("Visual")] 
        [SerializeField] private EnemyBoidAgent boidAgent;
        [SerializeField] private Transform uiHealth;
        public EnemyAnimController animController;
        [SerializeField] private GameObject shadow;
        
        private bool inAttackRange;
        private Coroutine attackCoroutine;

        private Vector2 attackPosition;
        
        private float invisibleTimer;
        private float freezeDuration;
        
        #region Initialize

        private void Awake()
        {
            boundaryManager = MapBoundaryManager.Instance;
        }

        public void Init(EnemyBehaviour eConfig, TowerEntity target, float hpMultiplier, float dmgMultiplier, float levelExpRatio, float levelDarkRatio)
        {
            config = eConfig;
            
            // Set target and attack position
            Target = target.transform;
            TargetTower = target;

            collider2d.enabled = false;
            
            var myPos = transform.position;
            var targetPos = Target.position;
            attackPosition = ((Quaternion.Euler(0f, 0f, Random.Range(-75f, 75f)) *
                               (Vector2)(myPos - targetPos).normalized) * (0.9f * config.attackRange)
                              + targetPos);
            animController.transform.localScale =
                new Vector3(Mathf.Sign(attackPosition.x - myPos.x), 1f, 1f);
            
            MaxHealth = (int)(config.hp * hpMultiplier);
            CurrentHealth = MaxHealth;
            CurrentDamage = Mathf.RoundToInt(config.dmg * dmgMultiplier);
            Exp = Mathf.RoundToInt(config.exp * levelExpRatio);
            Dark = Mathf.RoundToInt(config.dark * levelDarkRatio);
            DarkRatio = LevelUtility.GetDropRate(config.darkRatio);
            BossPoint = config.bossPoint;
            
            State = EnemyState.Spawn;
            inAttackRange = false;
            IsDestroyed = false;
            config.Init(this);
            
            shadow.SetActive(true);
            
            ActivateELite(config.elite);
        }

        #endregion

        #region Core function

        private void OnDestroy()
        {
            DOTween.Kill(this);
        }
        
        public virtual void Activate()
        {
            config.Spawn(this, () =>
            {
                StartAttackCoroutine();
                State = EnemyState.Move;
                animController.PlayRun();
                boidAgent.IsActive = true;
                collider2d.enabled = true;
            });
        }

        protected virtual void Update()
        {
            if (!Target) return;
            if (IsDestroyed) return;
            if (State == EnemyState.Freeze) return;
            if (State == EnemyState.Spawn) return;
            if (State == EnemyState.Invisible)
            {
                invisibleTimer -= Time.deltaTime;
                if (invisibleTimer <= 0) State = EnemyState.Move;
            }

            if (staggerDuration > 0)
            {
                staggerDuration -= Time.deltaTime;
                // transform.position = Vector2.MoveTowards(transform.position, (Vector2)transform.position + staggerDirection, 5f * Time.deltaTime);
                transform.position = Vector2.Lerp(transform.position, staggerTargetPos, staggerDuration);
            }
            // else if (freezeDuration > 0)
            // {
            //     freezeDuration -= Time.deltaTime;
            // }
            else
            {
                if (boundaryManager.ContainPoint(transform.position))
                {
                    directionAddition.x = 0;
                    directionAddition.y = 0;
                }
                else
                {
                    boidAgent.GetBoidAdditionNonAlloc(ref directionAddition);
                }
                MoveTo(Target);
            }
        }
        
        #endregion
        
        private void MoveTo(Transform target)
        {
            if (Vector3.Distance(transform.position, target.position) < config.attackRange)
            {
                inAttackRange = true;
                animController.SetDefaultRun(false);
            }
            else
            {
                config.moveBehaviour.MoveNonAlloc(transform, attackPosition, directionAddition, config.attackRange, config.moveSpeed, ref direction);
                animController.SetDefaultRun(true);
            }
        }

        public void Stop()
        {
            State = EnemyState.Freeze;
            if (attackCoroutine != null)
                StopCoroutine(attackCoroutine);
        }
        
        private void StartAttackCoroutine()
        {
            if (attackCoroutine != null)
                StopCoroutine(attackCoroutine);

            attackCoroutine = StartCoroutine(IEAttack());
        }

        protected virtual IEnumerator IEAttack()
        {
            while (true)
            {
                if (inAttackRange)
                {
                    Attack();
                    yield return new WaitForSeconds(1 / config.attackSpeed);
                }
                else
                    yield return new WaitUntil(() => inAttackRange);
            }
        }

        private void Attack()
        {
            if (TargetTower.IsDestroyed) return;
            config.attackBehaviour.Attack(TargetTower, transform.position, CurrentDamage);
            animController.PlayAttack();
        }

        public float HitDirectionX { get; set; }
        public float HitDirectionY { get; set; }

        public void Damage(int damage, Vector2 dealerPosition, float stagger)
        {
            if (IsDestroyed) return;
            if (State == EnemyState.Invisible) return;
            
            CurrentHealth -= damage;
            
            
            OnHit?.Invoke(damage);
            if (CurrentHealth <= 0)
            {
                OnDie();
                sfxHit.Play();
            }
            else
            {
                if (stagger - config.staggerResist > 0)
                {
                    var mag = Mathf.Sqrt(HitDirectionX * HitDirectionX + HitDirectionY * HitDirectionY);
                    if (mag == 0)
                    {
                        staggerTargetPos.x = transform.position.x;
                        staggerTargetPos.y = transform.position.y;
                    }
                    else
                    {
                        staggerTargetPos.x = (stagger - config.staggerResist) * HitDirectionX / mag + transform.position.x;
                        staggerTargetPos.y = (stagger - config.staggerResist) * HitDirectionY / mag + transform.position.y;
                    }
                    
                    staggerDuration = Mathf.Abs(stagger - config.staggerResist) / config.staggerVelocity;
                    freezeDuration = staggerDuration;
                }

                animController.PlayHit();
                invisibleTimer = config.invisibleDuration;
                State = EnemyState.Invisible;
            }
        }

        public bool IsDestroyed { get; set; }

        private void OnDie()
        {
            if (attackCoroutine != null)
                StopCoroutine(attackCoroutine);
            
            // reset stagger
            staggerDuration = 0f;
            freezeDuration = 0f;
            invisibleTimer = 0f;
            
            collider2d.enabled = false;
            IsDestroyed = true;
            OnDead?.Invoke();
            OnDead = null;
            boidAgent.IsActive = false;
            callbackBurnComplete?.Invoke();
            callbackBurnComplete = null;
            StartCoroutine(IEDie(
                animController.PlayDie(), 0.5f
                ));
        }

        private IEnumerator IEDie(float delayAnim, float delayRelease)
        {
            shadow.SetActive(false);    
            yield return new WaitForSeconds(delayAnim);
            yield return new WaitForSeconds(delayRelease);
            EnemyPool.Instance.Release(this, config.enemyId);
            
        }

        #region Effect 

        public Transform TargetTransform => transform;
        private Action callbackBurnComplete;
        public void Burn(float duration, float delayEachBurn, int damage, Action callbackComplete)
        {
            callbackBurnComplete = callbackComplete;
            StartCoroutine(IEBurn(duration, delayEachBurn, damage));
        }

        private IEnumerator IEBurn(float duration, float delayEachBurn, int damage)
        {
            var totalBurn = (int)(duration / delayEachBurn);

            while (totalBurn > 0)
            {
                yield return new WaitForSeconds(delayEachBurn);
                HitDirectionX = 0f;
                HitDirectionY = 0f;
                Damage(damage, transform.position, 0f);
                totalBurn -= 1;
            }
            
            callbackBurnComplete?.Invoke();
            callbackBurnComplete = null;
        }

        public void Kill()
        {
            HitDirectionX = 0f;
            HitDirectionY = 0f;
            Damage(CurrentHealth, transform.position, 0f);
        }
        #endregion

        #region Elite

        [Space] [Header("Elite")] 
        [SerializeField] private SpriteRenderer visual;
        [SerializeField] private Material materialNormal;
        [SerializeField] private Material materialElite;
        
        public void ActivateELite(bool active)
        {
            if (active)
            {
                visual.material = materialElite;
                transform.localScale = GameConst.EnemyEliteScale * Vector3.one;
            }
            else
            {
                visual.material = materialNormal;
                transform.localScale = Vector3.one;
            }
        }

        #endregion
    }
}