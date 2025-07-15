using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace InGame
{
    [CreateAssetMenu(menuName = "InGame/Level/Wave Config", fileName = "WaveConfig")]
    public class WaveConfig : SerializedScriptableObject
    {
        [NonSerialized, OdinSerialize] public List<GateConfig> gateConfigs;
    }
}