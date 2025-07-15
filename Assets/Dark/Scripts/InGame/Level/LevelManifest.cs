using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace InGame
{
    [CreateAssetMenu(menuName = "InGame/Level Manifest", fileName = "LevelManifest")]
    public class LevelManifest : SerializedScriptableObject
    {
        [ReadOnly, NonSerialized, OdinSerialize]
        private Dictionary<int, LevelConfig> levelMap;

        [ReadOnly, NonSerialized, OdinSerialize]
        private int maxLevel;

        public LevelConfig GetLevel(int level)
        {
            if (levelMap.TryGetValue(level, out var value)) return Instantiate(value);
            return null;
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
        private const string LevelPath = "Assets/Config/LevelInGame";

        [Button]
        private void Validate()
        {
            Debug.ClearDeveloperConsole();
            
            var levels = AssetUtility.LoadAllScriptableObjectsInFolder<LevelConfig>(LevelPath).ToList();
            levelMap = new Dictionary<int, LevelConfig>();
            foreach (var level in levels)
            {
                if (int.TryParse(level.name.Split(" ")[1], out var levelNum))
                {
                    level.level = levelNum;
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
        }
#endif
    }
}