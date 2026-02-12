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
        private Dictionary<int, LevelConfig> archerLevelMap;
        
        [ReadOnly, NonSerialized, OdinSerialize]
        private int maxLevelArcher;

        [ReadOnly, NonSerialized, OdinSerialize]
        private Dictionary<int, LevelConfig> knightLevelMap;

        [ReadOnly, NonSerialized, OdinSerialize]
        private int maxLevelKnight;

        public LevelConfig GetLevel(CharacterClass.CharacterClass classType, int level)
        {
            if (classType == CharacterClass.CharacterClass.Archer)
            {
                if (archerLevelMap.TryGetValue(level, out var value)) return Instantiate(value);
            }
            else if (classType == CharacterClass.CharacterClass.Knight)
            {
                if (knightLevelMap.TryGetValue(level, out var value)) return Instantiate(value);
            }
            return null;
        }

        public LevelConfig GetTrueLevel(CharacterClass.CharacterClass classType, int level)
        {
            if (classType == CharacterClass.CharacterClass.Archer)
            {
                if (archerLevelMap.TryGetValue(level, out var value)) return value;
            }
            else if (classType == CharacterClass.CharacterClass.Knight)
            {
                if (knightLevelMap.TryGetValue(level, out var value)) return value;
            }
            return null;
        }

        public LevelConfig[] GetAllLevels(CharacterClass.CharacterClass classType)
        {
            if (classType == CharacterClass.CharacterClass.Archer)
            {
                if (archerLevelMap == null)
                    return null;
            
                return archerLevelMap.Values.ToArray();
            }
            else if (classType == CharacterClass.CharacterClass.Knight)
            {
                if (knightLevelMap == null)
                    return null;
                
                return knightLevelMap.Values.ToArray();
            }

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
        public const string ArcherLevelPath = "Assets/Dark/Config/LevelArcherInGame";
        public const string ArcherWavePath = "Assets/Dark/Config/LevelWaveArcherInGame";
        public const string KnightLevelPath = "Assets/Dark/Config/LevelKnightInGame";
        public const string KnightWavePath = "Assets/Dark/Config/LevelWaveKnightInGame";

        [MenuItem("Dark/Manifest/Generate Level Manifest")]
        public static void CreateInstance()
        {
            AssetDatabaseUtils.CreateSOInstance<LevelManifest>(FilePath);
        }
        
        [Button]
        public void Validate()
        {
            Debug.ClearDeveloperConsole();
            
            var archerLevels = AssetUtility.LoadAllScriptableObjectsInFolder<LevelConfig>(ArcherLevelPath).ToList();
            archerLevelMap = new Dictionary<int, LevelConfig>();
            foreach (var level in archerLevels)
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
                    
                if (archerLevelMap.ContainsKey(level.level))
                {
                    DebugUtility.LogError($"Level {level.name} has invalid level index!");
                    continue;
                }
                
                archerLevelMap.Add(level.level, level);
            }

            archerLevelMap = archerLevelMap.OrderBy((pair) => pair.Key).ToDictionary((pair) => pair.Key, pair => pair.Value);
            maxLevelArcher = archerLevelMap.Keys.Max();

            for (var i = 1; i <= maxLevelArcher; i++)
            {
                if (!archerLevelMap.ContainsKey(i))
                    DebugUtility.LogError($"Level {i} is missing!");
            }
            
            // Knight
            var knightLevels = AssetUtility.LoadAllScriptableObjectsInFolder<LevelConfig>(KnightLevelPath).ToList();
            knightLevelMap = new Dictionary<int, LevelConfig>();
            foreach (var level in knightLevels)
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
                    
                if (knightLevelMap.ContainsKey(level.level))
                {
                    DebugUtility.LogError($"Level {level.name} has invalid level index!");
                    continue;
                }
                
                knightLevelMap.Add(level.level, level);
            }

            knightLevelMap = knightLevelMap.OrderBy((pair) => pair.Key).ToDictionary((pair) => pair.Key, pair => pair.Value);
            maxLevelKnight = knightLevelMap.Keys.Max();

            for (var i = 1; i <= maxLevelKnight; i++)
            {
                if (!knightLevelMap.ContainsKey(i))
                    DebugUtility.LogError($"Level {i} is missing!");
            }
            
            AssetDatabase.SaveAssets();
        }
#endif
    }
}