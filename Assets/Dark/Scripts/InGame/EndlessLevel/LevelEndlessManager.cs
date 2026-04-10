using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data;
using DG.Tweening;
using Economic;
using Economic.InGame.DropItems;
using Sirenix.OdinInspector;
using UnityEngine;

namespace InGame.EndlessLevel
{
    public class LevelEndlessManager : SerializedMonoBehaviour
    {
        public static int passedWave;

        public PoolWaveEndless wavePool;
        
        [Space]
        [SerializeField] private LevelEndlessManifest levelManifest;
        [SerializeField] private float delayStartLevel = 2.5f;
        [SerializeField] private float defaultWaveDuration = 100f;
        [SerializeField] private bool autoStartOnLoad = true;
        
        public LevelManager levelManager;
        public BackgroundSpawner backgroundSpawner;
        private MapFollower mapFollower;
        
        private LevelEndlessConfig currentLevel;
        private int currentWaveIndex;
        private Coroutine levelCoroutine;
        private EndlessWaveRuntime currentWaveRuntime;

        public TowerEntity[] allTowers;

        public int CurrentBackgroundIndex { get; set; }

        private bool hasStartLevel;
        private bool isLevelEnded;
        private bool isBossing;
        private int highestWavePassed;
        private List<int> mapRandomGroup3;

        public static event Action<int> OnStartWave;
        public static event Action<float> OnStartHideMap;
        public static event Action<float> OnStartShowMap;
        public static event Action<int, int> OnChangeMap;
        public static bool IsPlayingNewWave { get; private set; }

        private void Awake()
        {
            wavePool.Init();
            mapFollower = new MapFollower() { isActiveFollowers = false };
        }

        private void OnDestroy()
        {
            OnStartWave = null;
            OnChangeMap = null;
        }

        private void Update()
        {
            if (!mapFollower.isActiveFollowers) return;
            mapFollower.Update(Time.deltaTime);
        }

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
            isLevelEnded = false;
            isBossing = false;
            passedWave = 0;
            highestWavePassed = PlayerDataManager.Instance.Data.endlessWavePassed;
            CheckNewWave();

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
            
            var maxHp = LevelUtilityV2.GetBaseTowerHp();
            var maxShield = LevelUtilityV2.GetBaseTowerShield();

            while (true)
            {
                var waveTemplate = GetWaveTemplateForIndex(currentWaveIndex);
                if (!IsPlayingNewWave)
                {
                    waveTemplate.sigils = 0;
                    waveTemplate.ashes = 0;
                }
                
                if (waveTemplate == null)
                {
                    Debug.LogWarning("[LevelEndlessManager] Wave template is null, stopping endless loop.");
                    yield break;
                }

                var lastMapId = CurrentBackgroundIndex;
                var shouldInvokeShowMap = false;
                if (!hasStartLevel)
                    backgroundSpawner?.Spawn(UpdateBackgroundIndex(waveTemplate));
                else
                {
                    UpdateBackgroundIndex(waveTemplate);
                    if (lastMapId != CurrentBackgroundIndex)
                    {
                        if (lastMapId == 1)
                        {
                            var durationAnim = 4f;
                            backgroundSpawner?.CurrentBackground?.bg.PlayTransition();
                            if (backgroundSpawner && backgroundSpawner.CurrentBackground != null)
                            {
                                mapFollower.boneFollowers =
                                    backgroundSpawner.CurrentBackground.bg.GetTransitionBoneFollowers();
                            }
                            if (allTowers is { Length: > 2 })
                            {
                                mapFollower.towers = new[]
                                {
                                    allTowers[1].transform,
                                    allTowers[2].transform
                                };
                            }

                            mapFollower.isActiveFollowers = true;
                            mapFollower.Start(levelManager.Player.transform, levelManager.CurrentTower.Id);
                            yield return new WaitForSeconds(durationAnim);
                            mapFollower.isActiveFollowers = false;
                        }
                        
                        var durationHideMap = 1f;
                        OnStartHideMap?.Invoke(durationHideMap);
                        shouldInvokeShowMap = true;
                        yield return new WaitForSeconds(durationHideMap);
                        OnChangeMap?.Invoke(lastMapId, CurrentBackgroundIndex);
                        yield return backgroundSpawner?.IETransition(CurrentBackgroundIndex);
                    }
                    else
                    {
                        backgroundSpawner.CurrentBackground.bg.Reset();
                    }
                }
                
                var waveConfig = GetRandomWaveConfig(waveTemplate);
                if (waveConfig == null)
                {
                    Debug.LogWarning("[LevelEndlessManager] Wave config is null, stopping endless loop.");
                    yield break;
                }

                OnStartWave?.Invoke(currentWaveIndex);
                if (waveTemplate.waveType == WaveEndlessType.Boss) levelManager.OnBossWaveStart?.Invoke();

                Vector2[] towerPositions;
                if (currentLevel.towerPositionsMap == null ||
                    !currentLevel.towerPositionsMap.TryGetValue(CurrentBackgroundIndex, out towerPositions))
                {
                    towerPositions = null;
                }
                for (var i = 0; i < allTowers.Length; i++)
                {
                    if (towerPositions != null && i < towerPositions.Length)
                        allTowers[i].transform.position = towerPositions[i];
                    else if (waveConfig.towerPositions != null && i < waveConfig.towerPositions.Length)
                            allTowers[i].transform.position = waveConfig.towerPositions[i];
                    
                    // Giữ nguyên lượng hp hiện tại
                    allTowers[i].Initialize(i, maxHp, allTowers[i].CurrentHp, maxShield, allTowers[i].shield.CurrentShield);
                    allTowers[i].OnDestroyed += levelManager.OnTowerDestroyed;
                    if (i >= 3 && CurrentBackgroundIndex == 0)
                    {
                        allTowers[i].gameObject.SetActive(false);   
                    }
                    else
                    {
                        allTowers[i].gameObject.SetActive(true);
                        allTowers[i].ForceAutoRegen();
                    }
                }

                // Nếu từ wave 4 trụ về wave 3 trụ thì clamp player về 1, 2, 3
                var towerIndexReset = levelManager.CurrentTower.Id;
                if (CurrentBackgroundIndex == 0)
                {
                    if (levelManager.CurrentTower.Id == 3)
                        towerIndexReset = 2;
                }

                levelManager.TeleportTower(towerIndexReset);
                levelManager.Player.transform.position = levelManager.CurrentTower.transform.position +
                                                         levelManager.CurrentTower.GetTowerHeight();
                levelManager.Player.ShowShotRadius(levelManager.CurrentTower.GetBaseCenter(),
                    LevelUtilityV2.GetNormalAttackRange(Vector2.right));
                
                isBossing = waveTemplate.waveType == WaveEndlessType.Boss;
                
                currentWaveRuntime = new EndlessWaveRuntime(
                    currentWaveIndex,
                    waveTemplate,
                    waveConfig,
                    levelManager.Towers,
                    waveTemplate.timeToEnd,
                    (int waveIndex, WaveEndReason reason) =>
                    {
                        if (waveTemplate.sigils > 0)
                        {
                            EItemDropManager.Instance.AddCollectedData(WealthType.Sigils, waveTemplate.sigils);
                            EItemDropManager.Instance.CollectedData.ClaimSigils();
                        }

                        if (waveTemplate.ashes > 0)
                        {
                            EItemDropManager.Instance.AddCollectedAshes(waveTemplate.ashes);
                            EItemDropManager.Instance.CollectedData.ClaimAshes();
                        }
                        
                        OnWaveForceStop(waveIndex, reason);
                    });

                currentWaveRuntime.SetupWave();

                // Delay start level, nhưng phải setup wave được để hiện map trước
                if (!hasStartLevel)
                {
                    hasStartLevel = true;
                    yield return new WaitForSeconds(delayStartLevel);
                }
                else
                {
                    if (shouldInvokeShowMap)
                    {
                        var durationShowMap = 0.5f;
                        OnStartShowMap?.Invoke(durationShowMap);
                        yield return new WaitForSeconds(durationShowMap);
                    }
                }
                
                yield return currentWaveRuntime.IEActivateWave();

                currentWaveIndex++;
                passedWave += 1;
                PlayerDataManager.Instance.UpdateEndlessWave(passedWave, false);
                CheckNewWave();
            }
        }

        private WaveEndlessInfo GetWaveTemplateForIndex(int waveIndex)
        {
            if (currentLevel == null || currentLevel.waveInfo == null || currentLevel.waveInfo.Length == 0)
                return null;

            var clampedIndex = Mathf.Abs(waveIndex) % currentLevel.waveInfo.Length;
            var waveInfo = currentLevel.waveInfo[clampedIndex];
            return new WaveEndlessInfo()
            {
                scaleHp = waveInfo.scaleHp,
                scaleDmg = waveInfo.scaleDmg,
                scaleSpe = waveInfo.scaleSpe,
                expRatio = waveInfo.expRatio,
                darkRatio = waveInfo.darkRatio,
                darkUnitValue = waveInfo.darkUnitValue,
                sigils = waveInfo.sigils,
                ashes = waveInfo.ashes,
                timeToEnd = waveInfo.timeToEnd,
                waveType = waveInfo.waveType,
                changeToMap = waveInfo.changeToMap
            };
        }

        private int UpdateBackgroundIndex(WaveEndlessInfo waveInfo)
        {
            mapRandomGroup3 ??= new List<int>();
            
            switch (waveInfo.changeToMap)
            {
                case 0: return CurrentBackgroundIndex;
                case 1:
                    CurrentBackgroundIndex = 0;
                    break;
                case 2:
                    CurrentBackgroundIndex = 1;
                    break;
                case 3:
                    CurrentBackgroundIndex = 2;
                    break;
                case -1:
                    var posible = new List<int>();
                    for (var i = 0; i < 3; i++)
                    {
                        if (mapRandomGroup3.Contains(i)) continue;
                        posible.Add(i);
                    }

                    if (posible.Count == 0)
                    {
                        CurrentBackgroundIndex = mapRandomGroup3[RandomUtil.Range(0, mapRandomGroup3.Count)];
                        mapRandomGroup3.Clear();
                    }
                    else
                    {
                        CurrentBackgroundIndex = posible[RandomUtil.Range(0, posible.Count)];
                    }
                    
                    mapRandomGroup3.Add(CurrentBackgroundIndex);
                    break;
            }

            return CurrentBackgroundIndex;
        }

        private WaveEndlessConfig GetRandomWaveConfig(WaveEndlessInfo info)
        {
            if (!wavePool) return null;
            if (info == null) return null;

            return wavePool.GetRandomWave(CurrentBackgroundIndex, info.waveType);
        }

        private void OnWaveForceStop(int waveIndex, WaveEndReason reason)
        {
            if (isLevelEnded) return;
            
            if (waveIndex < currentWaveIndex) return;
            
            // In endless mode we simply start the next wave immediately.
            if (levelCoroutine != null)
                StopCoroutine(levelCoroutine);

            currentWaveIndex++;
            passedWave += 1;
            PlayerDataManager.Instance.UpdateEndlessWave(passedWave, false);
            CheckNewWave();
            levelCoroutine = StartCoroutine(IELevelLoop());
        }

        public void EndLevel()
        {
            isLevelEnded = true;
            if (levelCoroutine != null)
                StopCoroutine(levelCoroutine);
            
            PlayerDataManager.Instance.UpdateEndlessWave(passedWave, true);
        }

        private void CheckNewWave()
        {
            IsPlayingNewWave = currentWaveIndex >= highestWavePassed;
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
        
        private class MapFollower
        {
            public Transform[] towers;
            public Transform[] boneFollowers;
            public bool isActiveFollowers;

            private List<Vector3> cacheBoneFollowerPositions;
            private Transform _player;
            private int indexPlayerTower;
            
            public void Start(Transform player, int currentStandingTower)
            {
                if (!isActiveFollowers) return;
                if (boneFollowers == null) return;
                cacheBoneFollowerPositions = new List<Vector3>();
                foreach (var t in boneFollowers)
                {
                    cacheBoneFollowerPositions.Add(t.position);
                }

                _player = player;
                indexPlayerTower = currentStandingTower - 1;
            }
            
            public void Update(float dt)
            {
                if (!isActiveFollowers) return;
                if (towers == null) return;
                if (boneFollowers == null) return;
                if (towers.Length != boneFollowers.Length) return;
                
                if (_player && indexPlayerTower >= 0 && indexPlayerTower < boneFollowers.Length)
                {
                    _player.position += boneFollowers[indexPlayerTower].position - cacheBoneFollowerPositions[indexPlayerTower];
                }
                
                for (var i = 0; i < towers.Length; i++)
                {
                    towers[i].transform.position += boneFollowers[i].position - cacheBoneFollowerPositions[i];
                    cacheBoneFollowerPositions[i] = boneFollowers[i].position;
                }
            }
        }
    }
}