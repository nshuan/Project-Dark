using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Serialization;

namespace InGame
{
    [CreateAssetMenu(menuName = "InGame/Passive Effect", fileName = "PassiveConfig")]
    public class PassiveConfig : SerializedScriptableObject
    {
        public int effectId;
        public PassiveTriggerType triggerType;
        public PassiveType logicType;
        public float cooldown;
        [Range(0f, 1f)] public float chance;
        public float size; // Size chính là duration đối với effect burn
        public float value;
        public float stagger;
        public MonoPassiveEntity passivePrefab;
    }
}
