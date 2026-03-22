using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core;
using Dark.Scripts.Leaderboard;
using Dark.Scripts.Settings;
using Dark.Scripts.Utils;
using Data;
using Economic;
using InGame.AttackNormalConfig;
using InGame.ChargeConfig;
using InGame.ConfigManager;
using InGame.CounterConfig;
using InGame.Upgrade;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace InGame
{
    public class LevelManager : SerializedMonoSingleton<LevelManager>
    {
        [SerializeField] private PlayerStats playerStats;
        public MoveTowersConfig defaultTeleConfig;
        public MoveTowersConfig flashConfig;
        public MoveTowersConfig dashConfig;

        [SerializeField] private PlayerSpawner playerSpawner;
        [SerializeField] private BackgroundSpawner backgroundSpawner;
        public GateEntity gatePrefab;
        
        [SerializeField] private TowerEntity[] towers;
        public TowerEntity FirstDestroyedTower { get; set; }
        public float delayStartLevel = 2.5f;
        public TowerEntity[] Towers => towers;
        private int currentTowerIndex;
        public TowerEntity CurrentTower
        {
            get
            {
                currentTowerIndex = Math.Clamp(currentTowerIndex, 0, towers.Length - 1);
                return towers[currentTowerIndex];
            }
        }

        public bool LevelStarted { get; private set; } = false;
        public LevelConfig Level { get; private set; }
        private bool IsEndLevel { get; set; }
        public string LevelBossName { get; set; }
        
        public PlayerCharacter Player { get; set; }

        private List<LevelMapVariant> mapVarientItems;
        
        #region Upgrade

        [ReadOnly, NonSerialized, OdinSerialize] private UpgradeBonusInfoV2 bonusInfo = new UpgradeBonusInfoV2();
        
        #endregion
        
        #region Action

        public Action OnInitTowers { get; set; }
        public Action OnInitPlayer { get; set; }
        public Action<LevelConfig> OnLevelPreLoaded { get; set; }
        public Action<LevelConfig> OnLevelLoaded { get; set; }
        public Action<TowerEntity> OnChangeTower { get; set; }

        // <int waveIndex, float waveDuration>
        public event Action<int, float> OnWaveStart;
        public event Action OnBossWaveStart;
        public event Action<int, WaveEndReason> onWaveEnded;
        
        public event Action OnWin;
        public event Action OnLose;

        #endregion
        
        private WinLoseManager winLoseManager;

        [Space] public bool autoLoadLevel = true;
        public static bool isLoadFromInit;
        private Coroutine coroutineStartLevel;
        
        private void Start()
        {
            InitSkillTreeBonus();
            InitLevelMapVariant();
            
#if UNITY_EDITOR

            if (isLoadFromInit == false && autoLoadLevel && Level == null)
            {
                this.DelayCall(2f, () => LoadLevel(testLevel.level));
            }
#endif
        }

        protected override void OnDestroy()
        {
            ClearAction();
            base.OnDestroy();
        }

        #region Core

        private void InitSkillTreeBonus()
        {
            UpgradeManager.Instance.ActivateTree(ref bonusInfo);
            LevelUtilityV2.BonusInfo = bonusInfo;
            LevelUtilityV2.StatsBase = playerStats;
            LevelUtilityV2.StatsNormalAttack = ClassConfigManifest.GetConfig(PlayerDataManager.Instance.Data.characterClass);
            LevelUtilityV2.StatsNormalPiercing = PlayerSkillNormalManifest.Get(PlayerDataManager.Instance.Data.Class, NormalType.Piercing);
            LevelUtilityV2.StatsNormalBullet = PlayerSkillNormalManifest.Get(PlayerDataManager.Instance.Data.Class, NormalType.Bullet);
            LevelUtilityV2.StatsChargeBullet = PlayerChargeManifest.Get(PlayerDataManager.Instance.Data.Class, ChargeType.Bullet);
            LevelUtilityV2.StatsChargeSize = PlayerChargeManifest.Get(PlayerDataManager.Instance.Data.Class, ChargeType.Size);
            LevelUtilityV2.StatsDash = dashConfig;
            LevelUtilityV2.StatsFlash = flashConfig;
            LevelUtilityV2.StatsTele = defaultTeleConfig; 
            LevelUtilityV2.StatsCounterPiercing = TowerCounterManifest.Get(NodeTowerCounter.CounterType.Pierce);
            LevelUtilityV2.StatsCounterSlash = TowerCounterManifest.Get(NodeTowerCounter.CounterType.Slash);
        }
        
        private void InitPlayerAndTowers()
        {
            InitTowers();
            OnInitTowers?.Invoke();
            currentTowerIndex = -1;
            
            if (Player != null) Destroy(Player.gameObject);
            Player = playerSpawner.SpawnCharacter((CharacterClass.CharacterClass)PlayerDataManager.Instance.Data.characterClass);
            Player.transform.position = towers[0].transform.position + towers[0].GetTowerHeight();
            OnInitPlayer?.Invoke();
        }

        private void InitLevelMapVariant()
        {
            mapVarientItems = FindObjectsByType<LevelMapVariant>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList();
        }
        
        public void LoadLevel(int level)
        {
            var levelConfig = LevelManifest.Instance.GetLevel(PlayerDataManager.Instance.Data.Class, level);
            if (!levelConfig) return;
            Level = levelConfig;
            backgroundSpawner.Spawn(Level.backgroundIndex);
            SceneManager.sceneLoaded += OnLevelSceneLoaded;
            LoadMapScene(Level.mapType);
        }

        private void OnLevelSceneLoaded(Scene scene, LoadSceneMode loadMode)
        {
            SceneManager.sceneLoaded -= OnLevelSceneLoaded;
            InitPlayerAndTowers();
            LoadLevel(Level, false);
        }
        
        private void LoadMapScene(LevelMapType mapType)
        {
            if (coroutineLoadMap != null) StopCoroutine(coroutineLoadMap);
            var sceneName = mapType switch
            {
                LevelMapType.ThreeTowers => "Level3Towers",
                LevelMapType.FourTowers => "Level4Towers",
                LevelMapType.ThreeTowersSquare => "Level3TowersSquare",
                LevelMapType.FourTowersTriangle =>  "Level4TowersTriangle",
                _ => "Level3Towers"
            };
            coroutineLoadMap = StartCoroutine(IELoadMapScene(sceneName));
        }

        private Coroutine coroutineLoadMap;
        private IEnumerator IELoadMapScene(string sceneName)
        {
            yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }
        
        public void LoadLevel(LevelConfig level, bool overrideLevel = false)
        {
            if (overrideLevel)
                Level = level;
            
            EnemyManager.Instance.Initialize();
            winLoseManager = new WinLoseManager();
            IsEndLevel = false;
            
            TeleportTower(0);
            
            // Get level boss name
            foreach (var waveInfo in Level.waveInfo)
            {
                var foundedBoss = false;
                foreach (var gate in waveInfo.waveConfig.gateConfigs)
                {
                    if (gate.isBossGate)
                    {
                        LevelBossName = gate.spawnType.displayName;
                        foundedBoss = true;
                        break;
                    }
                }
                if (foundedBoss) break;
            }
            
            // Start level
            if (coroutineStartLevel != null) StopCoroutine(coroutineStartLevel);
            coroutineStartLevel = StartCoroutine(IELevel());
        }

        private IEnumerator IELevel()
        {
            // Start waves
            currentWaveIndex = 0;
            if (waveCoroutine != null) StopCoroutine(waveCoroutine);
            
            OnLevelPreLoaded?.Invoke(Level);
            yield return new WaitForSeconds(delayStartLevel);

            foreach (var waveInfo in Level.waveInfo)
            {
                waveInfo.SetupWave(Towers, OnWaveForceStop);
            }
            waveCoroutine = StartCoroutine(IEWave(Level.waveInfo));
            LevelStarted = true;
            OnLevelLoaded?.Invoke(Level);
            StartTimer();
        }
        
        public void WinLevel()
        {
            if (IsEndLevel) return;
            
            StopTimer();
            foreach (var tower in towers)
            {
                tower.Deactivate();
            }
            
            WealthManager.Instance.Save();
            if (Level.level >= PlayerDataManager.Instance.Data.level + 1)
            {
                PlayerDataManager.Instance.CompleteLevel();
                
                // Add score to leaderboard
                if (PlayerDataManager.Instance.Data.level ==
                    LevelManifest.Instance.GetMaxLevel(PlayerDataManager.Instance.Data.Class))
                {
                    var classType = PlayerDataManager.Instance.Data.Class;
                    LeaderboardManager.Instance.GetLeaderboard(classType).UploadScore((int)PlayerDataManager.Instance.Data.timePlayedMilli, new int[] { (int) classType });
                    LeaderboardManager.Instance.GetFullLeaderboard().UploadScore((int)PlayerDataManager.Instance.Data.timePlayedMilli, new int[] { (int) classType });
                }
            }
            
            DebugUtility.LogError($"Level {Level.level + 1} is ended: WIN");
            IsEndLevel = true;
            OnWin?.Invoke();
            ClearAction();
        }

        public void LoseLevel()
        {
            if (IsEndLevel) return;
            
            StopTimer();
            foreach (var tower in towers)
            {
                tower.Deactivate();
            }
            
            if (waveCoroutine != null) StopCoroutine(waveCoroutine);
            foreach (var tower in towers)
            {
                tower.IsDestroyed = true;
            }
            
            WealthManager.Instance.Save();
            
            DebugUtility.LogError($"Level {Level.level} is ended: LOSE");
            IsEndLevel = true;
            OnLose?.Invoke();    
            ClearAction();
        }

        private void ClearAction()
        {
            OnWin = null;
            OnLose = null;
            OnChangeTower = null;
            OnWaveStart = null;
            OnBossWaveStart = null;
            onWaveEnded = null;
            OnInitPlayer = null;
            OnInitTowers = null;
            
            CombatActions.Clear();
        }
        
        #endregion
        
        #region Waves

        // Start from 0
        private int currentWaveIndex;
        public int CurrentWaveIndex => currentWaveIndex;
        private Coroutine waveCoroutine;
        private IEnumerator IEWave(WaveInfo[] waves)
        {
            if (waves == null || waves.Length == 0) yield break;
            yield return new WaitForEndOfFrame();

            while (currentWaveIndex < waves.Length)
            {
                var currentWave = waves[currentWaveIndex];
                OnWaveStart?.Invoke(currentWaveIndex, currentWave.timeToEnd);
                if (currentWave.IsBossWave) OnBossWaveStart?.Invoke();
                currentWaveIndex += 1;
                if (GameSettings.ShowGateWarning && currentWaveIndex < waves.Length)
                    waves[currentWaveIndex].PreOpen();
                yield return currentWave.IEActivateWave();
                onWaveEnded?.Invoke(currentWaveIndex - 1, WaveEndReason.EndTime);
            }
        }

        private void OnWaveForceStop(int waveIndex, WaveEndReason reason)
        {
            // Nếu ko phải wave đang chạy thì ko stop coroutine
            if (waveIndex == currentWaveIndex - 1)
            {
                if (waveCoroutine != null) StopCoroutine(waveCoroutine);
            }

            // Nếu wave stop vì hết thời gian thì invoke hàm này
            // if (reason == WaveEndReason.EndTime)
            onWaveEnded?.Invoke(waveIndex, reason);
            
            if (!IsEndLevel)
                winLoseManager.CheckWin(this);
                
            if (waveIndex == currentWaveIndex - 1)
            {
                waveCoroutine = StartCoroutine(IEWave(Level.waveInfo));
            }
        }

        #endregion
        
        #region Towers

        private void InitTowers()
        {
            towers = FindObjectsByType<TowerEntity>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            towers.Sort((t1, t2) => t1.transform.GetSiblingIndex().CompareTo(t2.transform.GetSiblingIndex()));
            for (var i = 0; i < towers.Length; i++)
            {
                if (Level.towerPositions != null && i < Level.towerPositions.Length)
                    towers[i].transform.position = Level.towerPositions[i];
                towers[i].Initialize(i, LevelUtilityV2.GetBaseTowerHp());
                towers[i].OnDestroyed += OnTowerDestroyed;
            }
        }

        private void OnTowerDestroyed(TowerEntity tower)
        {
            Debug.LogError($"Tower {tower.name} is destroyed");
            FirstDestroyedTower = tower;
            winLoseManager.CheckLose(this);
        }
        
        public void TeleportTower(int towerIndex)
        {
            if (towers[Math.Clamp(towerIndex, 0, towers.Length - 1)].IsDestroyed) return;
            if (towerIndex == currentTowerIndex) return;
            
            for (var i = 0; i < towers.Length; i++)
            {
                if (i == towerIndex)
                {
                    towers[i].EnterTower();
                }
                else towers[i].LeaveTower();
            }
            
            currentTowerIndex = towerIndex;
            OnChangeTower?.Invoke(CurrentTower);
        }

        public void BlockDamageAllTowers()
        {
            if (towers == null) return;
            
            foreach (var tower in towers)
            {
                tower.BlockDamage = true;
            }    
        }
        
        #endregion

        #region Timer

        // Calculate time played

        private Coroutine coroutineTimer;
        private float timePlayedInSec;
        public TimeSpan TimePlayed => TimeSpan.FromSeconds(timePlayedInSec);
        
        private void StartTimer()
        {
            if (coroutineTimer != null) StopCoroutine(coroutineTimer);
            timePlayedInSec = 0f;
            coroutineTimer = StartCoroutine(IELevelTimer());
        }

        private void StopTimer()
        {
            if (coroutineTimer != null) StopCoroutine(coroutineTimer);
        }

        private IEnumerator IELevelTimer()
        {
            while (!IsEndLevel)
            {
                // Should ignore timeScale
                timePlayedInSec += Time.unscaledDeltaTime;
                yield return null;
            }
        }
        
        #endregion
#if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.K)) WinLevel();
            else if (Input.GetKeyDown(KeyCode.L)) LoseLevel();
        }
#endif

        public LevelConfig testLevel;
        [Button]
        public void TestLoadLevel()
        {
            LoadLevel(testLevel.level);
        }
    }
}