using UnityEngine;

namespace InGame.AttackNormalConfig
{
    [CreateAssetMenu(menuName = "InGame/Player/Knight Skill Normal Config", fileName = "KnightSkillNormalConfig")]
    public class KnightSkillNormalConfig : PlayerSkillNormalConfig
    {
        public float sizeScale = 1f;
        public float rangeScale = 1f;
        public float atkSpeed = 1f; // Number of slash per second
        
    }
}