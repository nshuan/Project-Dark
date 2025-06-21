using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace InGame
{
    [Serializable]
    public class GameStats
    {
        // Enemy
        [Header("Enemy")]
        public float eSpawnRate = 0.5f; // Enemies spawned per seconds
        public float eHpMultiplier = 1f;
        public float eBaseMoveSpeed = 10f;
        public float eBaseAttackRange = 2f;
        public float eBaseAttackCd = 2f;
        
        // Player
        [Space]
        [Header("Player")]
        public float pShotCooldown = 1f;
        public float pDmgPerShot = 15f;
        public float cursorScale = 1f;

        // Elemental Skills
        [Space] 
        [Header("Elemental")] 
        public float sLightningChance = 0.5f;
        public float sLightningDamage = 5f;
        public float sLightningBurstChance = 0.5f;
        public float sLightningBurstDamage = 5f;
        
        #region FUNCTION

        public static T CalculateStat<T>(T baseStat, params Func<T, T>[] effects)
        {
            var result = baseStat;
            foreach (var effect in effects)
            {
                result = effect(result);
            }
            return result;
        }

        #endregion
    }
}