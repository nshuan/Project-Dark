using System;
using UnityEngine;

namespace InGame.BossConfig
{
    [CreateAssetMenu(menuName = "InGame/Boss/Boss Summoner King", fileName = "BossSummonerKing")]
    public class BossSummonerKingConfig : BossBehaviourConfig
    {
        [Tooltip("Should buff all enemy on spawn")]
        public bool buffOnSpawn = true;

        [Tooltip("Buff value on spawn")] 
        public SummonerKingBuffInfo buffSpawn;
        
        [Serializable]
        public class SummonerKingBuffInfo
        {
            public float scaleDmg = 1f;
            public float scaleSpeed = 1f;
            public float scaleAtkSpeed = 1f;
        }
    }
    
}