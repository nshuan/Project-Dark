using InGame.BossConfig;
using UnityEngine;

namespace InGame
{
    [CreateAssetMenu(menuName = "InGame/Enemy/Enemy Boss Lord of Flame Behaviour", fileName = "EnemyBossLordOfFlameBehaviour")]
    public class EnemyBossLordOfFlameBehaviour : EnemyBehaviour
    {
        [Space] [Header("Lord of Flame exclusive")]
        public BossLordOfFlameConfig lordOfFlameConfig;
    }
}