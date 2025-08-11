using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace InGame
{
    [CreateAssetMenu(menuName = "InGame/Level/Level Config", fileName = "Level")]
    public class LevelConfig : SerializedScriptableObject
    {
        [ReadOnly] public int level;
        public float levelExpRatio = 1f;
        public float levelDarkRatio = 1f;
        public WaveInfo[] waveInfo;

        private void OnValidate()
        {
            if (waveInfo == null)
            {
                DebugUtility.LogError($"Level {name} does not have any waves!!!");
                return;
            }
            
            for (var i = 0; i < waveInfo.Length; i++)
            {
                waveInfo[i].waveIndex = i;
            }
        }

        // [Button]
        // public void SyncData()
        // {
        //     waveInfo = new WaveInfo[waveInfo.Length];
        //     for (var i = 0; i < waveInfo.Length; i++)
        //     {
        //         waveInfos[i].WaveIndex = i;
        //         waveInfo[i] = new WaveInfo()
        //         {
        //             waveIndex = i,
        //             scaleHp = ((SingleWaveInfo)waveInfos[i]).scaleHp,
        //             scaleDmg = ((SingleWaveInfo)waveInfos[i]).scaleDmg,
        //             timeToEnd = ((SingleWaveInfo)waveInfos[i]).timeToEnd,
        //             waveConfig = ((SingleWaveInfo)waveInfos[i]).waveConfig,
        //         };
        //     }
        // }
    }
}