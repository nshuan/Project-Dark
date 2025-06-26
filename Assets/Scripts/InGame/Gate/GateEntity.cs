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
        private float WaveHpMultiplier { get; set; }
        private float WaveDmgMultiplier { get; set; }
        public bool IsActive { get; set; } = false;

        #region Gate config

        [Space] [Header("Gate config")] 
        [NonSerialized, OdinSerialize, ReadOnly] private GateConfig config;

        #endregion

        public void Activate()
        {
            spawnCoroutine = StartCoroutine(IESpawn());      
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
        
        public void Initialize(GateConfig cfg, TowerEntity targetBase, float waveHpMultiplier, float waveDmgMultiplier)
        {
            config = cfg;
            target = targetBase;
            WaveHpMultiplier = waveHpMultiplier;
            WaveDmgMultiplier = waveDmgMultiplier;
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
            
            while (true)
            {
                var enemies = config.spawnLogic.Spawn(transform.position, config.spawnType);
                foreach (var enemy in enemies)
                {
                    enemy.Init(target, config.spawnType, WaveHpMultiplier, WaveDmgMultiplier);
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
        
        private Coroutine lifeTimeCoroutine;
        private IEnumerator IELifeTime(float duration)
        {
            yield return new WaitForSeconds(duration);
            Deactivate();
        }
    }
}