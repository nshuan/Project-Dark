using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dark.Tools.Utils;
using InGame.ConfigManager;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;

namespace InGame
{
    public class LevelManifest : SerializedScriptableObject
    {
        private static string Path = "LevelManifest";
        private static string FilePath = "Assets/Dark/Resources/LevelManifest.asset";
        
        [ReadOnly, NonSerialized, OdinSerialize]
        private Dictionary<int, LevelConfig> levelMap;

        [ReadOnly, NonSerialized, OdinSerialize]
        private int maxLevel;

        public LevelConfig GetLevel(int level)
        {
            if (levelMap.TryGetValue(level, out var value)) return Instantiate(value);
            return null;
        }

        public LevelConfig GetTrueLevel(int level)
        {
            if (levelMap.TryGetValue(level, out var value)) return value;
            return null;
        }

        public LevelConfig[] GetAllLevels()
        {
            if (levelMap == null)
                return null;
            
            return levelMap.Values.ToArray();
        }
        
        #region SINGLETON

        private static LevelManifest instance;

        public static LevelManifest Instance
        {
            get
            {
                if (instance == null)
                    instance = Resources.Load<LevelManifest>("LevelManifest");

                return instance;
            }
        }
        #endregion
        
#if UNITY_EDITOR
        public const string LevelPath = "Assets/Dark/Config/LevelInGame";
        public const string WavePath = "Assets/Dark/Config/LevelWaveInGame";

        [MenuItem("Dark/Manifest/Generate Level Manifest")]
        public static void CreateInstance()
        {
            AssetDatabaseUtils.CreateSOInstance<LevelManifest>(FilePath);
        }
        
        [Button]
        public void Validate()
        {
            Debug.ClearDeveloperConsole();
            
            var levels = AssetUtility.LoadAllScriptableObjectsInFolder<LevelConfig>(LevelPath).ToList();
            levelMap = new Dictionary<int, LevelConfig>();
            foreach (var level in levels)
            {
                if (int.TryParse(level.name.Split(" ")[1], out var levelNum))
                {
                    if (level.level != levelNum)
                    {
                        level.level = levelNum;
                        EditorUtility.SetDirty(level);
                    }
                }
                else
                {
                    DebugUtility.LogError($"Level {level.name} has invalid level name!");
                }
                    
                if (levelMap.ContainsKey(level.level))
                {
                    DebugUtility.LogError($"Level {level.name} has invalid level index!");
                    continue;
                }
                
                levelMap.Add(level.level, level);
            }

            levelMap = levelMap.OrderBy((pair) => pair.Key).ToDictionary((pair) => pair.Key, pair => pair.Value);
            maxLevel = levelMap.Keys.Max();

            for (var i = 1; i <= maxLevel; i++)
            {
                if (!levelMap.ContainsKey(i))
                    DebugUtility.LogError($"Level {i} is missing!");
            }
            
            AssetDatabase.SaveAssets();
        }
#endif
    }
}