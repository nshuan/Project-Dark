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
        
        #region Action

        public Action<PlayerSkillConfig> OnChangeSkill;

        #endregion

        private void Start()
        {
            OnChangeSkill?.Invoke(skillConfig);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            OnChangeSkill = null;
        }

        [Button]
        public void TestSkillChange()
        {
            OnChangeSkill?.Invoke(skillConfig);
        }
    }
}