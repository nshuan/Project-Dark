using System;
using System.Collections;
using Dark.Scripts.AudioV2;
using Dark.Scripts.Settings;
using Dark.Scripts.Utils;
using DG.Tweening;
using InGame.EnemyEffect;
using InGame.EnemyVisualBody;
using InGame.MapBoundary;
using UnityEngine;

namespace InGame
{
    public class EnemyEntity : MonoBehaviour, IDamageable, IEffectTarget
    {
        [SerializeField] protected Collider2D collider2d;
        [SerializeField] protected EnemyHealthBar healthBar;
        [SerializeField] protected EnemyDisplayStats displayStats;
        [SerializeField] private Transform burnVfxParent;
        [SerializeField] protected Transform visualAttackRange;
        [SerializeField] protected bool showAttackRange;
        public EnemyBody body;

        private MapBoundaryManager boundaryManager;
        public Transform Target { get; set; }
        public TowerEntity TargetTower { get; set; }
        public EnemyBehaviour config;

        [SerializeField] private AudioPlayComponentV2 sfxHit;

        #region Stats
        public int MaxHealth { get; set; }
        protected int CurrentHealth { get; set; }
        public int CurrentDamage { get; set; }
        public int Exp { get; private set; }
        public int Dark { get; private set; }
        public int DarkUnitValue { get; private set; }
        public float DarkRatio { get; private set; }
        public int BossPoint { get; protected set; }
        public float AttackRange { get; set; }
        public float TempSpeedScale { get; set; } = 1f;
        public float TempDmgScale { get; set; } = 1f;

        #endregion

        #region Wave and Level config

        public WaveStatsScale StatsScale { get; set; }
        public float LevelExpRatio { get; private set; }
        public float LevelDarkRatio { get; private set; }
        public int LevelDarkUnitValue { get; private set; }

        #endregion

        public bool IsBoss { get; set; }
        public float PercentageHpLeft => (float)CurrentHealth / MaxHealth;
        public Action OnInit { get; set; }
        public Action OnSpawn { get; set; }
        public Action<int, DamageType> OnHit { get; set; }
        public Action<EnemyEntity> OnStartDead { get; set; }
        public Action<EnemyEntity, EnemyDieReason> OnDead { get; set; }
        public EnemyState State { get; set; }
        public bool Activated { get; set; }
        public int UniqueId { get; set; }
        protected Vector3 direction = new Vector3();
        private Vector2 directionAddition = new Vector2();
        private float staggerDuration;
        private Vector2 staggerTargetPos;

        [Space, Header("Visual")] 
        [SerializeField] protected EnemyBoidAgentWithObstacles boidAgent;
        [SerializeField] private Transform uiHealth;
        public EnemyAnimController animController;
        [SerializeField] protected GameObject shadow;
        [SerializeField] protected GameObject aimPointer;
        [SerializeField] protected GameObject hoverPointer;
        
        protected bool inAttackRange;
        private Coroutine attackCoroutine;

        public Vector2 attackPosition;
        protected bool reachAttackPosition;
        
        private float invisibleTimer;
        private float freezeDuration;

        private float delayDieAnimation;
        
        #region Initialize

        private void Awake()
        {
            boundaryManager = MapBoundaryManager.Instance;
        }

        public virtual void Init(EnemyBehaviour eConfig, TowerEntity target, WaveStatsScale statsScale, float levelExpRatio, float levelDarkRatio, int levelDarkUnitValue)
        {
            config = eConfig;
            
            // Set wave and level configs
            StatsScale = statsScale;
            LevelExpRatio = levelExpRatio;
            LevelDarkRatio = levelDarkRatio;
            LevelDarkUnitValue = levelDarkUnitValue;

            TempDmgScale = 1f;
            TempSpeedScale = 1f;
            
            // Set target and attack position
            Target = target.transform;
            TargetTower = target;

            collider2d.enabled = false;
            
            var myPos = transform.position;
            var targetPos = Target.position;
            attackPosition = ((Quaternion.Euler(0f, 0f, RandomUtil.Range(-75f, 75f)) *
                               (Vector2)(myPos - targetPos).normalized) * (0.9f * AttackRange)
                              + targetPos);
            reachAttackPosition = false;
            animController.transform.localScale =
                new Vector3(Mathf.Sign(attackPosition.x - myPos.x), 1f, 1f);
            healthBar.transform.localScale = new Vector3(animController.transform.localScale.x,
                healthBar.transform.localScale.y, healthBar.transform.localScale.z);
            
            MaxHealth = (int)(config.hp * StatsScale.hpScale);
            CurrentHealth = MaxHealth;
            healthBar.gameObject.SetActive(false);
            healthBar.MaxHp = MaxHealth;
            healthBar.UpdateHp(CurrentHealth);
            CurrentDamage = Mathf.RoundToInt(config.dmg * StatsScale.dmgScale);
            Exp = Mathf.RoundToInt(config.exp * levelExpRatio);
            Dark = Mathf.RoundToInt(config.dark * levelDarkRatio);
            if (!IsBoss)
            {
                Exp = Mathf.RoundToInt(Exp * LevelUtilityV2.GetExpDropScale());
                Dark = Mathf.RoundToInt(Dark * LevelUtilityV2.GetVestigeDropScale());
                if (RandomUtil.Range(0f, 1f) < LevelUtilityV2.GetVestigeDoubleChance()) Dark *= 2;
                if (RandomUtil.Range(0f, 1f) < LevelUtilityV2.GetVestigeTripleChance()) Dark *= 3;
            }
            
            DarkRatio = 1f; // Chắc chắn rớt
            DarkUnitValue = levelDarkUnitValue;
            BossPoint = config.bossPoint;
            AttackRange = config.attackRange;
            
            displayStats.gameObject.SetActive(false);
            if (GameConst.ShowTextEnemyAtkAndAtkRange)
            {
                displayStats.UpdateStats(LevelUtilityV2.ToInt(CurrentDamage * TempDmgScale), AttackRange);
                displayStats.transform.localScale = new Vector3(animController.transform.localScale.x,
                    displayStats.transform.localScale.y, displayStats.transform.localScale.z);
            }

            visualAttackRange.gameObject.SetActive(false);
            
            State = EnemyState.Spawn;
            inAttackRange = false;
            IsDestroyed = false;
            config.Init(this);
            
            shadow.SetActive(true);
            SetAimed(false);
            
            delayDieAnimation = 0f;

            Activated = false;
            
            ActivateELite(config.elite);
        }

        #endregion

        #region Core function

        protected virtual void OnDestroy()
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
                Activated = true;
                healthBar.gameObject.SetActive(!IsBoss ? GameSettings.ShowEnemyHealth : GameSettings.ShowBossHealth);
                if (showAttackRange)
                {
                    visualAttackRange.localScale = AttackRange * Vector3.one;
                    visualAttackRange.gameObject.SetActive(true);
                }
                
                OnSpawn?.Invoke();
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
            if (Vector3.Distance(transform.position, target.position) < AttackRange)
            {
                inAttackRange = true;
                animController.SetDefaultRun(false);
                animController.transform.localScale =
                    new Vector3(Mathf.Sign(Target.position.x - transform.position.x), 1f, 1f);
                healthBar.transform.localScale = new Vector3(animController.transform.localScale.x,
                    healthBar.transform.localScale.y, healthBar.transform.localScale.z);
                displayStats.transform.localScale = new Vector3(animController.transform.localScale.x,
                    displayStats.transform.localScale.y, displayStats.transform.localScale.z);
            }
            else
            {
                inAttackRange = false;
                if (!reachAttackPosition)
                {
                    if (Vector2.Distance(transform.position, attackPosition) < 0.1f)
                    {
                        reachAttackPosition = true;
                        attackPosition = Target.position;
                    }
                }
                
                config.moveBehaviour.MoveNonAlloc(transform, attackPosition, directionAddition, AttackRange, config.moveSpeed * StatsScale.speScale * TempSpeedScale, ref direction);
                animController.SetDefaultRun(true);
            }
        }

        public void Stop()
        {
            State = EnemyState.Freeze;
            if (attackCoroutine != null)
                StopCoroutine(attackCoroutine);
        }
        
        protected void StartAttackCoroutine()
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

        protected virtual void Attack()
        {
            if (TargetTower.IsDestroyed) return;
            animController.PlayAttack();
            this.DelayCall(animController.GetAttackDelayTrigger(), () =>
            {
                if (TargetTower.IsDestroyed) return;
                config.attackBehaviour.Attack(this, TargetTower, transform.position, LevelUtilityV2.ToInt(CurrentDamage * TempDmgScale));
            });
        }

        public float HitDirectionX { get; set; }
        public float HitDirectionY { get; set; }

        public virtual void Damage(int damage, Vector2 dealerPosition, float stagger, DamageType dmgType, bool instantKill = false)
        {
            if (!Activated) return;
            if (IsDestroyed) return;
            if (dmgType != DamageType.Enemy && dmgType != DamageType.SelfDestruct && State == EnemyState.Invisible && instantKill == false) return;
            
            // Scale damage on boss
            if (IsBoss)
                damage = LevelUtilityV2.ToInt(LevelUtilityV2.GetBossScaleDamage() * damage);
            
            var lastHealth = CurrentHealth;
            CurrentHealth -= damage;
            healthBar.UpdateHp(CurrentHealth);
            if (dmgType != DamageType.Enemy && dmgType != DamageType.SelfDestruct)
                CombatActions.OnDamageDealt?.Invoke(lastHealth - CurrentHealth);

            if (instantKill)
            {
                OnDie(EnemyDieReason.PlayerKill);
                return;
            }
            
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
                    case DamageType.Enemy:
                        dieReason = EnemyDieReason.EnemyKill;
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
            if (IsDestroyed) return;
            
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
            SetAimed(false);
            if (reason != EnemyDieReason.Suicide && reason != EnemyDieReason.EnemyKill)
                DropResource();
            StartCoroutine(IEDie(.5f, reason));
        }

        protected virtual IEnumerator IEDie(float delayRelease, EnemyDieReason reason)
        {
            yield return new WaitForEndOfFrame();
            // Đợi chạy xong anim hit rồi mới chạy anim die
            shadow.SetActive(false);    
            OnStartDead?.Invoke(this);
            OnStartDead = null;
            if (showAttackRange)
            {
                visualAttackRange.gameObject.SetActive(false);
            }
            yield return new WaitForSeconds(delayDieAnimation);
            yield return new WaitForSeconds(animController.PlayDie());
            OnDead?.Invoke(this, reason);
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
        private bool isBurning;
        public void Burn(float duration, float delayEachBurn, int damage, Action callbackComplete)
        {
            if (IsDestroyed || isBurning)
            {
                callbackComplete?.Invoke();
                return;
            }

            isBurning = true;
            callbackBurnComplete = callbackComplete;
            callbackBurnComplete += () => { isBurning = false; };
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
            Damage(CurrentHealth, transform.position, 0f, dmgType, true);
        }

        public bool IsEffectTargetDead => IsDestroyed;

        #endregion

        #region Elite

        [Space] [Header("Elite")] 
        [SerializeField] private SpriteRenderer visual;
        [SerializeField] private Material materialNormal;
        [SerializeField] private Material materialElite;

        private Material cacheMaterial;
        
        public virtual void ActivateELite(bool active)
        {
            if (active)
            {
                visual.material = materialElite;
                cacheMaterial = materialElite;
                transform.localScale = (IsBoss ? GameConst.BossEliteScale : GameConst.EnemyEliteScale) * Vector3.one;
                healthBar.transform.localScale = new Vector3(healthBar.transform.localScale.x, 1f / (IsBoss ? GameConst.BossEliteScale : GameConst.EnemyEliteScale), healthBar.transform.localScale.z);
            }
            else
            {
                visual.material = materialNormal;
                cacheMaterial = materialNormal;
                transform.localScale = Vector3.one;
                healthBar.transform.localScale = new Vector3(healthBar.transform.localScale.x, 1f, 1f);
            }
        }

        #endregion

        #region Highlight

        [Space] [Header("Highlight")] 
        [SerializeField] private Material materialHighlight;
        
        public void SetAimed(bool aimed)
        {
            aimPointer.SetActive(aimed);
            if (IsBoss) return;
            
            // Boss ko show stats
            if (GameConst.ShowTextEnemyAtkAndAtkRange)
                displayStats.gameObject.SetActive(aimed);
            
            if (config.elite) return;
            visual.material = aimed ? materialHighlight : cacheMaterial;
        }

        public void SetHover(bool hover)
        {
            if (aimPointer.activeInHierarchy)
            {
                hoverPointer.SetActive(false);
                return;
            }
            hoverPointer.SetActive(hover);
            if (IsBoss) return;
            
            // Boss ko show stats
            if (GameConst.ShowTextEnemyAtkAndAtkRange)
                displayStats.gameObject.SetActive(hover);
            
            if (config.elite) return;
            visual.material = hover ? materialHighlight : cacheMaterial;
        }

        #endregion
    }
}