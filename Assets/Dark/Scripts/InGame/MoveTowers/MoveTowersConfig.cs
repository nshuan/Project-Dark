using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace InGame
{
    [CreateAssetMenu(menuName = "InGame/Move Towers Config", fileName = "MoveTowersConfig")]
    public class MoveTowersConfig : SerializedScriptableObject
    {
        public int id;
        public float delayMove;
        public float cooldown;
        public int damage;
        public float size;
        public float stagger;
        public int maxHitEachTrigger = 5;
        [NonSerialized, OdinSerialize] public IMoveTowersLogic moveLogic;
    }
}