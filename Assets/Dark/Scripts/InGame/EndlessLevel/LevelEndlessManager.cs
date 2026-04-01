using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using InGame;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities;
using UnityEngine;

namespace InGame.EndlessLevel
{
    public class LevelEndlessManager : SerializedMonoBehaviour
    {
        public static int passedWave;
        
        [SerializeField] private LevelEndlessManifest levelManifest;
        [SerializeField] private float delayStartLevel = 2.5f;
        [SerializeField] private float defaultWaveDuration = 100f;
        [SerializeField] private bool autoStartOnLoad = true;
        
        public LevelManager levelManager;
        public BackgroundSpawner backgroundSpawner;
        
        private LevelEndlessConfig currentLevel;
        private int currentWaveIndex;
        private Coroutine levelCoroutine;
        private EndlessWaveRuntime currentWaveRuntime;

        public TowerEntity[] allTowers;

        private bool hasStartLevel;

        public void LoadLevel(LevelEndlessConfig level)
        {
            if (level == null)
            {
                Debug.LogError("[LevelEndlessManager] Tried to load a null level config.");
                return;
            }

            currentLevel = level;
            currentWaveIndex = 0;
            hasStartLevel = false;

            if (levelCoroutine != null)
                StopCoroutine(levelCoroutine);

            if (autoStartOnLoad)
                levelCoroutine = StartCoroutine(IELevelLoop());
        }

        private IEnumerator IELevelLoop()
        {
            if (currentLevel == null || currentLevel.waveInfo == null || currentLevel.waveInfo.Length == 0)
            {
                Debug.LogError("[LevelEndlessManager] Current level has no wave info.");
                yield break;
            }
            
            while (true)
            {
                var waveTemplate = GetWaveTemplateForIndex(currentWaveIndex);
                if (waveTemplate == null)
                {
                    Debug.LogWarning("[LevelEndlessManager] Wave template is null, stopping endless loop.");
                    yield break;
                }

                var waveConfig = GetRandomWaveConfig(waveTemplate);
                if (waveConfig == null)
                {
                    Debug.LogWarning("[LevelEndlessManager] Wave config is null, stopping endless loop.");
                    yield break;
                }

                backgroundSpawner?.Spawn(waveConfig.backgroundIndex);
                
                for (var i = 0; i < allTowers.Length; i++)
                {
                    if (waveConfig.towerPositions != null && i < waveConfig.towerPositions.Length)
                        allTowers[i].transform.position = waveConfig.towerPositions[i];
                    allTowers[i].Initialize(i, LevelUtilityV2.GetBaseTowerHp());
                    allTowers[i].OnDestroyed += levelManager.OnTowerDestroyed;
                    if (i >= 3 && (waveConfig.mapType == LevelMapType.ThreeTowers ||
                                   waveConfig.mapType == LevelMapType.ThreeTowersSquare))
                    {
                        allTowers[i].gameObject.SetActive(false);   
                    }
                    else
                    {
                        allTowers[i].gameObject.SetActive(true);
                    }
                }

                // Nếu từ wave 4 trụ về wave 3 trụ thì clamp player về 1, 2, 3
                var towerIndexReset = levelManager.CurrentTower.Id;
                if (waveConfig.mapType == LevelMapType.ThreeTowers ||
                    waveConfig.mapType == LevelMapType.ThreeTowersSquare)
                {
                    if (levelManager.CurrentTower.Id == 3)
                        towerIndexReset = 2;

                }
                levelManager.TeleportTower(towerIndexReset);
                levelManager.Player.transform.position = levelManager.CurrentTower.transform.position +
                                                         levelManager.CurrentTower.GetTowerHeight();
                levelManager.Player.ShowShotRadius(levelManager.CurrentTower.GetBaseCenter(),
                    LevelUtilityV2.GetNormalAttackRange(Vector2.right));
                
                currentWaveRuntime = new EndlessWaveRuntime(
                    currentWaveIndex,
                    waveTemplate,
                    waveConfig,
                    levelManager.Towers,
                    defaultWaveDuration,
                    OnWaveForceStop);

                currentWaveRuntime.SetupWave();

                // Delay start level, nhưng phải setup wave được để hiện map trước
                if (!hasStartLevel)
                {
                    hasStartLevel = true;
                    yield return new WaitForSeconds(delayStartLevel);
                }
                
                yield return currentWaveRuntime.IEActivateWave();

                currentWaveIndex++;
                passedWave += 1;
            }
        }

        private WaveEndlessInfo GetWaveTemplateForIndex(int waveIndex)
        {
            if (currentLevel == null || currentLevel.waveInfo == null || currentLevel.waveInfo.Length == 0)
                return null;

            var clampedIndex = Mathf.Abs(waveIndex) % currentLevel.waveInfo.Length;
            return currentLevel.waveInfo[clampedIndex];
        }

        private WaveEndlessConfig GetRandomWaveConfig(WaveEndlessInfo info)
        {
            if (info == null || info.wavePool == null || info.wavePool.allWaves == null ||
                info.wavePool.allWaves.Length == 0)
                return null;

            var allWaves = info.wavePool.allWaves;
            var randomIndex = RandomUtil.Range(0, allWaves.Length);
            return allWaves[randomIndex];
        }

        private void OnWaveForceStop(int waveIndex, WaveEndReason reason)
        {
            // In endless mode we simply start the next wave immediately.
            if (levelCoroutine != null)
                StopCoroutine(levelCoroutine);

            passedWave += 1;
            levelCoroutine = StartCoroutine(IELevelLoop());
        }

        /// <summary>
        /// Runtime helper that mirrors the behaviour of WaveInfo,
        /// but works with WaveEndlessConfig and WaveEndlessInfo.
        /// </summary>
        private class EndlessWaveRuntime
        {
            private readonly int waveIndex;
            private readonly WaveEndlessInfo waveInfo;
            private readonly WaveEndlessConfig waveConfig;
            private readonly TowerEntity[] towers;
            private readonly float waveDuration;
            private readonly Action<int, WaveEndReason> onWaveForceStop;

            private GateEntity[] gates;
            private Dictionary<GateEntity, GameObject> vfxPreOpen;

            public EndlessWaveRuntime(
                int waveIndex,
                WaveEndlessInfo waveInfo,
                WaveEndlessConfig waveConfig,
                TowerEntity[] towers,
                float waveDuration,
                Action<int, WaveEndReason> onWaveForceStop)
            {
                this.waveIndex = waveIndex;
                this.waveInfo = waveInfo;
                this.waveConfig = waveConfig;
                this.towers = towers;
                this.waveDuration = waveDuration;
                this.onWaveForceStop = onWaveForceStop;
            }

            public void SetupWave()
            {
                if (waveConfig == null || waveConfig.gateConfigs == null || waveConfig.gateConfigs.Count == 0)
                    return;

                DebugUtility.LogError($"[Endless] Setup wave {waveConfig.name}");

                gates = new GateEntity[waveConfig.gateConfigs.Count];
                vfxPreOpen = new Dictionary<GateEntity, GameObject>();

                var statsScale = new WaveStatsScale
                {
                    hpScale = waveInfo.scaleHp,
                    dmgScale = waveInfo.scaleDmg,
                    speScale = waveInfo.scaleSpe
                };

                for (var i = 0; i < waveConfig.gateConfigs.Count; i++)
                {
                    var gateCfg = waveConfig.gateConfigs[i];
                    var gatePrefab = gateCfg.gatePrefab ? gateCfg.gatePrefab : GateManifest.Get(0);

                    gates[i] = UnityEngine.Object.Instantiate(
                        gatePrefab,
                        gateCfg.position,
                        Unity.Mathematics.quaternion.identity,
                        null);

                    gates[i].Initialize(
                        gateCfg,
                        gateCfg.targetBaseIndex.Select(index =>
                        {
                            if (index >= 0 && index < towers.Length)
                                return towers[index];
                            return towers[^1];
                        }).ToArray(),
                        statsScale,
                        waveInfo.expRatio,
                        waveInfo.darkRatio,
                        waveInfo.darkUnitValue);

                    gates[i].gameObject.SetActive(false);
                    vfxPreOpen[gates[i]] = gates[i].vfxPreOpen;
                }

                Array.Sort(gates, (g1, g2) => g1.config.startTime.CompareTo(g2.config.startTime));
            }

            public IEnumerator IEActivateWave()
            {
                if (gates == null || gates.Length == 0)
                    yield break;

                ActivateWave();
                yield return new WaitForSeconds(waveDuration);

                CheckStopAllGate();
            }

            private void ActivateWave()
            {
                DebugUtility.LogError($"[Endless] Activate wave {waveIndex + 1}");

                for (var i = 0; i < gates.Length; i++)
                {
                    var gate = gates[i];
                    var localGateIndex = i;

                    gate.onActivated += () =>
                    {
                        CombatActions.OnGateActivated?.Invoke(gate, waveIndex, localGateIndex);
                        GateManager.Instance.AddGate(gate);
                    };

                    gate.gameObject.SetActive(true);
                    gate.Activate();

                    gate.OnAllEnemiesDead += () => { OnStopGate(localGateIndex); };
                }
            }

            private void CheckStopAllGate()
            {
                var bossGate = gates.FirstOrDefault(g => g.config.isBossGate);
                if (bossGate && bossGate.AllEnemyDead)
                {
                    DebugUtility.LogError($"[Endless] Stop wave {waveIndex + 1}: Boss is dead");
                    onWaveForceStop?.Invoke(waveIndex, WaveEndReason.AllDead);
                    return;
                }

                if (gates.All(g => g.AllEnemyDead))
                {
                    DebugUtility.LogError($"[Endless] Stop wave {waveIndex + 1}: All enemies are dead");
                    onWaveForceStop?.Invoke(waveIndex, WaveEndReason.AllDead);
                }
            }

            private void OnStopGate(int index)
            {
                var reduceStartTime = 0f;
                if (index + 1 < gates.Length)
                    reduceStartTime = gates[index + 1].config.startTime;

                for (var i = index + 1; i < gates.Length; i++)
                {
                    gates[i].ForceRestartGate(reduceStartTime);
                }

                CheckStopAllGate();
            }
        }
    }
}