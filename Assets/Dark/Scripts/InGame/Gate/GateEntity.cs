using System;
using System.Collections;
using System.Linq;
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
        
        [ReadOnly] public TowerEntity[] target;
        private float WaveHpMultiplier { get; set; }
        private float WaveDmgMultiplier { get; set; }
        private float LevelExpRatio { get; set; }
        private float LevelDarkRatio { get; set; }
        public bool IsActive { get; set; } = false;
        public bool AllEnemyDead { get; set; }
        private int TotalSpawnTurn { get; set; } // unlimited = -1
        private int currentSpawnTurn = 0;
        private int AliveEnemyCount { get; set; }
        
        #region Gate config

        [Space] [Header("Gate config")] 
        [NonSerialized, OdinSerialize, ReadOnly] private GateConfig config;

        [Space] [Header("Visual")] 
        [SerializeField] private GameObject visual;

        #endregion

        public Action OnAllEnemiesDead { get; set; }

        private float orbSpawnTimer;
        
        public void Activate()
        {
            IsActive = true;
            spawnCoroutine = StartCoroutine(IESpawn());
        }

        public void Deactivate()
        {
            if (spawnCoroutine != null)
                StopCoroutine(spawnCoroutine);

            IsActive = false;
            gameObject.SetActive(false);
        }
        
        public void Initialize(GateConfig cfg, TowerEntity[] targetBase, float waveHpMultiplier, float waveDmgMultiplier, float levelExpRatio, float levelDarkRatio)
        {
            config = cfg;
            target = targetBase;
            WaveHpMultiplier = waveHpMultiplier;
            WaveDmgMultiplier = waveDmgMultiplier;
            LevelExpRatio = levelExpRatio;
            LevelDarkRatio = levelDarkRatio;
            TotalSpawnTurn = cfg.duration >= 0 ? (int)(cfg.duration / cfg.intervalLoop) : -1;
            currentSpawnTurn = 0;
            AliveEnemyCount = 0;
            IsActive = false;
            AllEnemyDead = false;

            if (spawnCoroutine != null)
                StopCoroutine(spawnCoroutine);
            
            visual.SetActive(false);
            orbSpawnTimer = 0f;
        }
        
        private Coroutine spawnCoroutine;
        private IEnumerator IESpawn()
        {
            yield return new WaitForSeconds(config.startTime);
            
            visual.SetActive(config.showVisual);
            
            while (TotalSpawnTurn == -1 || currentSpawnTurn < TotalSpawnTurn)
            {
                var enemies = config.spawnLogic.Spawn(transform.position, config.spawnType.enemyId, config.spawnType.enemyPrefab, target);
                
                // Không phải boss thì spawn orb
                if (!config.isBossGate)
                {
                    var orbs = new Transform[enemies.Length];

                    for (var i = 0; i < enemies.Length; i++)
                    {
                        orbs[i] = EnemyOrbPool.Instance.Get(null);
                        orbs[i].position = transform.position;
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
                    enemy.Item1.Init(config.spawnType, enemy.Item2, WaveHpMultiplier, WaveDmgMultiplier, LevelExpRatio, LevelDarkRatio);
                    enemy.Item1.Activate();
                    enemy.Item1.UniqueId = EnemyManager.Instance.CurrentEnemyIndex;
                    AliveEnemyCount += 1;
                    EnemyManager.Instance.OnEnemySpawn(enemy.Item1);
                    enemy.Item1.OnDead += () =>
                    {
                        AliveEnemyCount -= 1;
                        EnemyManager.Instance.OnEnemyDead(enemy.Item1);
                        CheckAllEnemiesDead();
                    };
                }

                currentSpawnTurn += 1;
                orbSpawnTimer = 0f;
                
                if (TotalSpawnTurn == -1 || currentSpawnTurn < TotalSpawnTurn)
                    yield return new WaitForSeconds(config.intervalLoop);
            }
            
            Deactivate();
            CheckAllEnemiesDead();
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
    }
}