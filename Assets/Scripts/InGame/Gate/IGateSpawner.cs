using System;
using UnityEngine;

namespace InGame
{
    public interface IGateSpawner
    {
        EnemyEntity[] Spawn(Vector2 gatePosition, EnemyType enemyType);
    }
    
    [Serializable]
    public class GateSpawnSingle : IGateSpawner
    {
        public EnemyEntity[] Spawn(Vector2 gatePosition, EnemyType enemyType)
        {
            var enemy = EnemyPool.Instance.Get(enemyType, null);
            enemy.transform.position = gatePosition;
            return new [] { enemy };
        }
    }

    [Serializable]
    public class GateSpawnTriangle : IGateSpawner
    {
        [Range(1f, 2f)] public float radius = 1.5f;
        [Range(0f, 360f)] public float rotation = 0f;
        
        public EnemyEntity[] Spawn(Vector2 gatePosition, EnemyType enemyType)
        {
            var enemies = new EnemyEntity[3];

            enemies[0] = EnemyPool.Instance.Get(enemyType, null);
            enemies[0].transform.position = 
                gatePosition + (Vector2)(Quaternion.Euler(0f, 0f, rotation) * Vector2.up).normalized * radius;
            enemies[1] = EnemyPool.Instance.Get(enemyType, null);
            enemies[1].transform.position =
                gatePosition + (Vector2)(Quaternion.Euler(0f, 0f, 120f + rotation) * Vector2.up).normalized * radius;
            enemies[2] = EnemyPool.Instance.Get(enemyType, null);
            enemies[2].transform.position =
                gatePosition + (Vector2)(Quaternion.Euler(0f, 0f, -120f + rotation) * Vector2.up).normalized * radius;
            
            return enemies; 
        }
    }
}