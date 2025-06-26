using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace InGame
{
    [CreateAssetMenu(menuName = "InGame/Level/Wave Config", fileName = "WaveConfig")]
    public class WaveConfig : SerializedScriptableObject
    {
        [NonSerialized, OdinSerialize] public GateConfig[] gateConfigs;
    }
}