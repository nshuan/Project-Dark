using System;
using UnityEngine;
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
        public (EnemyEntity, TowerEntity)[] Spawn(Vector2 gatePosition, int enemyId, EnemyEntity enemyPrefab, TowerEntity[] targetTower)
        {
            var enemy = EnemyPool.Instance.Get(enemyPrefab, enemyId, null, false);
            TowerEntity target = null;
            
            target = targetTower[Random.Range(0, targetTower.Length)];
            enemy.transform.position =
                (Vector3)gatePosition + 
                (Quaternion.Euler(0f, 0f, Random.Range(-45f, 45f)) * (target.transform.position - (Vector3)gatePosition).normalized)
                * 1.5f;
            
            return new [] { (enemy, target) };
        }
    }

    [Serializable]
    public class GateSpawnTriangle : IGateSpawner
    {
        [Range(1f, 2f)] public float radius = 1.5f;
        
        public (EnemyEntity, TowerEntity)[] Spawn(Vector2 gatePosition, int enemyId, EnemyEntity enemyPrefab, TowerEntity[] targetTower)
        {
            var enemies = new EnemyEntity[3];
            var targets = new TowerEntity[3];

            enemies[0] = EnemyPool.Instance.Get(enemyPrefab, enemyId,null, false);
            targets[0] = targetTower[Random.Range(0, targetTower.Length)];
            enemies[0].transform.position =
                (Vector3)gatePosition + 
                (Quaternion.Euler(0f, 0f, Random.Range(-45f, 45f)) * (targets[0].transform.position - (Vector3)gatePosition).normalized)
                * radius;

            enemies[1] = EnemyPool.Instance.Get(enemyPrefab, enemyId, null, false);
            targets[1] = targetTower[Random.Range(0, targetTower.Length)];
            enemies[1].transform.position =
                (Vector3)gatePosition + 
                (Quaternion.Euler(0f, 0f, Random.Range(-45f, 45f)) * (targets[1].transform.position - (Vector3)gatePosition).normalized)
                * radius;
            
            enemies[2] = EnemyPool.Instance.Get(enemyPrefab, enemyId, null, false);
            targets[2] = targetTower[Random.Range(0, targetTower.Length)];
            enemies[2].transform.position =
                (Vector3)gatePosition + 
                (Quaternion.Euler(0f, 0f, Random.Range(-45f, 45f)) * (targets[2].transform.position - (Vector3)gatePosition).normalized)
                * radius;
            
            return new []
            {
                (enemies[0], targets[0]),
                (enemies[1], targets[1]),
                (enemies[2], targets[2])
            }; 
        }
    }
}