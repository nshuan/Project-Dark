using System;
using System.Collections.Generic;
using System.Linq;
using InGame.EndlessLevel;
using UnityEditor;
using UnityEngine;

namespace InGame.EndlessEditor.Editor
{
    /// <summary>
    /// Editor conversion utility: convert WaveConfig -> WaveEndlessConfig.
    /// </summary>
    public static class ToolConvertWaveToEndless
    {
        public const string DefaultOutputFolder = "Assets/Dark/Config/LevelEndlessWave";

        /// <summary>
        /// Converts one wave config into a new endless wave config.
        /// SourceLevel fields (mapType/backgroundIndex/towerPositions) are copied from the level that references the wave.
        /// </summary>
        public static WaveEndlessConfig ConvertWaveConfig(
            InGame.WaveConfig waveConfig,
            InGame.LevelConfig sourceLevel,
            int id,
            bool createAsset = true,
            string outputFolder = DefaultOutputFolder,
            string assetNameOverride = null)
        {
            if (!waveConfig)
            {
                Debug.LogError("[ToolConvertWaveToEndless] waveConfig is null.");
                return null;
            }
            
            if (!sourceLevel)
            {
                Debug.LogError($"[ToolConvertWaveToEndless] sourceLevel is null for waveConfig '{waveConfig.name}'.");
                return null;
            }
            
            var endlessWave = ScriptableObject.CreateInstance<WaveEndlessConfig>();
            endlessWave.id = id;
            endlessWave.mapType = sourceLevel.mapType;
            endlessWave.towerPositions = sourceLevel.towerPositions != null ? sourceLevel.towerPositions.ToArray() : null;
            endlessWave.gateConfigs = waveConfig.gateConfigs != null
                ? waveConfig.gateConfigs.Select(CloneGateConfig).Where(g => g != null).ToList()
                : new List<InGame.GateConfig>();
            
            var baseName = assetNameOverride;
            if (string.IsNullOrWhiteSpace(baseName))
                baseName = $"{id}_{waveConfig.name}_EndlessWave";
            endlessWave.name = baseName;
            
            if (!createAsset)
                return endlessWave;
            
            if (string.IsNullOrWhiteSpace(outputFolder) || !AssetDatabase.IsValidFolder(outputFolder))
            {
                Debug.LogError($"[ToolConvertWaveToEndless] Output folder '{outputFolder}' is invalid.");
                return endlessWave;
            }
            
            // If a wave was already converted previously with the same name, reuse it.
            var sameName = FindWaveEndlessByNameInFolder(endlessWave.name, outputFolder);
            if (sameName)
                return sameName;
            
            var path = AssetDatabase.GenerateUniqueAssetPath($"{outputFolder}/{endlessWave.name}.asset");
            AssetDatabase.CreateAsset(endlessWave, path);
            EditorUtility.SetDirty(endlessWave);
            return endlessWave;
        }

        /// <summary>
        /// Converts every WaveConfig referenced by any LevelConfig (Knight + Archer) into WaveEndlessConfig assets.
        /// </summary>
        public static void ConvertAllWaveConfigsToEndless(
            string outputFolder = DefaultOutputFolder,
            bool archer = true,
            bool knight = true,
            bool createAssets = true)
        {
            if (!archer && !knight) return;
            
            if (!AssetDatabase.IsValidFolder(outputFolder))
            {
                Debug.LogError($"[ToolConvertWaveToEndless] Output folder '{outputFolder}' is invalid.");
                return;
            }

            var existing = AssetUtility.LoadAllScriptableObjectsInFolder<WaveEndlessConfig>(outputFolder);
            var maxId = existing != null && existing.Count > 0 ? existing.Max(x => x.id) : 0;

            // Map: original WaveConfig asset -> the LevelConfig that references it.
            // If a WaveConfig is referenced by multiple levels, we keep the first one (fields should match for that wave).
            var waveToLevel = new Dictionary<InGame.WaveConfig, InGame.LevelConfig>();

            foreach (var level in LoadAllLevelConfigs(archer, knight))
            {
                if (level?.waveInfo == null) continue;

                for (var wi = 0; wi < level.waveInfo.Length; wi++)
                {
                    var waveInfo = level.waveInfo[wi];
                    if (waveInfo == null) continue;

                    if (waveInfo.waveConfig)
                        TryAddWave(waveInfo.waveConfig, level, waveToLevel);

                    if (waveInfo.randomWaveConfigs == null) continue;
                    for (var ri = 0; ri < waveInfo.randomWaveConfigs.Length; ri++)
                    {
                        var randomWaveConfig = waveInfo.randomWaveConfigs[ri];
                        if (!randomWaveConfig) continue;
                        TryAddWave(randomWaveConfig, level, waveToLevel);
                    }
                }
            }

            var orderedWaves = waveToLevel.Keys.OrderBy(w => w.name).ToList();
            var nextId = maxId + 1;

            foreach (var waveConfig in orderedWaves)
            {
                var sourceLevel = waveToLevel[waveConfig];
                if (!sourceLevel) continue;

                ConvertWaveConfig(
                    waveConfig,
                    sourceLevel,
                    id: nextId++,
                    createAsset: createAssets,
                    outputFolder: outputFolder);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static IEnumerable<InGame.LevelConfig> LoadAllLevelConfigs(bool archer, bool knight)
        {
            var archerLevels = AssetUtility.LoadAllScriptableObjectsInFolder<InGame.LevelConfig>(InGame.LevelManifest.ArcherLevelPath);
            var knightLevels = AssetUtility.LoadAllScriptableObjectsInFolder<InGame.LevelConfig>(InGame.LevelManifest.KnightLevelPath);

            if (archer && !knight) return archerLevels;
            if (!archer && knight) return knightLevels;
            
            return (archerLevels ?? Enumerable.Empty<InGame.LevelConfig>())
                .Concat(knightLevels ?? Enumerable.Empty<InGame.LevelConfig>());
        }

        private static void TryAddWave(
            InGame.WaveConfig waveConfig,
            InGame.LevelConfig level,
            Dictionary<InGame.WaveConfig, InGame.LevelConfig> waveToLevel)
        {
            if (!waveConfig || !level) return;
            if (waveToLevel.ContainsKey(waveConfig)) return;
            waveToLevel[waveConfig] = level;
        }

        private static WaveEndlessConfig FindWaveEndlessByNameInFolder(string assetName, string folderPath)
        {
            var guids = AssetDatabase.FindAssets($"t:{nameof(WaveEndlessConfig)}", new[] { folderPath });
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<WaveEndlessConfig>(path);
                if (!asset) continue;
                if (asset.name == assetName) return asset;
            }

            return null;
        }

        private static InGame.GateConfig CloneGateConfig(InGame.GateConfig src)
        {
            if (src == null) return null;

            // GateConfig is a class, so we clone it to avoid shared references between WaveConfig and WaveEndlessConfig.
            return new InGame.GateConfig
            {
                isBossGate = src.isBossGate,
                position = src.position,
                targetBaseIndex = src.targetBaseIndex != null ? src.targetBaseIndex.ToArray() : null,
                startTime = src.startTime,
                duration = src.duration,
                spawnType = src.spawnType,
                gatePrefab = src.gatePrefab,
                intervalLoop = src.intervalLoop,
                spawnLogic = src.spawnLogic,
                startTimeVisual = src.startTimeVisual,
                durationVisual = src.durationVisual,
                hideOrb = src.hideOrb
            };
        }
    }
}