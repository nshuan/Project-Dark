using System;
using System.Collections;
using Core;
using Data;
using Economic;
using InGame.ConfigManager;
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
        public MoveTowersConfig shortTeleConfig;
        public MoveTowersConfig longTeleConfig;

        [SerializeField] private PlayerSpawner playerSpawner;
        [SerializeField] private GateEntity gatePrefab;
        
        [SerializeField] private TowerEntity[] towers;
        public TowerEntity[] Towers => towers;
        private int currentTowerIndex;
        public TowerEntity CurrentTower
        {
            get
            {
                currentTowerIndex = Math.Clamp(currentTowerIndex, 0, towers.Length);
                return towers[currentTowerIndex];
            }
        }
        
        private PlayerSkillConfig skillConfig;
        public PlayerSkillConfig SkillConfig => skillConfig;
   
        public LevelConfig Level { get; private set; }
        public PlayerStats PlayerStats => playerStats;
        private bool IsEndLevel { get; set; }
        
        public PlayerCharacter Player { get; set; }

        #region Upgrade

        [ReadOnly, NonSerialized, OdinSerialize] private UpgradeBonusInfo bonusInfo;
        
        #endregion
        
        #region Action

        public Action<LevelConfig> OnLevelLoaded { get; set; }
        public Action<TowerEntity> OnChangeTower { get; set; }

        // <int waveIndex, float waveDuration>
        public event Action<int, float> OnWaveStart;
        public event Action<int> onWaveEnded;
        
        public event Action OnWin;
        public event Action OnLose;

        #endregion
        
        private WinLoseManager winLoseManager;

#if UNITY_EDITOR
        [Space] public bool autoLoadLevel = true;
        private void Start()
        {
            if (autoLoadLevel)
                LoadLevel(testLevel);
        }
#endif

        protected override void OnDestroy()
        {
            base.OnDestroy();

            ClearAction();
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
            UpgradeManager.Instance.ActivateTree(ref bonusInfo);
            LevelUtility.BonusInfo = bonusInfo;
            EnemyManager.Instance.Initialize();
            winLoseManager = new WinLoseManager();
            IsEndLevel = false;
            
            InitTowers();
            TeleportTower(0);

            skillConfig = ClassConfigManifest.GetConfig(PlayerDataManager.Instance.Data.characterClass);
            
            if (Player != null) Destroy(Player.gameObject);
            Player = playerSpawner.SpawnCharacter((CharacterClass.CharacterClass)skillConfig.skillId);
            Player.transform.position = CurrentTower.transform.position + CurrentTower.standOffset;
            
            // Start waves
            currentWaveIndex = 0;
            if (waveCoroutine != null) StopCoroutine(waveCoroutine);
            waveCoroutine = StartCoroutine(IEWave(level.waveInfo));
            
            OnLevelLoaded?.Invoke(level);
        }

        public void WinLevel()
        {
            if (IsEndLevel) return;
            
            WealthManager.Instance.Save();
            
            DebugUtility.LogError($"Level {Level.level} is ended: WIN");
            IsEndLevel = true;
            OnWin?.Invoke();
            ClearAction();
        }

        public void LoseLevel()
        {
            if (IsEndLevel) return;
            
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
            onWaveEnded = null;
        }
        
        #region Waves

        // Start from 0
        private int currentWaveIndex;
        private Coroutine waveCoroutine;
        private IEnumerator IEWave(WaveInfo[] waves)
        {
            if (waves == null || waves.Length == 0) yield break;
            yield return new WaitForEndOfFrame();

            while (currentWaveIndex < waves.Length)
            {
                var currentWave = waves[currentWaveIndex];
                currentWave.SetupWave(gatePrefab, Towers, Level.levelExpRatio, Level.levelDarkRatio, OnWaveForceStop);
                OnWaveStart?.Invoke(currentWaveIndex, currentWave.timeToEnd);
                currentWaveIndex += 1;
                yield return currentWave.IEActivateWave();
                onWaveEnded?.Invoke(currentWaveIndex - 1);
            }
        }

        private void OnWaveForceStop()
        {
            if (waveCoroutine != null) StopCoroutine(waveCoroutine);
            onWaveEnded?.Invoke(currentWaveIndex - 1);
            winLoseManager.CheckWin(this);
            waveCoroutine = StartCoroutine(IEWave(Level.waveInfo));
        }

        #endregion
        
        #region Towers

        private void InitTowers()
        {
            for (var i = 0; i < towers.Length; i++)
            {
                towers[i].Initialize(i, playerStats.hp);
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

        public LevelConfig testLevel;
        [Button]
        public void TestLoadLevel()
        {
            LoadLevel(testLevel);
        }
    }
}