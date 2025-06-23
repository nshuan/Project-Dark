using System;
using Redzen.Structures;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace InGame
{
    [CreateAssetMenu(menuName = "InGame/Player/Player Skill Config", fileName = "PlayerSKillConfig")]   
    public class PlayerSkillConfig : SerializedScriptableObject
    {
        public PlayerSkillType skillType;
        [NonSerialized, OdinSerialize] public PlayerSkillBehaviour shootLogic;
        public float damePerBullet; // bullet base damage
        public int numberOfBullets; // number of bullets in each shot
        public float cooldown;  // time between shots
        public float range; // Max damage range, also max distance from player to mouse aimìng position
        public float size; // size of the aiming field
        public float chargeDameMax = -1;
        public float chargeDameTime = -1;
        public float chargeSizeMax = -1;
        public float chargeSizeTime = -1;
        public float chargeRangeMax = -1;
        public float chargeRangeTime = -1;
        public float chargeBulletInterval = -1;
        public float chargeBulletMaxAdd = -1;
    }

    public enum PlayerSkillType
    {
        
    }
}