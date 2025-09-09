using System;
using System.Collections;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace InGame
{
    [Serializable]
    public class WaveInfo
    {
        public int waveIndex;
        public float scaleHp = 1f;
        public float scaleDmg = 1f;
        public float timeToEnd;
        public WaveConfig waveConfig;
        public WaveConfig[] randomWaveConfigs;
        public bool isRandomWaveConfig;

        public GateEntity[] Gates { get; private set; }
        public Action OnWaveForceStop { get; set; }
        public bool WaveEndedCompletely { get; set; }
        
        public void SetupWave(GateEntity gatePrefab, TowerEntity[] towers, float levelExpRatio, float levelDarkRatio, Action onWaveForceEnded)
        {
            if (isRandomWaveConfig)
                waveConfig = randomWaveConfigs[Random.Range(0, randomWaveConfigs.Length)];
            DebugUtility.LogError($"Setup wave {waveConfig.name}");
            Gates = new GateEntity[waveConfig.gateConfigs.Count];
            for (var i = 0; i < waveConfig.gateConfigs.Count; i++)
            {
                var gateCfg = waveConfig.gateConfigs[i];
                Gates[i] = Object.Instantiate(gatePrefab, gateCfg.position, quaternion.identity, null);
                Gates[i].Initialize(gateCfg, gateCfg.targetBaseIndex.Select((index) => towers[index]).ToArray(), scaleHp, scaleDmg, levelExpRatio, levelDarkRatio);
            }

            WaveEndedCompletely = false;
            OnWaveForceStop = onWaveForceEnded;
        }

        public void ActivateWave()
        {
            DebugUtility.LogError($"Activate wave {waveIndex}");
            
            foreach (var gate in Gates)
            {
                gate.Activate();
                gate.OnAllEnemiesDead += CheckStopAllGate;
            }
        }
        
        public IEnumerator IEActivateWave()
        {
            ActivateWave();
            
            yield return new WaitForSeconds(timeToEnd);
            DebugUtility.LogError($"Wave {waveIndex}: End duration");
            CheckStopAllGate();
        }

        private void CheckStopAllGate()
        {
            if (Gates.All((gate) => gate.AllEnemyDead))
            {
                DebugUtility.LogError($"Stop wave {waveIndex}: All enemies are dead");
                WaveEndedCompletely = true;
                OnWaveForceStop?.Invoke();
                OnWaveForceStop = null;
            }
        }
    }
}