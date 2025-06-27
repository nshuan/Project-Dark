using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace InGame
{
    [CreateAssetMenu(menuName = "InGame/Level/Level Config", fileName = "Level")]
    public class LevelConfig : SerializedScriptableObject
    {
        [NonSerialized, OdinSerialize] public IWaveInfo[] waveInfos;

        private void OnValidate()
        {
            if (waveInfos == null)
            {
                DebugUtility.LogError($"Level {name} does not have any waves!!!");
                return;
            }
            
            for (var i = 0; i < waveInfos.Length; i++)
            {
                waveInfos[i].WaveIndex = i;
            }
        }
    }
}