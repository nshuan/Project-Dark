#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using InGame.Upgrade;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;
using System.IO;
using InGame.CharacterClass;

namespace Dark.Scripts.OutGame.Upgrade.UIUpgradeTreeCreator.Editor
{
    public class UpgradeTreeGenerator : EditorWindow
    {
        [SerializeField] private UpgradeTreeManifest manifest;
        [SerializeField] private UpgradeTreeGeneratorConfig generatorConfig;
        [SerializeField] private UpgradeTreeConfig treeConfig;

        private GameObject treePrefab;
        private CharacterClass treeClass;
        private string dataFilePath = "";
        private string dataJson = "";

        [MenuItem("Tools/Dark/Upgrade Tree Generator")]
        public static void ShowWindow()
        {
            GetWindow<UpgradeTreeGenerator>("Upgrade Tree Generator");
        }

        private void OnGUI()
        {
            GUILayout.Label("Upgrade Tree Generator", EditorStyles.boldLabel);

            manifest = (UpgradeTreeManifest)EditorGUILayout.ObjectField("Manifest", manifest, typeof(UpgradeTreeManifest), false);
            treeConfig = (UpgradeTreeConfig)EditorGUILayout.ObjectField("Tree Config", treeConfig, typeof(UpgradeTreeConfig), false);
            generatorConfig = (UpgradeTreeGeneratorConfig)EditorGUILayout.ObjectField("Generator Config", generatorConfig, typeof(UpgradeTreeGeneratorConfig), false);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextField(dataFilePath, GUILayout.ExpandWidth(true));
            if (GUILayout.Button("Browse", GUILayout.Width(80)))
            {
                string path = EditorUtility.OpenFilePanel("Select JSON File", Application.dataPath, "json");
                if (!string.IsNullOrEmpty(path))
                {
                    dataFilePath = path;
                }
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(10);
            if (GUILayout.Button("Generate", GUILayout.Width(160)))
            {
                // Load JSON Data
                dataJson = File.ReadAllText(dataFilePath);
                var treeData = JsonUtility.FromJson<TreeDataStruct>(dataJson);
                
                // Create tree prefab
                if (treeData == null || treeData.nodes == null)
                {
                    Debug.LogError("Invalid JSON data.");
                    return;
                }
                treePrefab = CreateTreePrefab(Path.GetFileNameWithoutExtension(dataFilePath), treeData);
                if (treePrefab == null)
                {
                    Debug.LogError("Can not create tree.");
                    return;
                }
            }
            
            // Update manifest
            treeClass = (CharacterClass)EditorGUILayout.EnumPopup(treeClass);
            if (GUILayout.Button("Update manifest", GUILayout.Width(160)))
            {
                if (treePrefab == null)
                {
                    Debug.LogError("Create tree prefab first.");
                    return;
                }

                if (treePrefab.GetComponent<UIUpgradeTree>() == null)
                {
                    Debug.LogError("Invalid tree prefab.");
                    return;
                }

                if (treeConfig == null)
                {
                    Debug.LogError("Assign tree config first.");
                    return;
                }
                
                manifest.upgradeTreeMap.TryAdd(treeClass, null);
                manifest.upgradeTreeMap[treeClass] = new UpgradeTreeInfo()
                {
                    config = treeConfig,
                    prefab = treePrefab.GetComponent<UIUpgradeTree>()
                };
                    
                EditorUtility.SetDirty(manifest);
                    
            }

            EditorGUILayout.Space(10);
            EditorGUILayout.HelpBox("ScriptableObject reference and folder path can be used to drive behavior in your JSON logic.", MessageType.Info);
        }

        private GameObject CreateTreePrefab(string prefabName, TreeDataStruct treeData)
        {
            const string outputPrefabPath = "Assets/Dark/Prefabs/UIUpgradeTrees/";
        
            var canvas = Instantiate(generatorConfig.canvasPrefab.gameObject).GetComponent<Canvas>();
            var root = Instantiate(generatorConfig.treePrefab.gameObject, canvas.transform);
    
            foreach (var node in treeData.nodes)
            {
                if (!generatorConfig.nodePrefabsMap.TryGetValue((NodeType)node.idType, out var prefabList))
                {
                    Debug.LogWarning($"Missing prefab for idPrefab: {node.idPrefab}, skipping.");
                    continue;
                }

                if (node.idPrefab < 0 || node.idPrefab >= prefabList.Count)
                {
                    Debug.LogWarning($"Invalid idPrefab: {node.idPrefab}, skipping.");
                    continue;
                }
    
                GameObject go = (GameObject)PrefabUtility.InstantiatePrefab(prefabList[node.idPrefab]);
                go.transform.SetParent(root.transform.Find("Nodes"));
                go.transform.localPosition = node.position;
                go.GetComponent<UIUpgradeNode>().config = treeConfig.GetNodeById(node.id);
            }
            
            root.GetComponent<UIUpgradeTree>().ValidateNodes();
    
            var outputPath = outputPrefabPath + prefabName + ".prefab";
            var prefab = PrefabUtility.SaveAsPrefabAsset(root, outputPath);
            DestroyImmediate(root);
            DestroyImmediate(canvas.gameObject);
    
            Debug.Log("Prefab saved to: " + outputPath);
            return prefab;
        }
    }
}

#endif