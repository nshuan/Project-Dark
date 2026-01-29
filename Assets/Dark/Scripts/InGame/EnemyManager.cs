using System;
using System.Collections.Generic;
using Core;
using Object = UnityEngine.Object;

namespace InGame
{
    public class EnemyManager : Singleton<EnemyManager>
    {
        public Dictionary<int, EnemyEntity> Enemies { get; private set; } = new Dictionary<int, EnemyEntity>();
        public int CurrentEnemyIndex { get; private set; } // index is stored as enemy id
        
        public void Initialize()
        { 
            Enemies = new Dictionary<int, EnemyEntity>();
            CurrentEnemyIndex = 0;

            LevelManager.Instance.OnLose += OnLevelCompleted;
            LevelManager.Instance.OnWin += OnLevelCompleted;
        }

        private void OnLevelCompleted()
        {
            CombatActions.OnOneEnemyDead = null;
            
            for (var i = 0; i < Enemies.Count; i++)
            {
                if (Enemies[i].gameObject.activeInHierarchy)
                    Enemies[i].Stop();
            }
        }
        
        public void OnEnemySpawn(EnemyEntity enemy)
        {
            Enemies.Add(CurrentEnemyIndex, enemy);
            CurrentEnemyIndex += 1;
            CombatActions.OnOneEnemySpawn?.Invoke(enemy);
        }

        public void OnEnemyDead(EnemyEntity enemy, EnemyDieReason reason)
        {
            CombatActions.OnOneEnemyDead?.Invoke(enemy, reason);
        }

        public int FilterEnemiesNonAlloc(Func<EnemyEntity, bool> filter, ref EnemyEntity[] enemies, bool aliveOnly = true)
        {
            if (enemies == null) return 0;

            var count = 0;
            foreach (var enemy in Enemies)
            {
                if (count >= enemies.Length) break;
                var enemyScript = Enemies[enemy.Key];
                if (aliveOnly && (enemyScript.IsDestroyed || !enemyScript.gameObject.activeInHierarchy)) continue;

                if (filter(enemy.Value))
                {
                    enemies[count] = enemy.Value;
                    count += 1;
                }
            }

            return count;
        }
    }

    public enum EnemyDieReason
    {
        PlayerKill,
        TowerKill,
        Suicide,
        EnemyKill
    }
}