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
        public int passiveId;
        public PassiveTriggerType triggerType;
        public PassiveType logicType;
        public float cooldown;
        [Range(0f, 1f)] public float chance;
        public float size; // duration đối với effect burn, execution chance đối với thunder
        public float value;
        public float stagger;
        public MonoPassiveEntity passivePrefab;

        public virtual float[] GetAdditionalParams()
        {
            return null;
        }
    }
}
