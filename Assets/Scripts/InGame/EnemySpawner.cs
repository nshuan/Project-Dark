using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace InGame
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private EnemyEntity enemyPref;
        [SerializeField] private Transform target;
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
                var enemy = Instantiate(enemyPref, transform.position, Quaternion.identity);
                enemy.Target = target;
                enemy.transform.localScale = Vector3.one * Random.Range(0.4f, 1f);
                enemy.Init(CalculateHealth());
                
                yield return new WaitForSeconds(spawnCd);
            }
        }

        protected virtual float CalculateHealth()
        {
            return LevelManager.Instance.GameStats.eBaseHp;
        }
    }
}