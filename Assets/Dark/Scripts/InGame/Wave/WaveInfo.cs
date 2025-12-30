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
        public float scaleSpeed = 1f;
        public float timeToEnd = 1f;
        public float expRatio = 1f;
        public float darkRatio = 1f;
        public int darkUnitValue = 1;
        public WaveConfig waveConfig;
        public WaveConfig[] randomWaveConfigs;
        public bool isRandomWaveConfig;

        public GateEntity[] Gates { get; private set; }
        public Action<int, WaveEndReason> OnWaveForceStop { get; set; }
        public bool WaveEndedCompletely { get; set; }
        private int currentGateIndex = 0;
        
        public void SetupWave(GateEntity gatePrefab, TowerEntity[] towers, Action<int, WaveEndReason> onWaveForceEnded)
        {
            if (isRandomWaveConfig)
                waveConfig = randomWaveConfigs[RandomUtil.Range(0, randomWaveConfigs.Length)];
            DebugUtility.LogError($"Setup wave {waveConfig.name}");
            Gates = new GateEntity[waveConfig.gateConfigs.Count];
            var waveStatsScale = new WaveStatsScale()
            {
                hpScale = scaleHp,
                dmgScale = scaleDmg,
                speScale = scaleSpeed,
            };
            for (var i = 0; i < waveConfig.gateConfigs.Count; i++)
            {
                var gateCfg = waveConfig.gateConfigs[i];
                Gates[i] = Object.Instantiate(gatePrefab, gateCfg.position, quaternion.identity, null);
                Gates[i].Initialize(gateCfg, gateCfg.targetBaseIndex.Select((index) => towers[index]).ToArray(), waveStatsScale, expRatio, darkRatio, darkUnitValue);
            }
            
            Gates.Sort((gate1, gate2) => gate1.config.startTime.CompareTo(gate2.config.startTime));

            currentGateIndex = 0;
            WaveEndedCompletely = false;
            OnWaveForceStop = onWaveForceEnded;
        }

        public void ActivateWave()
        {
            DebugUtility.LogError($"Activate wave {waveIndex + 1}");

            for (var i = 0; i < Gates.Length; i++)
            {
                var gate = Gates[i];
                var a = i;
                gate.onActivated += () => {
                {
                    currentGateIndex = a;
                    CombatActions.OnGateActivated?.Invoke(gate, waveIndex, currentGateIndex);
                    GateManager.Instance.AddGate(gate);
                }};
                gate.Activate();
                gate.OnAllEnemiesDead += () => { OnStopGate(a); };
            }
        }
        
        public IEnumerator IEActivateWave()
        {
            ActivateWave();
            
            yield return new WaitForSeconds(timeToEnd);
            DebugUtility.LogError($"Wave {waveIndex + 1}: End duration");
            CheckStopAllGate();
        }
        
        private void CheckStopAllGate()
        {
            if (Gates.All((gate) => gate.AllEnemyDead))
            {
                DebugUtility.LogError($"Stop wave {waveIndex + 1}: All enemies are dead");
                WaveEndedCompletely = true;
                OnWaveForceStop?.Invoke(waveIndex, WaveEndReason.AllDead);
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

        public bool IsBossWave => waveConfig.gateConfigs.Any((gate) => gate.isBossGate);
    }

    public enum WaveEndReason
    {
        EndTime,
        AllDead
    }

    public class WaveStatsScale
    {
        public float hpScale = 1f;
        public float dmgScale = 1f;
        public float speScale = 1f;
    }
}