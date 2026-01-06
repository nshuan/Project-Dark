using System;
using System.Linq;
using UnityEngine;

namespace InGame.AttackNormalConfig
{
    [CreateAssetMenu(menuName = "InGame/Player/Player Skill Normal Config", fileName = "PlayerSkillNormalConfig")]
    public class PlayerSkillNormalConfig : ScriptableObject
    {
        public int id;
        public float dmgScale;
        public float amount;
        public NormalBulletAngleSpanInfo[] angleSpanInfos; // For normal attack bullet

        public int GetNormalBulletSpanAngle(int numberOfBullets)
        {
            if (angleSpanInfos == null || angleSpanInfos.Length == 0) return 30;
            var info = angleSpanInfos.FirstOrDefault((span) => span.numberOfBullets == numberOfBullets);
            if (info == null) return 30;
            return info.angleSpan;
        }
        
        [Serializable]
        public class NormalBulletAngleSpanInfo
        {
            public int numberOfBullets;
            public int angleSpan;
        }
    }
    
    public enum NormalType
    {
        Piercing,
        Bullet
    }
}