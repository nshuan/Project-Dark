using InGame.BossConfig;
using UnityEngine;

namespace InGame
{
    [CreateAssetMenu(menuName = "InGame/Enemy/Enemy Boss Wizard Behaviour", fileName = "EnemyBossWizardBehaviour")]
    public class EnemyBossWizardBehaviour : EnemySummonBehaviour
    {
        [Space] [Header("Wizard exclusive")]
        public BossWizardConfig wizardConfig;
        public EnemyAttackSummonBehaviour summonBehaviour;
    }
}