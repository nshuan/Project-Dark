using UnityEngine;

namespace InGame.AttackNormalConfig
{
    [CreateAssetMenu(menuName = "InGame/Player/Player Skill Normal Config", fileName = "PlayerSkillNormalConfig")]
    public class PlayerSkillNormalConfig : ScriptableObject
    {
        public int id;
        public float dmgScale;
        public float amount;
    }
    
    public enum NormalType
    {
        Piercing,
        Bullet
    }
}