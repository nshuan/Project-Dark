using InGame.BossConfig;
using UnityEngine;

namespace InGame
{
    [CreateAssetMenu(menuName = "InGame/Enemy/Enemy Boss Tarnished Behaviour", fileName = "EnemyBossTarnishedBehaviour")]
    public class EnemyBossTarnishedBehaviour : EnemyBehaviour
    {
        [Space] [Header("The Tarnished exclusive")] 
        public BossTarnishedConfig tarnishedConfig;
    }
}