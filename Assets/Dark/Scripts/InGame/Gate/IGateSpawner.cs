using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace InGame
{
    public interface IGateSpawner
    {
        (EnemyEntity, TowerEntity)[] Spawn(Vector2 gatePosition, int enemyId, EnemyEntity enemyPrefab, TowerEntity[] targetTower);
    }
    
    [Serializable]
    public class GateSpawnSingle : IGateSpawner
    {
        [Range(1f, 2f)] public float radius = 1.5f;
        [Range(10f, 90f)] public float randomSpan = 90f;
        
        public (EnemyEntity, TowerEntity)[] Spawn(Vector2 gatePosition, int enemyId, EnemyEntity enemyPrefab, TowerEntity[] targetTower)
        {
            var enemy = EnemyPool.Instance.Get(enemyPrefab, enemyId, null, false);
            TowerEntity target = null;
            
            target = targetTower[RandomUtil.Range(0, targetTower.Length)];
            enemy.transform.position =
                (Vector3)gatePosition + 
                (Quaternion.Euler(0f, 0f, RandomUtil.Range(-randomSpan / 2, randomSpan / 2)) * (target.transform.position - (Vector3)gatePosition).normalized)
                * radius;
            
            return new [] { (enemy, target) };
        }
    }

    [Serializable]
    public class GateSpawnTriangle : IGateSpawner
    {
        [Range(1f, 2f)] public float radius = 1.5f;
        [Range(10f, 90f)] public float randomSpan = 90f;
        
        public (EnemyEntity, TowerEntity)[] Spawn(Vector2 gatePosition, int enemyId, EnemyEntity enemyPrefab, TowerEntity[] targetTower)
        {
            var enemies = new EnemyEntity[3];
            var targets = new TowerEntity[3];

            enemies[0] = EnemyPool.Instance.Get(enemyPrefab, enemyId,null, false);
            targets[0] = targetTower[RandomUtil.Range(0, targetTower.Length)];
            enemies[0].transform.position =
                (Vector3)gatePosition + 
                (Quaternion.Euler(0f, 0f, RandomUtil.Range(-randomSpan / 2, randomSpan / 2)) * (targets[0].transform.position - (Vector3)gatePosition).normalized)
                * radius;

            enemies[1] = EnemyPool.Instance.Get(enemyPrefab, enemyId, null, false);
            targets[1] = targetTower[RandomUtil.Range(0, targetTower.Length)];
            enemies[1].transform.position =
                (Vector3)gatePosition + 
                (Quaternion.Euler(0f, 0f, RandomUtil.Range(-randomSpan / 2, randomSpan / 2)) * (targets[1].transform.position - (Vector3)gatePosition).normalized)
                * radius;
            
            enemies[2] = EnemyPool.Instance.Get(enemyPrefab, enemyId, null, false);
            targets[2] = targetTower[RandomUtil.Range(0, targetTower.Length)];
            enemies[2].transform.position =
                (Vector3)gatePosition + 
                (Quaternion.Euler(0f, 0f, RandomUtil.Range(-randomSpan / 2, randomSpan / 2)) * (targets[2].transform.position - (Vector3)gatePosition).normalized)
                * radius;
            
            return new []
            {
                (enemies[0], targets[0]),
                (enemies[1], targets[1]),
                (enemies[2], targets[2])
            }; 
        }
    }
    
    [Serializable]
    public class GateSpawnMultiple : IGateSpawner
    {
        [Range(1, 10)] public int amount = 1;
        [Range(1f, 2f)] public float maxRadius = 1.8f;
        [Range(10f, 120f)] public float randomSpan = 120f;
        
        public (EnemyEntity, TowerEntity)[] Spawn(Vector2 gatePosition, int enemyId, EnemyEntity enemyPrefab, TowerEntity[] targetTower)
        {
            var enemies = new EnemyEntity[amount];
            var targets = new TowerEntity[amount];
            var result = new (EnemyEntity, TowerEntity)[amount];

            for (var i = 0; i < amount; i++)
            {
                enemies[i] = EnemyPool.Instance.Get(enemyPrefab, enemyId,null, false);
                targets[i] = targetTower[RandomUtil.Range(0, targetTower.Length)];
                enemies[i].transform.position =
                    (Vector3)gatePosition +
                    (Quaternion.Euler(0f, 0f, RandomUtil.Range(-randomSpan / 2, randomSpan / 2)) *
                     (targets[i].transform.position - (Vector3)gatePosition).normalized)
                    * RandomUtil.Range(maxRadius - 0.3f, maxRadius);
                result[i] = (enemies[i], targets[i]);
            }
            
            return result;
        }
    }
}