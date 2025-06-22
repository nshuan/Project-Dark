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
}