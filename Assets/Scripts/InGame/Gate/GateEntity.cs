using System;
using System.Collections;
using UnityEngine;

namespace InGame
{
    public class GateEntity : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private EnemyType spawnType;
        
        private float SpawnRate => LevelManager.Instance.GameStats.eSpawnRate;
        
        private void Start()
        {
            StartCoroutine(IESpawn());  
        }

        private IEnumerator IESpawn()
        {
            while (true)
            {
                var spawnCd = 1 / SpawnRate;
                var enemy = EnemyPool.Instance.Get(spawnType, null);
                enemy.transform.position = transform.position;
                enemy.Init(target, CalculateHpMultiplier());
                enemy.Activate();
                
                yield return new WaitForSeconds(spawnCd);
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