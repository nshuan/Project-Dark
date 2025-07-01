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
            playerConfig = LoadAllScriptableObjectsInFolder<PlayerStats>(PATH_PLAYER_CONFIG)[0];
        }
        
        private const string PATH_SKILL_CONFIG = "Assets/Config/PlayerSkill";
        [Button]
        public void ValidateSkillConfig()
        {
            skillConfig = LoadAllScriptableObjectsInFolder<PlayerSkillConfig>(PATH_SKILL_CONFIG).ToArray();
        }
        
        private List<T> LoadAllScriptableObjectsInFolder<T>(string folderPath) where T : ScriptableObject
        {
            List<T> results = new List<T>();

            string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name, new[] { folderPath });

            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset != null)
                    results.Add(asset);
            }

            return results;
        }
#endif
    }
}