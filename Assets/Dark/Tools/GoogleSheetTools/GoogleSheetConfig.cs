using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dark.Scripts.InGame.Upgrade;
using InGame;
using InGame.Upgrade;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;

namespace Dark.Tools.GoogleSheetTool
{
    public class GoogleSheetConfig : SerializedScriptableObject
    {
        public static string Path = "Assets/Dark/Tools/GoogleSheetTools/GoogleSheetConfig.asset";

        public string sheetLink;
        public string sheetId;
        public string sheetApiKey;
        
        [Space]
        [NonSerialized, OdinSerialize] public GoogleSheetDataInfo[] data;
    }
    
    [Serializable]
    public class GoogleSheetDataInfo
    {
        public GoogleSheetTabs sheetName;
        public ScriptableObject[] configs;

#if UNITY_EDITOR
        [Space]
        [Header("Editor")]
        [FolderOnly] public DefaultAsset configFolderPath;
        [Button]
        public virtual void GetConfigsSortByName()
        {
            if (configFolderPath == null)
            {
                Debug.LogError("Empty config folder path");
                return;
            }
            
            var folderPath = UnityEditor.AssetDatabase.GetAssetPath(configFolderPath);
            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:" + nameof(ScriptableObject), new[] { folderPath });
            List<(int, ScriptableObject)> assets = new List<(int, ScriptableObject)>();

            foreach (string guid in guids)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                ScriptableObject asset = UnityEditor.AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
                if (asset == null) continue;
                
                // Validate configs name
                var underscoreIndex = asset.name.IndexOf('_');
                if (underscoreIndex <= 0)
                {
                    Debug.LogError($"Invalid config name: {path}");
                    return;
                }

                // Validate config index
                if (int.TryParse(asset.name.Substring(0, underscoreIndex), out var index))
                {
                    assets.Add((index, asset));
                }
                else
                {
                    Debug.LogError($"Invalid config id: {path}");
                    return;
                }
            }
            
            // Sort by index (id)
            assets.Sort((asset1, asset2) => asset1.Item1.CompareTo(asset2.Item1));
            configs = assets.Select((asset) => asset.Item2).ToArray();
            EditorUtility.SetDirty(AssetDatabase.LoadAssetAtPath<GoogleSheetConfig>(GoogleSheetConfig.Path));
        }

        public T CreateNewConfig<T>(string name) where T : ScriptableObject
        {
            if (!configFolderPath)
            {
                Debug.LogError("Can't create new config - Empty config folder path");
                return null;
            }
            
            var folderPath = UnityEditor.AssetDatabase.GetAssetPath(configFolderPath);
            var filePath = folderPath + "/" + name + ".asset";
            
            if (File.Exists(filePath))
            {
                Debug.LogWarning("Can't create new config - Config file already exists!");
                return AssetDatabase.LoadAssetAtPath<T>(filePath);
            }
            
            T asset = ScriptableObject.CreateInstance<T>();
            
            // Create and save the asset
            AssetDatabase.CreateAsset(asset, filePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            return asset;
        }
#endif
    }

    [Serializable]
    public class GoogleSheetNodeCostDataInfo : GoogleSheetDataInfo
    {
#if UNITY_EDITOR
        public override void GetConfigsSortByName()
        {
            configs = new[]
            {
                AssetDatabase.LoadAssetAtPath<UpgradeRequirementConfig>(UpgradeRequirementConfig.FilePath),
            };
            EditorUtility.SetDirty(AssetDatabase.LoadAssetAtPath<GoogleSheetConfig>(GoogleSheetConfig.Path));
        }
#endif
    }
}
