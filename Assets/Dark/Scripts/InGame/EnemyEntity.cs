using System;
using System.Collections;
using Dark.Scripts.AudioV2;
using DG.Tweening;
using InGame.EnemyEffect;
using InGame.EnemyVisualBody;
using InGame.MapBoundary;
using UnityEngine;

namespace InGame
{
    public class EnemyEntity : MonoBehaviour, IDamageable, IEffectTarget
    {
        [SerializeField] private Collider2D collider2d;
        [SerializeField] private Transform burnVfxParent;
        public EnemyBody body;

        private MapBoundaryManager boundaryManager;
        public Transform Target { get; set; }
        public TowerEntity TargetTower { get; set; }
        public EnemyBehaviour config;

        [SerializeField] private AudioPlayComponentV2 sfxHit;

        #region Stats
        public int MaxHealth { get; set; }
        protected int CurrentHealth { get; set; }
        private int CurrentDamage { get; set; }
        public int Exp { get; private set; }
        public int Dark { get; private set; }
        public int DarkUnitValue { get; private set; }
        public float DarkRatio { get; private set; }
        public int BossPoint { get; private set; }

        #endregion

        #region Wave and Level config

        public WaveStatsScale StatsScale { get; set; }
        public float LevelExpRatio { get; private set; }
        public float LevelDarkRatio { get; private set; }
        public int LevelDarkUnitValue { get; private set; }

        #endregion

        public bool IsBoss { get; set; }
        public float PercentageHpLeft => (float)CurrentHealth / MaxHealth;
        public Action<int, DamageType> OnHit { get; set; }
        public Action OnStartDead { get; set; }
        public Action<EnemyDieReason> OnDead { get; set; }
        public EnemyState State { get; set; }
        public int UniqueId { get; set; }
        private Vector3 direction = new Vector3();
        private Vector2 directionAddition = new Vector2();
        private float staggerDuration;
        private Vector2 staggerTargetPos;

        [Space, Header("Visual")] 
        [SerializeField] private EnemyBoidAgentWithObstacles boidAgent;
        [SerializeField] private Transform uiHealth;
        public EnemyAnimController animController;
        [SerializeField] protected GameObject shadow;
        
        private bool inAttackRange;
        private Coroutine attackCoroutine;

        protected Vector2 attackPosition;
        
        private float invisibleTimer;
        private float freezeDuration;

        private float delayDieAnimation;
        
        #region Initialize

        private void Awake()
        {
            boundaryManager = MapBoundaryManager.Instance;
        }

        public void Init(EnemyBehaviour eConfig, TowerEntity target, WaveStatsScale statsScale, float levelExpRatio, float levelDarkRatio, int levelDarkUnitValue)
        {
            config = eConfig;
            
            // Set wave and level configs
            StatsScale = statsScale;
            LevelExpRatio = levelExpRatio;
            LevelDarkRatio = levelDarkRatio;
            LevelDarkUnitValue = levelDarkUnitValue;
            
            // Set target and attack position
            Target = target.transform;
            TargetTower = target;

            collider2d.enabled = false;
            
            var myPos = transform.position;
            var targetPos = Target.position;
            attackPosition = ((Quaternion.Euler(0f, 0f, RandomUtil.Range(-75f, 75f)) *
                               (Vector2)(myPos - targetPos).normalized) * (0.9f * config.attackRange)
                              + targetPos);
            animController.transform.localScale =
                new Vector3(Mathf.Sign(attackPosition.x - myPos.x), 1f, 1f);
            
            MaxHealth = (int)(config.hp * StatsScale.hpScale);
            CurrentHealth = MaxHealth;
            CurrentDamage = Mathf.RoundToInt(config.dmg * StatsScale.dmgScale);
            Exp = Mathf.RoundToInt(config.exp * levelExpRatio);
            Dark = Mathf.RoundToInt(config.dark * levelDarkRatio);
            DarkRatio = LevelUtility.GetDropRate(config.darkRatio);
            DarkUnitValue = levelDarkUnitValue;
            BossPoint = config.bossPoint;
            
            State = EnemyState.Spawn;
            inAttackRange = false;
            IsDestroyed = false;
            config.Init(this);
            
            shadow.SetActive(true);
            
            delayDieAnimation = 0f;
            
            ActivateELite(config.elite);
        }

        #endregion

        #region Core function

        private void OnDestroy()
        {
            DOTween.Kill(this);
        }
        
        public virtual void Activate(float delayStartAttack = 0f)
        {
            config.Spawn(this, delayStartAttack, () =>
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
            else if (freezeDuration > 0)
            {
                freezeDuration -= Time.deltaTime;
            }
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
                animController.transform.localScale =
                    new Vector3(Mathf.Sign(Target.position.x - transform.position.x), 1f, 1f);
            }
            else
            {
                config.moveBehaviour.MoveNonAlloc(transform, attackPosition, directionAddition, config.attackRange, config.moveSpeed * StatsScale.speScale, ref direction);
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
            animController.PlayAttack();
            config.attackBehaviour.Attack(this, TargetTower, transform.position, CurrentDamage);
        }

        public float HitDirectionX { get; set; }
        public float HitDirectionY { get; set; }

        public virtual void Damage(int damage, Vector2 dealerPosition, float stagger, DamageType dmgType)
        {
            if (IsDestroyed) return;
            if (State == EnemyState.Invisible) return;
            
            CurrentHealth -= damage;
            
            OnHit?.Invoke(damage, dmgType);
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
                freezeDuration = Mathf.Clamp(0.3f, 0.6f, 0.6f * (stagger - config.staggerResist));
            }

            animController.PlayHit();
            // invisibleTimer = config.invisibleDuration;
            invisibleTimer = 0f;
            State = EnemyState.Invisible;
            
            if (CurrentHealth <= 0)
            {
                var dieReason = EnemyDieReason.PlayerKill;
                switch (dmgType)
                {
                    case DamageType.Normal:
                    case DamageType.NormalCritical:
                        dieReason = EnemyDieReason.PlayerKill;
                        break;
                    case DamageType.Tower:
                    case DamageType.TowerCritical:
                        dieReason = EnemyDieReason.TowerKill;
                        break;
                    case DamageType.SelfDestruct:
                        dieReason = EnemyDieReason.Suicide;
                        break;
                }
                OnDie(dieReason);
                sfxHit.Play();
            }
        }

        public bool IsDestroyed { get; set; }

        private void OnDie(EnemyDieReason reason)
        {
            if (attackCoroutine != null)
                StopCoroutine(attackCoroutine);
            
            // reset stagger
            staggerDuration = 0f;
            freezeDuration = 0f;
            invisibleTimer = 0f;
            
            collider2d.enabled = false;
            IsDestroyed = true;
            boidAgent.IsActive = false;
            if (coroutineBurn != null) StopCoroutine(coroutineBurn);
            callbackBurnComplete?.Invoke();
            callbackBurnComplete = null;
            if (reason != EnemyDieReason.Suicide)
                DropResource();
            StartCoroutine(IEDie(.5f, reason));
        }

        protected virtual IEnumerator IEDie(float delayRelease, EnemyDieReason reason)
        {
            yield return new WaitForEndOfFrame();
            // Đợi chạy xong anim hit rồi mới chạy anim die
            shadow.SetActive(false);    
            OnStartDead?.Invoke();
            OnStartDead = null;
            yield return new WaitForSeconds(delayDieAnimation);
            yield return new WaitForSeconds(animController.PlayDie());
            OnDead?.Invoke(reason);
            OnDead = null;
            yield return new WaitForSeconds(delayRelease);
            EnemyPool.Instance.Release(this, config.enemyId);
        }

        protected virtual void DropResource()
        {
            var dropVestige = RandomUtil.Range(0f, 1f) <= DarkRatio && Dark > 0;
            CombatActions.OnDropResource?.Invoke(this, dropVestige);
        }

        #region Effect 

        public Transform TargetTransform => transform;
        private Action callbackBurnComplete;
        private Coroutine coroutineBurn;
        public void Burn(float duration, float delayEachBurn, int damage, Action callbackComplete)
        {
            if (IsDestroyed)
            {
                callbackComplete?.Invoke();
                return;
            }
            callbackBurnComplete = callbackComplete;
            if (coroutineBurn != null) StopCoroutine(coroutineBurn);
            coroutineBurn = StartCoroutine(IEBurn(duration, delayEachBurn, damage));
        }

        public Transform BurnVfxParent => burnVfxParent;

        private IEnumerator IEBurn(float duration, float delayEachBurn, int damage)
        {
            var totalBurn = (int)(duration / delayEachBurn);

            while (totalBurn > 0)
            {
                if (State == EnemyState.Freeze) yield return null;
                yield return new WaitForSeconds(delayEachBurn);
                HitDirectionX = 0f;
                HitDirectionY = 0f;
                Damage(damage, transform.position, 0f, DamageType.Normal);
                totalBurn -= 1;
            }
            
            callbackBurnComplete?.Invoke();
            callbackBurnComplete = null;
        }

        public virtual void Kill(DamageType dmgType, float delayAnimation = 0f)
        {
            delayDieAnimation = delayAnimation;
            HitDirectionX = 0f;
            HitDirectionY = 0f;
            Damage(CurrentHealth, transform.position, 0f, dmgType);
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