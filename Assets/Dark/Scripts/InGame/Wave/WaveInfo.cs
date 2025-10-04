using System;
using System.Collections;
using System.Linq;
using Sirenix.Utilities;
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
        private int currentGateIndex = 0;
        
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
            
            Gates.Sort((gate1, gate2) => gate1.config.startTime.CompareTo(gate2.config.startTime));

            currentGateIndex = 0;
            WaveEndedCompletely = false;
            OnWaveForceStop = onWaveForceEnded;
        }

        public void ActivateWave()
        {
            DebugUtility.LogError($"Activate wave {waveIndex}");

            for (var i = 0; i < Gates.Length; i++)
            {
                var gate = Gates[i];
                var a = i;
                gate.onActivated += () => { currentGateIndex = a; };
                gate.Activate();
                gate.OnAllEnemiesDead += () => { OnStopGate(a); };
            }
        }
        
        public IEnumerator IEActivateWave()
        {
            ActivateWave();
            
            yield return new WaitForSeconds(timeToEnd);
            DebugUtility.LogError($"Wave {waveIndex}: End duration");
            OnWaveForceStop = null;
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

        private void OnStopGate(int index)
        {
            // Nếu mà gate vừa end ko phải gate cuối cùng đã mở thì bỏ qua
            if (index < currentGateIndex)
            {
                CheckStopAllGate();
                return;
            }
            
            var reduceStartTime = 0f;
            if (index + 1 < Gates.Length)
                reduceStartTime = Gates[index + 1].config.startTime;
            
            for (var i = index + 1; i < Gates.Length; i++)
            {
                Gates[i].ForceRestartGate(reduceStartTime);
            }
            
            CheckStopAllGate();
        }
    }
}