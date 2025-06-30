using System;
using System.Collections;
using Core;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;

namespace InGame
{
    public class LevelManager : MonoSingleton<LevelManager>
    {
        [field: SerializeField] public GameStats GameStats { get; private set; }
        [SerializeField] private PlayerStats playerStats;
        [SerializeField] private PlayerSkillConfig skillConfig;

        [SerializeField] private GateEntity gatePrefab;
        
        [SerializeField] private TowerEntity[] towers;
        public TowerEntity[] Towers => towers;
        private int currentTowerIndex = 0;
        public TowerEntity CurrentTower
        {
            get
            {
                currentTowerIndex = Math.Clamp(currentTowerIndex, 0, towers.Length);
                return towers[currentTowerIndex];
            }
        }
        private bool canTeleportTower;
   
        private LevelConfig Level { get; set; }
        public PlayerStats PlayerStats => playerStats;
        
        #region Action

        public Action<LevelConfig> OnLevelLoaded;
        public Action<PlayerSkillConfig> OnChangeSkill;
        public Action<Transform> OnChangeTower;

        #endregion

        private void Start()
        {
            TestLoadLevel();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            OnLevelLoaded = null;
            OnChangeSkill = null;
            OnChangeTower = null;
        }

        public void LoadLevel(LevelConfig level)
        {
            Level = level;
            EnemyManager.Instance.Initialize();
            
            InitTowers();
            TeleportTower(0);
            
            // Start waves
            currentWaveIndex = 0;
            if (waveCoroutine != null) StopCoroutine(waveCoroutine);
            waveCoroutine = StartCoroutine(IEWave(level.waveInfos));
            
            OnChangeSkill?.Invoke(skillConfig);
            OnLevelLoaded?.Invoke(level);
        }

        #region Waves

        private int currentWaveIndex;
        private Coroutine waveCoroutine;
        private IEnumerator IEWave(IWaveInfo[] waves)
        {
            if (waves == null || waves.Length == 0) yield break;
            yield return new WaitForEndOfFrame();
            
            IWaveInfo currentWave = null;
            while (currentWaveIndex < waves.Length)
            {
                currentWave = waves[currentWaveIndex];
                currentWave.SetupWave(gatePrefab, Towers, OnWaveForceStop);
                currentWaveIndex += 1;
                yield return currentWave.IEActivateWave();
            }
        }

        private void OnWaveForceStop()
        {
            if (waveCoroutine != null) StopCoroutine(waveCoroutine);
            waveCoroutine = StartCoroutine(IEWave(Level.waveInfos));
        }

        #endregion
        
        #region Towers

        private void InitTowers()
        {
            foreach (var tower in towers)
            {
                tower.Initialize(playerStats.hp, skillConfig.range);
                tower.OnDestroyed += OnTowerDestroyed; 
            }
        }

        private void OnTowerDestroyed(TowerEntity tower)
        {
            Debug.LogError($"Tower {tower.name} is destroyed");
        }
        
        private void TeleportTower(int towerIndex)
        {
            if (!canTeleportTower) return;
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
            OnChangeTower?.Invoke(CurrentTower.transform);
        }

        public void SetTeleportTowerState(bool enable)
        {
            canTeleportTower = enable;
        }
        #endregion
        
        private void Update()
        {
#if UNITY_EDITOR
            // Test tower change
            for (var i = 1; i <= 3; i++)
            {
                if (Input.GetKeyDown(i.ToString()))
                {
                    TestTowerChange(i - 1);
                }
            }
#endif
        }

        [Button]
        public void TestSkillChange()
        {
            OnChangeSkill?.Invoke(skillConfig);
        }

        [Button]
        public void TestTowerChange(int towerIndex)
        {
            TeleportTower(towerIndex);   
        }

        public LevelConfig testLevel;
        [Button]
        public void TestLoadLevel()
        {
            LoadLevel(testLevel);
        }
    }
}