using System;
using Core;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;

namespace InGame
{
    public class LevelManager : MonoSingleton<LevelManager>
    {
        [field: SerializeField] public GameStats GameStats { get; private set; }
        [SerializeField] private PlayerSkillConfig skillConfig;

        [SerializeField] private GateEntity gatePrefab;
        
        [SerializeField] private TowerEntity[] towers;
        private int currentTowerIndex = 0;
        public TowerEntity CurrentTower
        {
            get
            {
                currentTowerIndex = Math.Clamp(currentTowerIndex, 0, towers.Length);
                return towers[currentTowerIndex];
            }
        }

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

            OnChangeSkill = null;
            OnChangeTower = null;
        }

        public void LoadLevel(LevelConfig level)
        {
            // Create gate objects
            foreach (var gateCfg in level.gates)
            {
                var gateEntity = Instantiate(gatePrefab, gateCfg.position, quaternion.identity, null);
                gateEntity.Initialize(gateCfg, towers[gateCfg.targetBaseIndex]);
            }

            InitTowers();
            TeleportTower(0);
            OnChangeSkill?.Invoke(skillConfig);
            OnLevelLoaded?.Invoke(level);
        }

        #region Towers

        private void InitTowers()
        {
            foreach (var tower in towers)
            {
                tower.Initialize(skillConfig.range);
                tower.OnDestroyed += OnTowerDestroyed; 
            }
        }

        private void OnTowerDestroyed(TowerEntity tower)
        {
            Debug.LogError($"Tower {tower.name} is destroyed");
        }
        
        private void TeleportTower(int towerIndex)
        {
            if (towers[Math.Clamp(towerIndex, 0, towers.Length - 1)].IsDestroyed) return;
            
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