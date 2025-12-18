using System;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace InGame
{
    [CreateAssetMenu(menuName = "InGame/Enemy/Enemy Behaviour", fileName = "EnemyBehaviour")]
    public class EnemyBehaviour : ScriptableObject
    {
        public int enemyId;
        public bool elite;
        public EnemyEntity enemyPrefab;
        public EnemySpawnBehaviour spawnBehaviour;
        public EnemyMoveBehaviour moveBehaviour;
        public EnemyAttackBehaviour attackBehaviour;
        public float attackRange; // Distance to start attacking
        public float attackSpeed; // Hit per second
        public int hp;
        public int dmg; // Base damage
        public float moveSpeed;
        public float staggerResist; // Reduce projectile stagger
        public float staggerVelocity = 1.5f; // Hit back X on 1s
        public float invisibleDuration;
        public int exp;
        public int dark;
        [Range(0f, 1f)] public float darkRatio;
        public int bossPoint;

        [Space] [Header("Summoner exclusive")] 
        public string summonIdsString;
        public string summonAmountString;
        public List<int> summonIds;
        public List<int> summonAmount;
        public string summonIdOnSpawnedString;
        public string summonAmountOnSpawnedString;
        public List<int> listSummonIdsOnSpawned;
        public List<int> listSummonAmountOnSpawned;
        
        public void Init(EnemyEntity enemy)
        {
            spawnBehaviour.Init(enemy);
        }
        
        public void Spawn(EnemyEntity enemy, float delayComplete, Action completeCallback)
        {
            DOTween.Kill(enemy);
            if (spawnBehaviour)
            {
                enemy.gameObject.SetActive(true);
                DOTween.Sequence().Append(spawnBehaviour.DoSpawn(enemy))
                    .AppendInterval(delayComplete)
                    .OnComplete(() => completeCallback?.Invoke()).SetTarget(enemy);
            }
        }

        [Button]
        public void Validate()
        {
            summonIds = new List<int>();
            summonAmount = new List<int>();
            if (!string.IsNullOrEmpty(summonIdsString) && !string.IsNullOrEmpty(summonAmountString))
            {
                var idsSplit = summonIdsString.Split(",");
                var amountSplit = summonAmountString.Split(",");
                for (var i = 0; i < idsSplit.Length; i++)
                {
                    if (int.TryParse(idsSplit[i], out var id))
                    {
                        summonIds.Add(id);
                        if (i < amountSplit.Length)
                        {
                            summonAmount.Add(int.TryParse(amountSplit[i], out var amount) ? amount : 0);
                        }
                        else summonAmount.Add(0);
                    }
                }
            }   
            
            listSummonIdsOnSpawned = new List<int>();
            listSummonAmountOnSpawned = new List<int>();
            if (!string.IsNullOrEmpty(summonIdOnSpawnedString) && !string.IsNullOrEmpty(summonAmountOnSpawnedString))
            {
                var idsSplit = summonIdOnSpawnedString.Split(",");
                var amountSplit = summonAmountOnSpawnedString.Split(",");
                for (var i = 0; i < idsSplit.Length; i++)
                {
                    if (int.TryParse(idsSplit[i], out var id))
                    {
                        listSummonIdsOnSpawned.Add(id);
                        if (i < amountSplit.Length)
                        {
                            listSummonAmountOnSpawned.Add(int.TryParse(amountSplit[i], out var amount) ? amount : 0);
                        }
                        else listSummonAmountOnSpawned.Add(0);
                    }
                }
            }   
        }
        
        private void OnValidate()
        {
            Validate();
        }
    }

    public enum EnemyState
    {
        Spawn,
        Move,
        Invisible,
        Freeze
    }
}