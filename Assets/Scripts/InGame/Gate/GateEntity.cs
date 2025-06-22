using System;
using System.Collections;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace InGame
{
    public class GateEntity : SerializedMonoBehaviour
    {
        [SerializeField] private Transform target;

        #region Gate config

        [Space] [Header("Gate config")] 
        [SerializeField] private float startTime; // delay before the gate start spawning
        [SerializeField] private float duration; // gate life time
        [SerializeField] private EnemyType spawnType;
        [SerializeField] private float intervalLoop = 4f; // duration between 2 spawns
        [NonSerialized, OdinSerialize] private IGateSpawner spawnLogic; // pattern for enemy appearance

        #endregion
        
        private void Start()
        {
            StartCoroutine(IESpawn());  
        }

        private IEnumerator IESpawn()
        {
            yield return new WaitForSeconds(startTime);
            
            while (true)
            {
                var enemies = spawnLogic.Spawn(transform.position, spawnType);
                foreach (var enemy in enemies)
                {
                    enemy.Init(target, spawnType, CalculateHpMultiplier());
                    enemy.Activate();
                }
                
                yield return new WaitForSeconds(intervalLoop);
            }
        }

        protected virtual float CalculateHpMultiplier()
        {
            return LevelManager.Instance.GameStats.eHpMultiplier;
        }
    }

    [Serializable]
    public class GateData
    {
        public float startTime; // Time to wait before start spawning
        public float duration; // Gate lifetime
        public GameObject enemy; // prefab of the enemy that spawn from this gate
        public float intervalLoop; // spawn cooldown
        public int groupPattern; // pattern for enemy appearing phase
    }
}