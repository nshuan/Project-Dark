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
        public bool IsActive { get; set; } = false;
        private int TotalSpawnTurn { get; set; } // unlimited = -1
        private int currentSpawnTurn = 0;
        private int AliveEnemyCount { get; set; }
        #region Gate config

        [Space] [Header("Gate config")] 
        [NonSerialized, OdinSerialize, ReadOnly] private GateConfig config;

        #endregion

        public Action OnAllEnemiesDead { get; set; }
        
        public void Activate()
        {
            spawnCoroutine = StartCoroutine(IESpawn());
            // Duration < 0 thì spawn mãi mãi
            if (config.duration >= 0)
                lifeTimeCoroutine = StartCoroutine(IELifeTime(config.duration + config.startTime));
            IsActive = true;
        }

        public void Deactivate()
        {
            if (lifeTimeCoroutine != null)
                StopCoroutine(lifeTimeCoroutine);
            if (spawnCoroutine != null)
                StopCoroutine(spawnCoroutine);

            IsActive = false;
            gameObject.SetActive(false);
        }
        
        public void Initialize(GateConfig cfg, TowerEntity[] targetBase, float waveHpMultiplier, float waveDmgMultiplier)
        {
            config = cfg;
            target = targetBase;
            WaveHpMultiplier = waveHpMultiplier;
            WaveDmgMultiplier = waveDmgMultiplier;
            TotalSpawnTurn = cfg.duration >= 0 ? (int)(cfg.duration / cfg.intervalLoop) : -1;
            currentSpawnTurn = 0;
            AliveEnemyCount = 0;
            IsActive = false;
            
            if (lifeTimeCoroutine != null)
                StopCoroutine(lifeTimeCoroutine);
            if (spawnCoroutine != null)
                StopCoroutine(spawnCoroutine);
        }
        
        private Coroutine spawnCoroutine;
        private IEnumerator IESpawn()
        {
            yield return new WaitForSeconds(config.startTime);
            
            while (TotalSpawnTurn == -1 || currentSpawnTurn < TotalSpawnTurn)
            {
                var enemies = config.spawnLogic.Spawn(transform.position, config.spawnType.enemyPrefab);
                for (var i = 0; i < enemies.Length; i++)
                {
                    var enemy = enemies[i];
                    enemy.Init(target[Random.Range(0, target.Length)], WaveHpMultiplier, WaveDmgMultiplier);
                    enemy.Activate();
                    enemy.Id = EnemyManager.Instance.CurrentEnemyIndex;
                    AliveEnemyCount += 1;
                    EnemyManager.Instance.OnEnemySpawn(enemy);
                    enemy.OnDead += () =>
                    {
                        AliveEnemyCount -= 1;
                        CheckAllEnemiesDead();
                        EnemyManager.Instance.OnEnemyDead(enemy);
                    };
                }

                currentSpawnTurn += 1;
                yield return new WaitForSeconds(config.intervalLoop);
            }
        }
        
        private Coroutine lifeTimeCoroutine;
        private IEnumerator IELifeTime(float duration)
        {
            yield return new WaitForSeconds(duration);
            Deactivate();
        }

        private void CheckAllEnemiesDead()
        {
            if (TotalSpawnTurn == -1 || currentSpawnTurn < TotalSpawnTurn) return;
            if (AliveEnemyCount == 0)
            {
                Deactivate();
                OnAllEnemiesDead?.Invoke();
                OnAllEnemiesDead = null;
            }
        }
    }
}