using System;
using System.Collections;
using System.Collections.Generic;
using Core;
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
using Unity.Mathematics;
using UnityEngine;
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
        public GateEntity gatePrefab;
        
        [SerializeField] private TowerEntity[] towers;
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

        #region Upgrade

        [ReadOnly, NonSerialized, OdinSerialize] private UpgradeBonusInfoV2 bonusInfo = new UpgradeBonusInfoV2();
        
        #endregion
        
        #region Action

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
            InitPlayerAndTowers();
            
#if UNITY_EDITOR

            if (isLoadFromInit == false && autoLoadLevel && Level == null)
            {
                this.DelayCall(2f, () => LoadLevel(testLevel));
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
            LevelUtilityV2.StatsNormalPiercing = PlayerSkillNormalManifest.Get(NormalType.Piercing);
            LevelUtilityV2.StatsNormalBullet = PlayerSkillNormalManifest.Get(NormalType.Bullet);
            LevelUtilityV2.StatsChargeBullet = PlayerChargeManifest.Get(ChargeType.Bullet);
            LevelUtilityV2.StatsChargeSize = PlayerChargeManifest.Get(ChargeType.Size);
            LevelUtilityV2.StatsDash = dashConfig;
            LevelUtilityV2.StatsFlash = flashConfig;
            LevelUtilityV2.StatsTele = defaultTeleConfig; 
            LevelUtilityV2.StatsCounterPiercing = TowerCounterManifest.Get(NodeTowerCounter.CounterType.Pierce);
            LevelUtilityV2.StatsCounterSlash = TowerCounterManifest.Get(NodeTowerCounter.CounterType.Slash);
        }
        
        private void InitPlayerAndTowers()
        {
            InitTowers();
            currentTowerIndex = -1;
            
            if (Player != null) Destroy(Player.gameObject);
            Player = playerSpawner.SpawnCharacter((CharacterClass.CharacterClass)LevelUtilityV2.StatsNormalAttack.skillId);
            Player.transform.position = towers[0].transform.position + towers[0].GetTowerHeight();
            OnInitPlayer?.Invoke();
        }
        
        public void LoadLevel(int level)
        {
            var levelConfig = LevelManifest.Instance.GetLevel(level);
            if (levelConfig == null) return;
            LoadLevel(levelConfig);
        }
        
        public void LoadLevel(LevelConfig level)
        {
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
            
            waveCoroutine = StartCoroutine(IEWave(Level.waveInfo));
            LevelStarted = true;
            OnLevelLoaded?.Invoke(Level);
            StartTimer();
        }
        
        public void WinLevel()
        {
            if (IsEndLevel) return;
            
            StopTimer();
            
            WealthManager.Instance.Save();
            PlayerDataManager.Instance.CompleteLevel();
            
            DebugUtility.LogError($"Level {Level.level + 1} is ended: WIN");
            IsEndLevel = true;
            OnWin?.Invoke();
            ClearAction();
        }

        public void LoseLevel()
        {
            if (IsEndLevel) return;
            
            StopTimer();
            
            if (waveCoroutine != null) StopCoroutine(waveCoroutine);
            
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
                currentWave.SetupWave(gatePrefab, Towers, OnWaveForceStop);
                OnWaveStart?.Invoke(currentWaveIndex, currentWave.timeToEnd);
                if (currentWave.IsBossWave) OnBossWaveStart?.Invoke();
                currentWaveIndex += 1;
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
            onWaveEnded?.Invoke(currentWaveIndex - 1, reason);
            
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
            for (var i = 0; i < towers.Length; i++)
            {
                towers[i].Initialize(i, LevelUtilityV2.GetBaseTowerHp());
                towers[i].OnDestroyed += OnTowerDestroyed;
            }
        }

        private void OnTowerDestroyed(TowerEntity tower)
        {
            Debug.LogError($"Tower {tower.name} is destroyed");
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
            LoadLevel(testLevel);
        }
    }
}