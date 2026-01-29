using System;
using System.Linq;
using UnityEngine;

namespace InGame
{
    public interface IGateSpawner
    {
        // (enemy, target, shouldRandomAttackPosition, attackPosition)
        (EnemyEntity, TowerEntity, bool, Vector3)[] Spawn(GateEntity gate, int enemyId, EnemyEntity enemyPrefab, TowerEntity[] targetTower);
    }
    
    [Serializable]
    public class GateSpawnSingle : IGateSpawner
    {
        [Range(1f, 2f)] public float radius = 1.5f;
        public float randomSpanAngle = 90f;
        
        public (EnemyEntity, TowerEntity, bool, Vector3)[] Spawn(GateEntity gate, int enemyId, EnemyEntity enemyPrefab, TowerEntity[] targetTower)
        {
            var enemy = EnemyPool.Instance.Get(enemyPrefab, enemyId, null, false);
            TowerEntity target = null;
            
            target = targetTower[RandomUtil.Range(0, targetTower.Length)];
            enemy.transform.position =
                gate.transform.position + 
                (Quaternion.Euler(0f, 0f, RandomUtil.Range(-randomSpanAngle / 2, randomSpanAngle / 2)) * (target.transform.position - gate.transform.position).normalized)
                * radius;
            
            return new [] { (enemy, target, true, target.transform.position) };
        }
    }

    [Serializable]
    public class GateSpawnTriangle : IGateSpawner
    {
        [Range(1f, 2f)] public float radius = 1.5f;
        public float randomSpanAngle = 90f;
        
        public (EnemyEntity, TowerEntity, bool, Vector3)[] Spawn(GateEntity gate, int enemyId, EnemyEntity enemyPrefab, TowerEntity[] targetTower)
        {
            var enemies = new EnemyEntity[3];
            var targets = new TowerEntity[3];

            enemies[0] = EnemyPool.Instance.Get(enemyPrefab, enemyId,null, false);
            targets[0] = targetTower[RandomUtil.Range(0, targetTower.Length)];
            enemies[0].transform.position =
                gate.transform.position + 
                (Quaternion.Euler(0f, 0f, RandomUtil.Range(-randomSpanAngle / 2, randomSpanAngle / 2)) * (targets[0].transform.position - gate.transform.position).normalized)
                * radius;

            enemies[1] = EnemyPool.Instance.Get(enemyPrefab, enemyId, null, false);
            targets[1] = targetTower[RandomUtil.Range(0, targetTower.Length)];
            enemies[1].transform.position =
                gate.transform.position + 
                (Quaternion.Euler(0f, 0f, RandomUtil.Range(-randomSpanAngle / 2, randomSpanAngle / 2)) * (targets[1].transform.position - gate.transform.position).normalized)
                * radius;
            
            enemies[2] = EnemyPool.Instance.Get(enemyPrefab, enemyId, null, false);
            targets[2] = targetTower[RandomUtil.Range(0, targetTower.Length)];
            enemies[2].transform.position =
                gate.transform.position + 
                (Quaternion.Euler(0f, 0f, RandomUtil.Range(-randomSpanAngle / 2, randomSpanAngle / 2)) * (targets[2].transform.position - gate.transform.position).normalized)
                * radius;
            
            return new []
            {
                (enemies[0], targets[0], true, targets[0].transform.position),
                (enemies[1], targets[1], true, targets[1].transform.position),
                (enemies[2], targets[2], true, targets[2].transform.position)
            }; 
        }
    }
    
    [Serializable]
    public class GateSpawnMultiple : IGateSpawner
    {
        [Range(1, 10)] public int amount = 1;
        [Range(1f, 2f)] public float maxRadius = 1.8f;
        public float randomSpanAngle = 120f;
        
        public (EnemyEntity, TowerEntity, bool, Vector3)[] Spawn(GateEntity gate, int enemyId, EnemyEntity enemyPrefab, TowerEntity[] targetTower)
        {
            var enemies = new EnemyEntity[amount];
            var targets = new TowerEntity[amount];
            var result = new (EnemyEntity, TowerEntity, bool, Vector3)[amount];

            for (var i = 0; i < amount; i++)
            {
                enemies[i] = EnemyPool.Instance.Get(enemyPrefab, enemyId,null, false);
                targets[i] = targetTower[RandomUtil.Range(0, targetTower.Length)];
                enemies[i].transform.position =
                    gate.transform.position +
                    (Quaternion.Euler(0f, 0f, RandomUtil.Range(-randomSpanAngle / 2, randomSpanAngle / 2)) *
                     (targets[i].transform.position - gate.transform.position).normalized)
                    * RandomUtil.Range(maxRadius - 0.3f, maxRadius);
                result[i] = (enemies[i], targets[i], true, targets[i].transform.position);
            }
            
            return result;
        }
    }

    [Serializable]
    public class GateSpawnCenter : IGateSpawner
    {
        public (EnemyEntity, TowerEntity, bool, Vector3)[] Spawn(GateEntity gate, int enemyId, EnemyEntity enemyPrefab,
            TowerEntity[] targetTower)
        {
            var enemy = EnemyPool.Instance.Get(enemyPrefab, enemyId, null, false);
            TowerEntity target = null;
            
            target = targetTower[RandomUtil.Range(0, targetTower.Length)];
            var totalPossibleSpawnPosition = gate.OrbSpawnPositions?.Length ?? 0;
            if (totalPossibleSpawnPosition > 0)
            {
                enemy.transform.position = gate.OrbSpawnPositions[RandomUtil.Range(0, totalPossibleSpawnPosition)]
                    .position;
            }
            else
            {
                enemy.transform.position = gate.transform.position + new Vector3(0.1f, 0f, 0f);
            }

            return new[] { (enemy, target, true, target.transform.position) };
        }
    }

    [Serializable]
    public class GateSpawnPositions : IGateSpawner
    {
        [Range(1, 10)] public int amount = 1;
        public Vector2[] spawnPositions;
        public GateSpawnPositionInfo[] spawnPositionInfos;
        
        public (EnemyEntity, TowerEntity, bool, Vector3)[] Spawn(GateEntity gate, int enemyId, EnemyEntity enemyPrefab, TowerEntity[] targetTower)
        {
            var enemies = new EnemyEntity[amount];
            var targets = new TowerEntity[amount];
            var result = new (EnemyEntity, TowerEntity, bool, Vector3)[amount];

            if (spawnPositionInfos == null || spawnPositionInfos.Length == 0)
            {
                if (spawnPositions != null)
                {
                    spawnPositionInfos = spawnPositions.Select((pos) => new GateSpawnPositionInfo()
                        { spawnPosition = pos }).ToArray();
                }   
            }
            
            var spawnPositionIndex = spawnPositionInfos is { Length: > 0 }
                ? RandomUtil.ShuffleIndex(0, spawnPositionInfos.Length - 1)
                : new int[] { };
            var currentSpawnPositionIndexIndex = 0;
            
            for (var i = 0; i < amount; i++)
            {
                enemies[i] = EnemyPool.Instance.Get(enemyPrefab, enemyId,null, false);
                targets[i] = targetTower[RandomUtil.Range(0, targetTower.Length)];
                
                if (spawnPositionInfos == null || spawnPositionInfos.Length == 0)
                {
                    enemies[i].transform.position = gate.transform.position + new Vector3(0.1f, 0f, 0f);
                    result[i] = (enemies[i], targets[i], true, targets[i].transform.position);
                }
                else
                {
                    var index = spawnPositionIndex[currentSpawnPositionIndexIndex];
                    enemies[i].transform.position = spawnPositionInfos[index].spawnPosition;
                    if (spawnPositionInfos[index].attackPositions is { Length: > 0 })
                    {
                        result[i] = (enemies[i], targets[i], false,
                            spawnPositionInfos[index]
                                .attackPositions[RandomUtil.Range(0, spawnPositionInfos[index].attackPositions.Length)]);
                    }
                    else
                    {
                        result[i] = (enemies[i], targets[i], true, targets[i].transform.position);
                    }
                    
                    currentSpawnPositionIndexIndex += 1;
                    if (currentSpawnPositionIndexIndex >= spawnPositionInfos.Length)
                    {
                        RandomUtil.ShuffleIndexNonAlloc(spawnPositionIndex, 0);
                        currentSpawnPositionIndexIndex = 0;
                    }
                }
            }
            
            return result;
        }
    }
    
    [Serializable]
    public class GateSpawnPositionInfo
    {
        public Vector2 spawnPosition;
        public Vector2[] attackPositions;
    }
}