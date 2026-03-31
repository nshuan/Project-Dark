using System;
using System.Collections.Generic;
using System.Linq;
using InGame.EndlessLevel;
using UnityEditor;
using UnityEngine;

namespace InGame.EndlessEditor.Editor
{
    public class EndlessWaveConverterWindow : EditorWindow
    {
        private string outputFolder = ToolConvertWaveToEndless.DefaultOutputFolder;
        private bool createAssets = true;

        private InGame.WaveConfig selectedWaveConfig;
        private InGame.LevelConfig selectedWaveSourceLevel;

        private int cachedUniqueWaveConfigCount;
        private bool showStats = true;

        [MenuItem("Dark/Tools/EndlessWaveConverter")]
        public static void OpenWindow()
        {
            var window = GetWindow<EndlessWaveConverterWindow>("Endless Wave Converter");
            window.minSize = new Vector2(420, 180);
            window.Show();
        }

        private void OnEnable()
        {
            RefreshStats();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Convert WaveConfig -> WaveEndlessConfig", EditorStyles.boldLabel);
            EditorGUILayout.Space(8);

            outputFolder = EditorGUILayout.TextField("Output Folder", outputFolder);
            createAssets = EditorGUILayout.Toggle("Create Assets", createAssets);

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Convert Selected WaveConfig", EditorStyles.boldLabel);
            selectedWaveConfig = (InGame.WaveConfig)EditorGUILayout.ObjectField("WaveConfig", selectedWaveConfig, typeof(InGame.WaveConfig), false);

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Use Project Selection"))
                {
                    if (Selection.activeObject is InGame.WaveConfig wave)
                    {
                        selectedWaveConfig = wave;
                        selectedWaveSourceLevel = FindFirstLevelUsingWave(selectedWaveConfig);
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Endless Wave Converter", "Select a WaveConfig asset in Project first.", "OK");
                    }
                }

                if (GUILayout.Button("Find Source Level") && selectedWaveConfig)
                {
                    selectedWaveSourceLevel = FindFirstLevelUsingWave(selectedWaveConfig);
                }
            }

            using (new EditorGUI.DisabledScope(!selectedWaveConfig))
            {
                EditorGUILayout.ObjectField("Source Level (auto)", selectedWaveSourceLevel, typeof(InGame.LevelConfig), false);

                if (GUILayout.Button("Convert Selected WaveConfig"))
                {
                    if (!selectedWaveConfig)
                        return;

                    if (string.IsNullOrWhiteSpace(outputFolder))
                    {
                        EditorUtility.DisplayDialog("Endless Wave Converter", "Output Folder is empty.", "OK");
                        return;
                    }

                    if (!AssetDatabase.IsValidFolder(outputFolder))
                    {
                        EditorUtility.DisplayDialog("Endless Wave Converter", $"Output folder is invalid:\n{outputFolder}", "OK");
                        return;
                    }

                    selectedWaveSourceLevel ??= FindFirstLevelUsingWave(selectedWaveConfig);
                    if (!selectedWaveSourceLevel)
                    {
                        EditorUtility.DisplayDialog(
                            "Endless Wave Converter",
                            "Couldn't find any LevelConfig that uses this WaveConfig.\n" +
                            "The converter needs the owning LevelConfig to copy mapType/backgroundIndex/towerPositions.",
                            "OK");
                        return;
                    }

                    var nextId = GetNextEndlessWaveId(outputFolder);

                    ToolConvertWaveToEndless.ConvertWaveConfig(
                        selectedWaveConfig,
                        selectedWaveSourceLevel,
                        id: nextId,
                        createAsset: createAssets,
                        outputFolder: outputFolder);

                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    RefreshStats();
                }
            }

            EditorGUILayout.Space(10);
            if (GUILayout.Button("Convert All WaveConfigs"))
            {
                if (string.IsNullOrWhiteSpace(outputFolder))
                {
                    EditorUtility.DisplayDialog("Endless Wave Converter", "Output Folder is empty.", "OK");
                    return;
                }

                if (EditorUtility.DisplayDialog(
                        "Confirm Convert",
                        $"Convert all WaveConfigs to WaveEndlessConfigs?\n\nOutput: {outputFolder}",
                        "Convert",
                        "Cancel"))
                {
                    ToolConvertWaveToEndless.ConvertAllWaveConfigsToEndless(outputFolder, createAssets);
                    RefreshStats();
                }
            }

            if (showStats)
            {
                EditorGUILayout.Space(8);
                EditorGUILayout.LabelField($"Unique WaveConfigs found: {cachedUniqueWaveConfigCount}");
                if (GUILayout.Button("Refresh Stats"))
                {
                    RefreshStats();
                }
            }
        }

        private void RefreshStats()
        {
#if UNITY_EDITOR
            try
            {
                cachedUniqueWaveConfigCount = CountUniqueWaveConfigs();
            }
            catch (Exception e)
            {
                Debug.LogError($"[EndlessWaveConverterWindow] RefreshStats failed: {e}");
                cachedUniqueWaveConfigCount = 0;
            }
#endif
        }

        private static int CountUniqueWaveConfigs()
        {
            var waveSet = new HashSet<InGame.WaveConfig>();

            var archerLevels = AssetUtility.LoadAllScriptableObjectsInFolder<InGame.LevelConfig>(InGame.LevelManifest.ArcherLevelPath);
            var knightLevels = AssetUtility.LoadAllScriptableObjectsInFolder<InGame.LevelConfig>(InGame.LevelManifest.KnightLevelPath);

            var levels = (archerLevels ?? Enumerable.Empty<InGame.LevelConfig>())
                .Concat(knightLevels ?? Enumerable.Empty<InGame.LevelConfig>());

            foreach (var level in levels)
            {
                if (level?.waveInfo == null) continue;

                for (var wi = 0; wi < level.waveInfo.Length; wi++)
                {
                    var waveInfo = level.waveInfo[wi];
                    if (waveInfo == null) continue;

                    if (waveInfo.waveConfig != null)
                        waveSet.Add(waveInfo.waveConfig);

                    if (waveInfo.randomWaveConfigs == null) continue;
                    for (var ri = 0; ri < waveInfo.randomWaveConfigs.Length; ri++)
                    {
                        var randomWaveConfig = waveInfo.randomWaveConfigs[ri];
                        if (randomWaveConfig != null)
                            waveSet.Add(randomWaveConfig);
                    }
                }
            }

            return waveSet.Count;
        }

        private static InGame.LevelConfig FindFirstLevelUsingWave(InGame.WaveConfig waveConfig)
        {
            if (!waveConfig) return null;

            var archerLevels = AssetUtility.LoadAllScriptableObjectsInFolder<InGame.LevelConfig>(InGame.LevelManifest.ArcherLevelPath);
            var knightLevels = AssetUtility.LoadAllScriptableObjectsInFolder<InGame.LevelConfig>(InGame.LevelManifest.KnightLevelPath);

            var levels = (archerLevels ?? Enumerable.Empty<InGame.LevelConfig>())
                .Concat(knightLevels ?? Enumerable.Empty<InGame.LevelConfig>());

            foreach (var level in levels)
            {
                if (level?.waveInfo == null) continue;
                for (var wi = 0; wi < level.waveInfo.Length; wi++)
                {
                    var waveInfo = level.waveInfo[wi];
                    if (waveInfo == null) continue;

                    if (waveInfo.waveConfig == waveConfig)
                        return level;

                    if (waveInfo.randomWaveConfigs == null) continue;
                    for (var ri = 0; ri < waveInfo.randomWaveConfigs.Length; ri++)
                    {
                        if (waveInfo.randomWaveConfigs[ri] == waveConfig)
                            return level;
                    }
                }
            }

            return null;
        }

        private static int GetNextEndlessWaveId(string outputFolder)
        {
            var existing = AssetUtility.LoadAllScriptableObjectsInFolder<WaveEndlessConfig>(outputFolder);
            if (existing == null || existing.Count == 0) return 1;
            return existing.Max(w => w ? w.id : 0) + 1;
        }
    }
}

