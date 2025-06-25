using System.Collections.Generic;
using Core;
using UnityEngine;

namespace InGame
{
    public class EnemyManager : Singleton<EnemyManager>
    {
        public Dictionary<int, EnemyEntity> Enemies { get; private set; } = new Dictionary<int, EnemyEntity>();
        public int CurrentEnemyIndex { get; private set; } // index is stored as enemy id
        
        public void Initialize()
        {
            if (Enemies == null)
                Enemies = new Dictionary<int, EnemyEntity>();
            
            if (Enemies.Count > 0)
            {
                foreach (var enemy in Enemies)
                {
                    GameObject.Destroy(enemy.Value.gameObject);
                }
                Enemies.Clear();
            }
        }
        
        public void OnEnemySpawn(EnemyEntity enemy)
        {
            Enemies.Add(CurrentEnemyIndex, enemy);
            CurrentEnemyIndex += 1;
        }

        public void OnEnemyDead(EnemyEntity enemy)
        {
            if (!Enemies.ContainsKey(enemy.Id)) return;

            Enemies.Remove(enemy.Id);
        }
    }
}