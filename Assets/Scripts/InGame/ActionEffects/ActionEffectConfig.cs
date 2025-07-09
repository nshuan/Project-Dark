using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace InGame
{
    [CreateAssetMenu(menuName = "InGame/Action Effect", fileName = "ActionEffectConfig")]
    public class ActionEffectConfig : SerializedScriptableObject
    {
        public int effectId;
        public EffectTriggerType triggerType;
        public EffectType logicType;
        public float cooldown;
        [Range(0f, 1f)] public float chance;
        public float size;
        public float value;
        public float stagger;
        public MonoEffectEntity effectPrefab;
    }
}
