using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace InGame.EndlessLevel
{
    [CreateAssetMenu(menuName = "InGame/Level/Wave Endless Pool", fileName = "WaveEndlessPool")]
    public class PoolWaveEndless : SerializedScriptableObject
    {
        public WaveEndlessConfig[] allWaves;

        public Dictionary<int, Dictionary<WaveEndlessType, List<int>>> pool;

        private Dictionary<int, WaveEndlessConfig> allWavesMap;

        public void Init()
        {
            allWavesMap = new Dictionary<int, WaveEndlessConfig>();
            if (allWaves == null) return;
            foreach (var wave in allWaves)
            {
                allWavesMap.TryAdd(wave.id, wave);
            }
        }
        
        public WaveEndlessConfig GetRandomWave(int mapId, WaveEndlessType waveType)
        {
            if (pool == null) return null;
            if (allWavesMap == null) return null;
            if (!pool.TryGetValue(mapId, out var poolMapByType)) return null;
            if (!poolMapByType.TryGetValue(waveType, out var waveIdList)) return null;
            if (waveIdList == null || waveIdList.Count == 0) return null;
            var randomWaveId = waveIdList[RandomUtil.Range(0, waveIdList.Count)];
            if (!allWavesMap.TryGetValue(randomWaveId, out var wave)) return null;
            return wave;
        }

        [Space]
        [SerializeField] private string wavePath = "Assets/Dark/Config/LevelEndlessWave";
        [Button]
        private void GetAllWave()
        {
            allWaves = AssetUtility.LoadAllScriptableObjectsInFolder<WaveEndlessConfig>(wavePath).ToArray();
        }
    }
}