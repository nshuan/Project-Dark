using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;

namespace InGame.ConfigManager
{
    [CreateAssetMenu(menuName = "InGame/Config Manifest", fileName = "ConfigManifest")]
    public class ConfigManifest : SerializedScriptableObject
    {
        [Space, Header("Player Stats")] [ReadOnly, NonSerialized, OdinSerialize]
        private PlayerStats playerConfig;
        public PlayerStats PlayerConfig => playerConfig;

        [Space, Header("Player Skills")] [ReadOnly, NonSerialized, OdinSerialize]
        private PlayerSkillConfig[] skillConfig;
        public PlayerSkillConfig[] SkillConfig => skillConfig;

        #region SINGTON

        private static ConfigManifest instance;

        public static ConfigManifest Instance
        {
            get
            {
                if (instance == null)
                    instance = Resources.Load<ConfigManifest>("ConfigManifest");

                return instance;
            }
        }

        #endregion
        
#if UNITY_EDITOR
        private const string PATH_PLAYER_CONFIG = "Assets/Config/Player";
        [Button]
        public void ValidatePlayerConfig()
        {
            playerConfig = AssetUtility.LoadAllScriptableObjectsInFolder<PlayerStats>(PATH_PLAYER_CONFIG)[0];
        }
        
        private const string PATH_SKILL_CONFIG = "Assets/Config/PlayerSkill";
        [Button]
        public void ValidateSkillConfig()
        {
            skillConfig = AssetUtility.LoadAllScriptableObjectsInFolder<PlayerSkillConfig>(PATH_SKILL_CONFIG).ToArray();
        }
#endif
    }
}