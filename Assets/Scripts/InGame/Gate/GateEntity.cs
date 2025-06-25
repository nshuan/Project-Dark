using System;
using System.Collections;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Serialization;

namespace InGame
{
    public class GateEntity : SerializedMonoBehaviour
    {
        [ReadOnly] public TowerEntity target;

        #region Gate config

        [Space] [Header("Gate config")] 
        [NonSerialized, OdinSerialize, ReadOnly] private GateConfig config;

        #endregion
        
        private void Start()
        {
            StartCoroutine(IESpawn());  
        }

        public void Initialize(GateConfig cfg, TowerEntity targetBase)
        {
            config = cfg;
            target = targetBase;
        }
        
        private IEnumerator IESpawn()
        {
            yield return new WaitForSeconds(config.startTime);
            
            while (true)
            {
                var enemies = config.spawnLogic.Spawn(transform.position, config.spawnType);
                foreach (var enemy in enemies)
                {
                    enemy.Init(target, config.spawnType, CalculateHpMultiplier());
                    enemy.Activate();
                    enemy.Id = EnemyManager.Instance.CurrentEnemyIndex;
                    EnemyManager.Instance.OnEnemySpawn(enemy);
                    enemy.OnDead += () =>
                    {
                        EnemyManager.Instance.OnEnemyDead(enemy);
                    };
                }
                
                yield return new WaitForSeconds(config.intervalLoop);
            }
        }

        protected float CalculateHpMultiplier()
        {
            return LevelManager.Instance.GameStats.eHpMultiplier;
        }
    }
}