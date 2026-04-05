using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace InGame.EndlessLevel
{
    [CreateAssetMenu(menuName = "InGame/Level/Wave Endless Config", fileName = "WaveEndless")]
    public class WaveEndlessConfig : SerializedScriptableObject
    {
        public int id;
        public LevelMapType mapType;
        public Vector2[] towerPositions;
        [NonSerialized, OdinSerialize] public List<GateConfig> gateConfigs;
    }
}