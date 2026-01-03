using UnityEngine;

namespace InGame.BossConfig
{
    [CreateAssetMenu(menuName = "InGame/Boss/Boss Lord of Flame", fileName = "BossLordOfFlame")]
    public class BossLordOfFlameConfig : BossBehaviourConfig
    {
        [Tooltip("Hp reach down to this percentage, buff recover hp")]
        public float percentageToHeal = 0.3f;
        [Tooltip("Hp to recover once")]
        public float percentageHealed = 0.3f;
        [Tooltip("Tower to move to in phase 2")]
        public int phase2TowerId = 2;
        [Tooltip("Attack range in phase 2")] 
        public float phase2AtkRange = 3f;
    }
}