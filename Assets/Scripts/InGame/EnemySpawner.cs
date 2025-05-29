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
        [field: SerializeField] public float SpawnRate { get; set; } = 0.5f; // Enemies spawned per seconds
        
        private void Start()
        {
            StartCoroutine(IESpawn(1 / SpawnRate));  
        }

        private IEnumerator IESpawn(float spawnCd)
        {
            while (true)
            {
                var enemy = Instantiate(enemyPref, transform.position, Quaternion.identity);
                enemy.Target = target;
                enemy.transform.localScale = Vector3.one * Random.Range(0.4f, 1f);
                enemy.Init(100f);
                
                yield return new WaitForSeconds(spawnCd);
            }
        }
    }
}