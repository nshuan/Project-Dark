using System;
using System.Collections.Generic;
using Core;
using Object = UnityEngine.Object;

namespace InGame
{
    public class EnemyManager : Singleton<EnemyManager>
    {
        public Dictionary<int, EnemyEntity> Enemies { get; private set; } = new Dictionary<int, EnemyEntity>();
        public Dictionary<int, bool> EnemiesAliveMap {get; private set; } = new Dictionary<int, bool>();
        public int CurrentEnemyIndex { get; private set; } // index is stored as enemy id
        
        public Action OnOneEnemyDead { get; set; }
        
        public void Initialize()
        { 
            Enemies = new Dictionary<int, EnemyEntity>();
            EnemiesAliveMap = new Dictionary<int, bool>();
            CurrentEnemyIndex = 0;

            LevelManager.Instance.OnLose += OnLevelCompleted;
        }

        private void OnLevelCompleted()
        {
            for (var i = 0; i < Enemies.Count; i++)
            {
                if (EnemiesAliveMap[i] == false) continue;
                Enemies[i].Stop();
            }
        }
        
        public void OnEnemySpawn(EnemyEntity enemy)
        {
            Enemies.Add(CurrentEnemyIndex, enemy);
            EnemiesAliveMap.Add(CurrentEnemyIndex, true);
            CurrentEnemyIndex += 1;
        }

        public void OnEnemyDead(EnemyEntity enemy)
        {
            EnemiesAliveMap[enemy.UniqueId] = false;
            OnOneEnemyDead?.Invoke();
        }

        public int FilterEnemiesNonAlloc(Func<EnemyEntity, bool> filter, ref EnemyEntity[] enemies, bool aliveOnly = true)
        {
            if (enemies == null) return 0;

            var count = 0;
            foreach (var enemy in Enemies)
            {
                if (count >= enemies.Length) break;
                if (aliveOnly && EnemiesAliveMap[enemy.Key] == false) continue;

                if (filter(enemy.Value))
                {
                    enemies[count] = enemy.Value;
                    count += 1;
                }
            }

            return count;
        }
    }
}