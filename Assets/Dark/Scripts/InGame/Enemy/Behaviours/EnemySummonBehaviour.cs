using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace InGame
{
    [CreateAssetMenu(menuName = "InGame/Enemy/Enemy Summon Behaviour", fileName = "EnemySummonBehaviour")]
    public class EnemySummonBehaviour : EnemyBehaviour
    {
        [Space] [Header("Summoner exclusive")] 
        public string summonIdsString;
        public string summonAmountString;
        public List<int> summonIds;
        public List<int> summonAmount;
        public string summonIdOnSpawnedString;
        public string summonAmountOnSpawnedString;
        public List<int> listSummonIdsOnSpawned;
        public List<int> listSummonAmountOnSpawned;
        
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
}