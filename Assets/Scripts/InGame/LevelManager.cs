using System;
using Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace InGame
{
    public class LevelManager : MonoSingleton<LevelManager>
    {
        [field: SerializeField] public GameStats GameStats { get; private set; }
        [SerializeField] private PlayerSkillConfig skillConfig;

        [SerializeField] private Transform[] towers;
        private int currentTowerIndex = 0;
        public Transform CurrentTower
        {
            get
            {
                currentTowerIndex = Math.Clamp(currentTowerIndex, 0, towers.Length);
                return towers[currentTowerIndex];
            }
        }

        #region Action

        public Action<PlayerSkillConfig> OnChangeSkill;
        public Action<Transform> OnChangeTower;

        #endregion

        private void Start()
        {
            OnChangeTower?.Invoke(CurrentTower);
            OnChangeSkill?.Invoke(skillConfig);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            OnChangeSkill = null;
            OnChangeTower = null;
        }

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
            currentTowerIndex = towerIndex;
            OnChangeTower?.Invoke(CurrentTower);
        }
    }
}