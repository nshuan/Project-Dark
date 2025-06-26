using System;
using System.Collections;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Unity.Mathematics;
using UnityEngine;

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
        
        public void SetupWave(GateEntity gatePrefab, TowerEntity[] towers)
        {
            Gates = new GateEntity[waveConfig.gateConfigs.Length];
            for (var i = 0; i < waveConfig.gateConfigs.Length; i++)
            {
                var gateCfg = waveConfig.gateConfigs[i];
                Gates[i] = GameObject.Instantiate(gatePrefab, gateCfg.position, quaternion.identity, null);
                Gates[i].Initialize(gateCfg, towers[gateCfg.targetBaseIndex], scaleHp, scaleDmg);
            }
        }

        public void ActivateWave()
        {
            foreach (var gate in Gates)
            {
                gate.Activate();
            }
        }
        
        public IEnumerator IEActivateWave()
        {
            ActivateWave();
            
            yield return new WaitForSeconds(timeToEnd);
            StopWave();
        }

        public void StopWave()
        {
            foreach (var gate in Gates)
            {
                gate.Deactivate();
            }
        }
    }
}