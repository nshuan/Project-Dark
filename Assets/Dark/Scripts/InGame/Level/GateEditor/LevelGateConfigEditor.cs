using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace InGame.GateEditor
{
    public class LevelGateConfigEditor : SerializedMonoBehaviour
    {
        [NonSerialized, OdinSerialize] public GateConfig gateConfig;

        public void SetupGate(GateConfig config)
        {
            transform.position = config.position;
            
            gateConfig = new GateConfig
            {
                position = config.position,
                targetBaseIndex = config.targetBaseIndex,
                startTime = config.startTime,
                duration = config.duration,
                spawnType = config.spawnType,
                intervalLoop = config.intervalLoop,
                spawnLogic = config.spawnLogic
            };
        }
    }
}