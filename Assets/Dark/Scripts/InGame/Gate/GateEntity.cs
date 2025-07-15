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
        }
        
        private Coroutine spawnCoroutine;
        private IEnumerator IESpawn()
        {
            yield return new WaitForSeconds(config.startTime);
            
            visual.SetActive(true);
            
            while (TotalSpawnTurn == -1 || currentSpawnTurn < TotalSpawnTurn)
            {
                yield return new WaitForSeconds(config.intervalLoop);
                
                var enemies = config.spawnLogic.Spawn(transform.position, config.spawnType.enemyId, config.spawnType.enemyPrefab);
                for (var i = 0; i < enemies.Length; i++)
                {
                    var enemy = enemies[i];
                    enemy.Init(config.spawnType, target[Random.Range(0, target.Length)], WaveHpMultiplier, WaveDmgMultiplier, LevelExpRatio, LevelDarkRatio);
                    enemy.Activate();
                    enemy.UniqueId = EnemyManager.Instance.CurrentEnemyIndex;
                    AliveEnemyCount += 1;
                    EnemyManager.Instance.OnEnemySpawn(enemy);
                    enemy.OnDead += () =>
                    {
                        AliveEnemyCount -= 1;
                        EnemyManager.Instance.OnEnemyDead(enemy);
                        CheckAllEnemiesDead();
                    };
                }

                currentSpawnTurn += 1;
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