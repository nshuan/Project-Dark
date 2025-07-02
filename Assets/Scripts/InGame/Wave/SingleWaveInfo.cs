using System;
using System.Collections;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Unity.Mathematics;
using UnityEngine;
using Object = UnityEngine.Object;

namespace InGame
{
    [Serializable]
    public class SingleWaveInfo : IWaveInfo
    {
        [field:NonSerialized, OdinSerialize, ReadOnly] public int WaveIndex { get; set; }
        
        public float scaleHp = 1f;
        public float scaleDmg = 1f;
        public float timeToEnd;
        public WaveConfig waveConfig;

        public GateEntity[] Gates { get; private set; }
        public Action OnWaveForceStop { get; set; }
        public bool WaveEndedCompletely { get; set; }
        
        public void SetupWave(GateEntity gatePrefab, TowerEntity[] towers, Action onWaveForceEnded)
        {
            Gates = new GateEntity[waveConfig.gateConfigs.Length];
            for (var i = 0; i < waveConfig.gateConfigs.Length; i++)
            {
                var gateCfg = waveConfig.gateConfigs[i];
                Gates[i] = Object.Instantiate(gatePrefab, gateCfg.position, quaternion.identity, null);
                Gates[i].Initialize(gateCfg, gateCfg.targetBaseIndex.Select((index) => towers[index]).ToArray(), scaleHp, scaleDmg);
            }

            WaveEndedCompletely = false;
            OnWaveForceStop = onWaveForceEnded;
        }

        public void ActivateWave()
        {
            DebugUtility.LogError($"Activate wave {WaveIndex}");
            
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
            DebugUtility.LogError($"Stop wave {WaveIndex}: End duration");
            StopWave();
        }

        public void StopWave()
        {
            foreach (var gate in Gates)
            {
                gate.Deactivate();
            }
        }

        private void CheckStopAllGate()
        {
            if (Gates.All((gate) => gate.IsActive == false))
            {
                DebugUtility.LogError($"Stop wave {WaveIndex}: All enemies are dead");
                WaveEndedCompletely = true;
                OnWaveForceStop?.Invoke();
                OnWaveForceStop = null;
            }
        }
    }
}