using InGame.BossConfig;
using UnityEngine;

namespace InGame
{
    [CreateAssetMenu(menuName = "InGame/Enemy/Enemy Boss Summoner King Behaviour", fileName = "EnemyBossSummonerKingBehaviour")]
    public class EnemyBossSummonerKingBehaviour : EnemySummonBehaviour
    {
        [Space] [Header("Summoner King exclusive")]
        public BossSummonerKingConfig summonerKingConfig;
    }
}