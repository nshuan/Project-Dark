using System;
using System.Collections;
using Core;
using Economic;
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
        [SerializeField] private PlayerSkillConfig skillConfig;
        public MoveTowersConfig defaultTeleConfig;
        public MoveTowersConfig shortTeleConfig;
        public MoveTowersConfig longTeleConfig;

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
   
        public LevelConfig Level { get; private set; }
        public PlayerStats PlayerStats => playerStats;
        private bool IsEndLevel { get; set; }

        #region Upgrade

        [ReadOnly, NonSerialized, OdinSerialize] private UpgradeBonusInfo bonusInfo;
        public UpgradeBonusInfo BonusInfo => bonusInfo;

        #endregion
        
        #region Action

        public Action<LevelConfig> OnLevelLoaded { get; set; }
        public Action<PlayerSkillConfig> OnChangeSkill { get; set; }
        public Action<TowerEntity> OnChangeTower { get; set; }

        public event Action<int> OnWaveStart;
        
        public event Action OnWin;
        public event Action OnLose;

        #endregion
        
        private WinLoseManager winLoseManager;
        
        private void Start()
        {
            LoadLevel(testLevel);
        }

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
            
            // Start waves
            currentWaveIndex = 0;
            if (waveCoroutine != null) StopCoroutine(waveCoroutine);
            waveCoroutine = StartCoroutine(IEWave(level.waveInfo));
            
            OnLevelLoaded?.Invoke(level);
            OnChangeSkill?.Invoke(skillConfig);
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
            OnLevelLoaded = null;
            OnChangeSkill = null;
            OnChangeTower = null;
        }
        
        #region Waves

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
                OnWaveStart?.Invoke(currentWaveIndex);
                currentWaveIndex += 1;
                yield return currentWave.IEActivateWave();
            }
        }

        private void OnWaveForceStop()
        {
            if (waveCoroutine != null) StopCoroutine(waveCoroutine);
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


        [Button]
        public void TestSkillChange()
        {
            OnChangeSkill?.Invoke(skillConfig);
        }

        public LevelConfig testLevel;
        [Button]
        public void TestLoadLevel()
        {
            LoadLevel(testLevel);
        }
    }
}