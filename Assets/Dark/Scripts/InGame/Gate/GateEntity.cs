using System;
using System.Collections;
using System.Linq;
using Dark.Scripts.Utils;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace InGame
{
    public class GateEntity : SerializedMonoBehaviour
    {
        [SerializeField] private AnimationCurve orbYCurve;
        [SerializeField] private float orbSpawnDuration;
        [SerializeField] private EnemyBoidObstacle obstacle;
        
        [ReadOnly] public TowerEntity[] target;
        private WaveStatsScale StatsScale { get; set; }
        private float LevelExpRatio { get; set; }
        private float LevelDarkRatio { get; set; }
        private int LevelDarkUnitValue { get; set; }
        public bool IsActive { get; set; } = false;
        public bool AllEnemyDead { get; set; }
        private int TotalSpawnTurn { get; set; } // unlimited = -1
        private int currentSpawnTurn = 0;
        private int AliveEnemyCount { get; set; }
        
        #region Gate config

        [Space] [Header("Gate config")] 
        [NonSerialized, OdinSerialize, ReadOnly] public GateConfig config;

        [Space] [Header("Visual")] 
        [SerializeField] private GameObject visual;
        [SerializeField] private GameObject vfxOpen;
        [SerializeField] private ParticleSystem vfxPortal;
        [SerializeField] private GameObject vfxClose;
        [SerializeField] private float vfxAppearDuration = 0.5f; // duration of vfxOpen
        [SerializeField] private float vfxCloseDuration = 6f; // duration of vfxClose
        [SerializeField] private float visualRadius = 2f;

        private ParticleSystem.MainModule vfxIdle;
        
        #endregion

        public Action OnAllEnemiesDead { get; set; }

        private float orbSpawnTimer;
        
        public void Activate()
        {
            IsActive = true;
            obstacle.gameObject.SetActive(true);
            delayCoroutine = StartCoroutine(IEStartSpawn(config.startTime));
        }

        public void Deactivate()
        {
            LevelManager.Instance.OnWin -= Deactivate;
            
            if (spawnCoroutine != null)
                StopCoroutine(spawnCoroutine);

            if (visualCoroutine != null)
            {
                StopCoroutine(visualCoroutine);
                ForceCloseGate();
            }

            IsActive = false;
            obstacle.gameObject.SetActive(false);
            this.DelayCall(vfxCloseDuration, () => gameObject.SetActive(false));
        }
        
        public void Deactivate(bool hideVisual)
        {
            LevelManager.Instance.OnWin -= Deactivate;
            
            if (spawnCoroutine != null)
                StopCoroutine(spawnCoroutine);

            if (hideVisual && visualCoroutine != null)
            {
                StopCoroutine(visualCoroutine);
                ForceCloseGate();
            }

            IsActive = false;
            obstacle.gameObject.SetActive(false);
        }
        
        public void Initialize(GateConfig cfg, TowerEntity[] targetBase, WaveStatsScale statsScale, float levelExpRatio, float levelDarkRatio, int levelDarkUnitValue)
        {
            config = cfg;
            target = targetBase;
            StatsScale = statsScale;
            LevelExpRatio = levelExpRatio;
            LevelDarkRatio = levelDarkRatio;
            LevelDarkUnitValue = levelDarkUnitValue;
            TotalSpawnTurn = cfg.duration >= 0 ? (int)(cfg.duration / cfg.intervalLoop) + 1 : -1;
            currentSpawnTurn = 0;
            AliveEnemyCount = 0;
            IsActive = false;
            AllEnemyDead = false;

            if (spawnCoroutine != null)
                StopCoroutine(spawnCoroutine);
            
            if (visualCoroutine != null)
                StopCoroutine(visualCoroutine);
            
            visual.SetActive(false);
            vfxOpen.SetActive(false);
            vfxClose.SetActive(false);
            vfxPortal.gameObject.SetActive(false);
            orbSpawnTimer = 0f;
            
            LevelManager.Instance.OnWin += Deactivate;
            LevelManager.Instance.OnLose += OnLose;
        }

        private Coroutine delayCoroutine;
        private Coroutine spawnCoroutine;
        private Coroutine visualCoroutine;
        [NonSerialized] public Action onActivated;
        private IEnumerator IEStartSpawn(float delay)
        {
            yield return new WaitForSeconds(delay);
            onActivated?.Invoke();
            onActivated = null;
            delayCoroutine = null;
            spawnCoroutine = StartCoroutine(IESpawn());
            visualCoroutine = StartCoroutine(IEVisual());
        }
        
        private IEnumerator IESpawn()
        {
            while (TotalSpawnTurn == -1 || currentSpawnTurn < TotalSpawnTurn)
            {
                var enemies = config.spawnLogic.Spawn(transform.position, config.spawnType.enemyId, config.spawnType.enemyPrefab, target);
                if (config.isBossGate)
                {
                    foreach (var enemy in enemies)
                    {
                        enemy.Item1.IsBoss = true;
                        LevelManager.Instance.LevelBossName = config.spawnType.displayName;
                    }
                    
                }
                else
                {
                    foreach (var enemy in enemies)
                    {
                        enemy.Item1.IsBoss = false;
                    }
                }
                
                // Không phải boss thì spawn orb
                if (!config.isBossGate)
                {
                    var orbs = new Transform[enemies.Length];

                    for (var i = 0; i < enemies.Length; i++)
                    {
                        orbs[i] = EnemyOrbPool.Instance.Get(null, false);
                        orbs[i].position = transform.position;
                        orbs[i].gameObject.SetActive(true);
                    }

                    while (orbSpawnTimer < orbSpawnDuration)
                    {
                        orbSpawnTimer += Time.deltaTime;
                        
                        for (var i = 0; i < orbs.Length; i++)
                        {
                            var t = Mathf.Clamp01(orbSpawnTimer / orbSpawnDuration);

                            // horizontal position (isometric: usually XZ plane)
                            var horizontalPos = Vector3.Lerp(transform.position, enemies[i].Item1.transform.position, t);

                            // height offset using curve
                            var curveY = orbYCurve.Evaluate(t) * 3f;

                            // final position
                            horizontalPos.y += curveY;

                            orbs[i].position = horizontalPos;
                        }

                        yield return new WaitForEndOfFrame();
                    }

                    foreach (var orb in orbs)
                    {
                        EnemyOrbPool.Instance.Release(orb);
                    }
                }
                
                for (var i = 0; i < enemies.Length; i++)
                {
                    var enemy = enemies[i];
                    enemy.Item1.Init(config.spawnType, enemy.Item2, StatsScale, LevelExpRatio, LevelDarkRatio, LevelDarkUnitValue);
                    enemy.Item1.Activate();
                    enemy.Item1.UniqueId = EnemyManager.Instance.CurrentEnemyIndex;
                    AliveEnemyCount += 1;
                    EnemyManager.Instance.OnEnemySpawn(enemy.Item1);
                    enemy.Item1.OnDead += (reason) =>
                    {
                        AliveEnemyCount -= 1;
                        EnemyManager.Instance.OnEnemyDead(enemy.Item1, reason);
                        CheckAllEnemiesDead();
                    };
                }

                currentSpawnTurn += 1;
                orbSpawnTimer = 0f;
                
                if (TotalSpawnTurn == -1 || currentSpawnTurn < TotalSpawnTurn)
                    yield return new WaitForSeconds(config.intervalLoop);
            }
            
            CheckAllEnemiesDead();
            
            Deactivate(false);
        }
        
        private void CheckAllEnemiesDead()
        {
            if (IsActive || currentSpawnTurn < TotalSpawnTurn) return;
            if (AliveEnemyCount == 0)
            {
                AllEnemyDead = true;
                OnAllEnemiesDead?.Invoke();
                OnAllEnemiesDead = null;
            }
        }

        private IEnumerator IEVisual()
        {
            yield return new WaitForSeconds(Mathf.Max(config.startTimeVisual - vfxAppearDuration, 0f));
            visual.SetActive(true);

            vfxOpen.SetActive(true);
            yield return new WaitForSeconds(vfxAppearDuration);
            
            vfxIdle = vfxPortal.main;
            vfxIdle.startLifetime = config.durationVisual;
            vfxIdle.duration = config.durationVisual;
            vfxPortal.gameObject.SetActive(true);
            yield return new WaitForEndOfFrame();
            vfxOpen.SetActive(false);

            yield return new WaitForSeconds(config.durationVisual);
            
            vfxClose.SetActive(true);
            yield return new WaitForEndOfFrame();
            vfxPortal.gameObject.SetActive(false);
            yield return new WaitForSeconds(vfxCloseDuration);

            vfxClose.SetActive(false);
        }

        private void ForceCloseGate()
        {
            vfxOpen.SetActive(false);
            vfxClose.SetActive(false);
            vfxPortal.gameObject.SetActive(false);
        }

        /// <summary>
        /// Restart lại gate nhưng giảm start time
        /// </summary>
        /// <param name="reduceStartTime"></param>
        public void ForceRestartGate(float reduceStartTime)
        {
            if (AllEnemyDead) return;
            if (delayCoroutine != null)
            {
                StopCoroutine(delayCoroutine);
                delayCoroutine = null;
                if (reduceStartTime < config.startTime)
                {
                    delayCoroutine = StartCoroutine(IEStartSpawn(config.startTime - reduceStartTime));
                }
                else
                {
                    spawnCoroutine = StartCoroutine(IESpawn());
                    visualCoroutine = StartCoroutine(IEVisual());
                }
            }
        }

        private void OnLose()
        {
            Deactivate();
            gameObject.SetActive(false);
        }

        public bool IsInGate(Vector2 position)
        {
            return Vector2.Distance(position, transform.position) < visualRadius;
        }
    }
}