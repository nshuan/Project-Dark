using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core;
using Dark.Scripts.Common.UIWarning;
using Dark.Scripts.SceneNavigation;
using Dark.Scripts.Utils;
using InGame.EndlessLevel;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace InGame.EndlessEditor
{
    public class EndlessLevelEditor : MonoSingleton<EndlessLevelEditor>
    {
#if UNITY_EDITOR
        public RectTransform parentWaves;
        public EndlessWaveEditor prefabWave;
        public Button btnSave;
        public UIPopupWarning popupConfirm;
        public Button btnPlayLevel;
        public EndlessWaveInfoEditor waveInfoConfigEditor;
        public TMP_Dropdown drdEndlessLevel;
        public TMP_Dropdown drdLevelWaveInfo;
        public TMP_Dropdown drdWavePool;
        public TMP_Dropdown drdWaves;
        public TMP_Dropdown drdWaveToAddToPool;
        public Button btnAddWave;
        public Button btnAddPool;
        public Button btnAddLevel;
        public Button btnAddWaveInfo;
        public Button btnDeleteWave;
        public Button btnDeletePool;
        public Button btnDeleteLevel;
        public Button btnDeleteWaveInfo;
        public Button btnRemoveWaveFromPool;

        [Space] [Header("Display")] 
        public TextMeshProUGUI txtLevel;

        [Space] [Header("Path")] 
        [FolderOnly] public DefaultAsset levelFolderPath;
        [FolderOnly] public DefaultAsset poolFolderPath;
        [FolderOnly] public DefaultAsset waveFolderPath;
        
        public CharacterClass.CharacterClass ClassType { get; set; }

        public List<LevelEndlessConfig> allLevels;
        public List<PoolWaveEndless> allWavePools;
        public List<WaveEndlessConfig> allWaves;
        public List<WaveEndlessConfig> allWavesToAddToPool;
        
        private LevelEndlessConfig currentLevel;
        private WaveEndlessInfo currentWaveInfo;
        private PoolWaveEndless currentPool;
        private WaveEndlessConfig currentWave;
        private EndlessWaveEditor currentWaveEditor;
        private List<WaveEndlessInfo> allWaveInfoInLevel;
        private List<WaveEndlessConfig> allWaveInPool;
        
        protected override void Awake()
        {
            base.Awake();
            
            GetAllLevels();
            GetAllWavePools();
            GetAllWaves();
            
            btnSave.onClick.RemoveAllListeners();
            btnSave.onClick.AddListener(SaveLevel);
            btnPlayLevel.onClick.RemoveAllListeners();
            btnPlayLevel.onClick.AddListener(PlaySelectingLevel);
            
            drdEndlessLevel.onValueChanged.RemoveAllListeners();
            drdEndlessLevel.ClearOptions();
            var options = new List<TMP_Dropdown.OptionData>() { new TMP_Dropdown.OptionData("None")};
            foreach (var level in allLevels)
            {
                options.Add(new TMP_Dropdown.OptionData() { text = level.name });
            }
            drdEndlessLevel.options = options;
            drdEndlessLevel.onValueChanged.AddListener(OnSelectLevel);
            
            drdLevelWaveInfo.onValueChanged.RemoveAllListeners();
            drdLevelWaveInfo.ClearOptions();
            drdLevelWaveInfo.onValueChanged.AddListener(OnSelectWaveInfo);
            
            drdWavePool.onValueChanged.RemoveAllListeners();
            drdWavePool.ClearOptions();
            options = new List<TMP_Dropdown.OptionData>() { new TMP_Dropdown.OptionData("None")};
            foreach (var pool in allWavePools)
            {
                options.Add(new TMP_Dropdown.OptionData() { text = pool.name });
            }
            drdWavePool.options = options;
            drdWavePool.onValueChanged.AddListener(OnSelectWavePool);
            
            drdWaves.onValueChanged.RemoveAllListeners();
            drdWaves.ClearOptions();
            options = new List<TMP_Dropdown.OptionData>() { new TMP_Dropdown.OptionData("None")};
            foreach (var wave in allWaves)
            {
                options.Add(new TMP_Dropdown.OptionData() { text = wave.name });
            }
            drdWaves.options = options;
            drdWaves.onValueChanged.AddListener(OnSelectWave);
            
            drdWaveToAddToPool.onValueChanged.RemoveAllListeners();
            drdWaveToAddToPool.ClearOptions();
            drdWaveToAddToPool.onValueChanged.AddListener(OnSelectWaveToAddToPool);
            
            btnAddLevel.onClick.RemoveAllListeners();
            btnAddLevel.onClick.AddListener(AddLevel);
            
            btnAddPool.onClick.RemoveAllListeners();
            btnAddPool.onClick.AddListener(AddPool);
            
            btnAddWave.onClick.RemoveAllListeners();
            btnAddWave.onClick.AddListener(AddWave);
            
            btnAddWaveInfo.onClick.RemoveAllListeners();
            btnAddWaveInfo.onClick.AddListener(AddWaveInfo);

            btnDeleteLevel.onClick.RemoveAllListeners();
            btnDeleteLevel.onClick.AddListener(DeleteLevel);
            
            btnDeletePool.onClick.RemoveAllListeners();
            btnDeletePool.onClick.AddListener(DeletePool);
            
            btnDeleteWave.onClick.RemoveAllListeners();
            btnDeleteWave.onClick.AddListener(DeleteWave);
            
            btnDeleteWaveInfo.onClick.RemoveAllListeners();
            btnDeleteWaveInfo.onClick.AddListener(DeleteWaveInfo);
            
            btnRemoveWaveFromPool.onClick.RemoveAllListeners();
            btnRemoveWaveFromPool.onClick.AddListener(DeleteWaveFromPool);
        }

        #region Selecting
        
        private void OnSelectLevel(int index)
        {
            if (index == 0) LoadLevel(null);
            else LoadLevel(allLevels[index - 1]);
        }

        private void OnSelectWaveInfo(int index)
        {
            if (index <= 0 || !currentLevel || allWaveInfoInLevel == null || allWaveInfoInLevel.Count == 0 || index > allWaveInfoInLevel.Count)
            {
                currentWaveInfo = null;
                waveInfoConfigEditor.UpdateValue(currentWaveInfo);
                waveInfoConfigEditor.gameObject.SetActive(false);
                drdWavePool.value = 0;
                OnSelectWavePool(0);
                drdWavePool.RefreshShownValue();
                return;
            }

            currentWaveInfo = allWaveInfoInLevel[index - 1];
            waveInfoConfigEditor.gameObject.SetActive(true);
            waveInfoConfigEditor.UpdateValue(currentWaveInfo);
            
            if (allWavePools == null || allWavePools.Count == 0) return;

            var poolIndex = 0;
            for (var i = 0; i < allWavePools.Count; i++)
            {
                if (allWavePools[i] == currentWaveInfo.wavePool)
                {
                    poolIndex = i + 1;
                    break;
                }
            }
            
            drdWavePool.value = poolIndex;
            OnSelectWavePool(poolIndex);
            drdWavePool.RefreshShownValue();
        }

        private void OnSelectWavePool(int index)
        {
            if (index <= 0 || allWavePools == null || allWavePools.Count == 0 || index > allWavePools.Count)
            {
                currentPool = null;
                if (currentWaveInfo != null) currentWaveInfo.wavePool = currentPool;
                DeselectAnyWave();
                return;
            }

            currentPool = allWavePools[index - 1];
            if (currentWaveInfo != null) currentWaveInfo.wavePool = currentPool;
            allWaveInPool = currentPool.allWaves.ToList();
            RefreshPossibleWavesToAddToPool();
            if (allWaveInPool == null)
            {
                DeselectAnyWave();
                return;
            }

            var options = new List<TMP_Dropdown.OptionData>() { new TMP_Dropdown.OptionData("None") };
            for (var i = 0; i < allWaveInPool.Count; i++)
            {
                options.Add(new TMP_Dropdown.OptionData() { text = allWaveInPool[i].name });
            }

            drdWaves.options = options;
            DeselectAnyWave();

            void DeselectAnyWave()
            {
                drdWaves.value = 0;
                OnSelectWave(0);
                drdWaves.RefreshShownValue();
            }
        }
        
        private void OnSelectWave(int index)
        {
            if (index <= 0 || allWaveInPool == null || allWaveInPool.Count == 0 || index > allWaveInPool.Count)
            {
                currentWave = null;
                
                // Destroy current wave editor
                if (currentWaveEditor)
                {
                    Destroy(currentWaveEditor.gameObject);
                    currentWaveEditor = null;
                }
                
                return;
            }

            currentWave = allWaveInPool[index - 1];
            
            if (currentWaveEditor == null)
            {
                currentWaveEditor = Instantiate(prefabWave, parentWaves);
            }
            
            currentWaveEditor.UpdateUI(currentWave);
            currentWaveEditor.gameObject.SetActive(true);
        }

        private void OnSelectWaveToAddToPool(int index)
        {
            if (!currentPool) return;
            if (index < 0 || allWavesToAddToPool == null || index >= allWavesToAddToPool.Count) return;
            var waveToAdd = allWavesToAddToPool[index];
            AddWaveToPool(waveToAdd);
            RefreshPossibleWavesToAddToPool();
            
            var options = new List<TMP_Dropdown.OptionData>() { new TMP_Dropdown.OptionData("None") };
            var showIndex = 0;
            for (var i = 0; i < allWaveInPool.Count; i++)
            {
                options.Add(new TMP_Dropdown.OptionData() { text = allWaveInPool[i].name });
                if (allWaveInPool[i] == waveToAdd) showIndex = i + 1;
            }

            drdWaves.options = options;
            
            // Auto-select the newly created wave (in-memory; not an asset yet)
            drdWaves.value = showIndex;
            OnSelectWave(showIndex);
            drdWaves.RefreshShownValue();
        }
        
        #endregion

        #region Adding

        private void AddWaveToPool(WaveEndlessConfig waveConfig)
        {
            allWaveInPool ??= new List<WaveEndlessConfig>();
            allWaveInPool.Add(waveConfig);
        }
        
        private void DeleteWaveFromPool()
        {
            if (allWaveInPool == null) return;
            if (!currentWave) return;
            if (allWaveInPool.Contains(currentWave))
            {
                allWaveInPool.Remove(currentWave);
                
                // Refresh global waves dropdown (used when picking inside a pool)
                var options = new List<TMP_Dropdown.OptionData>() { new TMP_Dropdown.OptionData("None") };
                for (var i = 0; i < allWaveInPool.Count; i++)
                {
                    options.Add(new TMP_Dropdown.OptionData() { text = allWaveInPool[i].name });
                }

                drdWaves.options = options;
                drdWaves.value = 0;
                OnSelectWave(0);
                drdWaves.RefreshShownValue();
                
                RefreshPossibleWavesToAddToPool();
            }
        }

        private void AddWaveInfo()
        {
            if (!currentLevel) return;
            
            allWaveInfoInLevel ??= new List<WaveEndlessInfo>();

            var newWaveInfo = new WaveEndlessInfo();
            allWaveInfoInLevel.Add(newWaveInfo);

            // Refresh pools dropdown
            var options = new List<TMP_Dropdown.OptionData>() { new TMP_Dropdown.OptionData("None") };
            for (var i = 0; i < allWaveInfoInLevel.Count; i++)
            {
                options.Add(new TMP_Dropdown.OptionData() { text = $"wave {i}" });
            }

            drdLevelWaveInfo.options = options;

            // Auto-select the newly created pool (in-memory; not an asset yet)
            var newIndex = Mathf.Max(1, options.Count - 1);
            drdLevelWaveInfo.value = newIndex;
            OnSelectWaveInfo(newIndex);
            drdLevelWaveInfo.RefreshShownValue();
        }
        
        private void AddWave()
        {
            allWaves ??= new List<WaveEndlessConfig>();

            var wave = ScriptableObject.CreateInstance<WaveEndlessConfig>();
            wave.name = GetUniqueName("WaveEndless_New", allWaves.Select(w => w ? w.name : null));
            wave.id = GetNextId(allWaves.Select(w => w ? w.id : 0));

            allWaves.Add(wave);
            AddWaveToPool(wave);
            RefreshPossibleWavesToAddToPool();

            // Refresh global waves dropdown (used when picking inside a pool)
            var options = new List<TMP_Dropdown.OptionData>() { new TMP_Dropdown.OptionData("None") };
            for (var i = 0; i < allWaveInPool.Count; i++)
            {
                options.Add(new TMP_Dropdown.OptionData() { text = allWaveInPool[i].name });
            }

            drdWaves.options = options;
     
            // Auto-select the newly created wave (in-memory; not an asset yet)
            var newIndex = Mathf.Max(1, options.Count - 1);
            drdWaves.value = newIndex;
            OnSelectWave(newIndex);
            drdWaves.RefreshShownValue();
        }

        private void AddPool()
        {
            allWavePools ??= new List<PoolWaveEndless>();

            var pool = ScriptableObject.CreateInstance<PoolWaveEndless>();
            pool.name = GetUniqueName("WaveEndlessPool_New", allWavePools.Select(p => p ? p.name : null));
            pool.allWaves = Array.Empty<WaveEndlessConfig>();

            allWavePools.Add(pool);

            // Refresh pools dropdown
            var options = new List<TMP_Dropdown.OptionData>() { new TMP_Dropdown.OptionData("None") };
            foreach (var p in allWavePools)
            {
                if (!p) continue;
                options.Add(new TMP_Dropdown.OptionData() { text = p.name });
            }
            drdWavePool.options = options;

            // Auto-select the newly created pool (in-memory; not an asset yet)
            var newIndex = Mathf.Max(1, options.Count - 1);
            drdWavePool.value = newIndex;
            OnSelectWavePool(newIndex);
            drdWavePool.RefreshShownValue();
        }

        private void AddLevel()
        {
            allLevels ??= new List<LevelEndlessConfig>();

            var level = ScriptableObject.CreateInstance<LevelEndlessConfig>();
            level.name = GetUniqueName("LevelEndless_New", allLevels.Select(l => l ? l.name : null));
            level.id = GetNextId(allLevels.Select(l => l ? l.id : 0));
            level.waveInfo = Array.Empty<WaveEndlessInfo>();

            allLevels.Add(level);

            // Refresh levels dropdown
            var options = new List<TMP_Dropdown.OptionData>() { new TMP_Dropdown.OptionData("None") };
            foreach (var l in allLevels)
            {
                if (!l) continue;
                options.Add(new TMP_Dropdown.OptionData() { text = l.name });
            }
            drdEndlessLevel.options = options;

            // Auto-select and load the newly created level (in-memory; not an asset yet)
            var newIndex = Mathf.Max(1, options.Count - 1);
            drdEndlessLevel.value = newIndex;
            OnSelectLevel(newIndex);
            drdEndlessLevel.RefreshShownValue();
        }

        private void RefreshPossibleWavesToAddToPool()
        {
            allWavesToAddToPool = new List<WaveEndlessConfig>();
            
            if (!currentPool || allWaves == null || allWaves.Count == 0)
            {
                drdWaveToAddToPool.options = new List<TMP_Dropdown.OptionData>();
                drdWaveToAddToPool.RefreshShownValue();
                return;
            }

            foreach (var w in allWaves)
            {
                if (allWaveInPool != null && allWaveInPool.Contains(w)) continue;
                allWavesToAddToPool.Add(w);
            }

            var options = allWavesToAddToPool.Select(w => new TMP_Dropdown.OptionData(w.name));
            drdWaveToAddToPool.options = options.ToList();
            drdWaveToAddToPool.RefreshShownValue();
        }

        private static int GetNextId(IEnumerable<int> existingIds)
        {
            var max = 0;
            foreach (var id in existingIds)
            {
                if (id > max) max = id;
            }
            return max + 1;
        }

        private static string GetUniqueName(string baseName, IEnumerable<string> existingNames)
        {
            var set = new HashSet<string>(existingNames.Where(n => !string.IsNullOrWhiteSpace(n)));
            if (!set.Contains(baseName)) return baseName;

            var i = 1;
            while (true)
            {
                var candidate = $"{baseName}_{i}";
                if (!set.Contains(candidate)) return candidate;
                i++;
            }
        }
        
        #endregion

        #region Deleting

        private void DeleteLevel()
        {
            if (!currentLevel) return;

            // Remove from list
            if (allLevels != null)
                allLevels.Remove(currentLevel);

#if UNITY_EDITOR
            // Delete asset on disk if it exists
            var levelPath = AssetDatabase.GetAssetPath(currentLevel);
            if (!string.IsNullOrEmpty(levelPath))
            {
                AssetDatabase.DeleteAsset(levelPath);
                AssetDatabase.Refresh();
            }
#endif

            currentLevel = null;
            txtLevel?.SetText(string.Empty);

            // Rebuild level dropdown
            var options = new List<TMP_Dropdown.OptionData> { new TMP_Dropdown.OptionData("None") };
            if (allLevels != null)
            {
                foreach (var level in allLevels)
                {
                    if (!level) continue;
                    options.Add(new TMP_Dropdown.OptionData { text = level.name });
                }
            }

            drdEndlessLevel.options = options;
            drdEndlessLevel.value = 0;
            OnSelectLevel(0);
            drdEndlessLevel.RefreshShownValue();
        }

        private void DeleteWaveInfo()
        {
            if (currentWaveInfo == null) return;

            if (allWaveInfoInLevel == null) return;
            allWaveInfoInLevel.Remove(currentWaveInfo);
            currentWaveInfo = null;

            // Rebuild wave-info dropdown for the current level
            var options = new List<TMP_Dropdown.OptionData> { new TMP_Dropdown.OptionData("None") };
            for (var i = 0; i < allWaveInfoInLevel.Count; i++)
            {
                options.Add(new TMP_Dropdown.OptionData { text = $"wave {i}" });
            }

            drdLevelWaveInfo.options = options;
            drdLevelWaveInfo.value = 0;
            OnSelectWaveInfo(0);
            drdLevelWaveInfo.RefreshShownValue();
        }

        private void DeletePool()
        {
            if (!currentPool) return;

            // Clear references from wave infos
            if (allWaveInfoInLevel != null)
            {
                foreach (var info in allWaveInfoInLevel)
                {
                    if (info != null && info.wavePool == currentPool)
                        info.wavePool = null;
                }
            }

            // Remove from list
            if (allWavePools != null)
                allWavePools.Remove(currentPool);

#if UNITY_EDITOR
            // Delete asset on disk if it exists
            var poolPath = AssetDatabase.GetAssetPath(currentPool);
            if (!string.IsNullOrEmpty(poolPath))
            {
                AssetDatabase.DeleteAsset(poolPath);
                AssetDatabase.Refresh();
            }
#endif

            currentPool = null;
            allWaveInPool = null;

            // Rebuild pool dropdown
            var options = new List<TMP_Dropdown.OptionData> { new TMP_Dropdown.OptionData("None") };
            if (allWavePools != null)
            {
                foreach (var pool in allWavePools)
                {
                    if (!pool) continue;
                    options.Add(new TMP_Dropdown.OptionData { text = pool.name });
                }
            }

            drdWavePool.options = options;
            drdWavePool.value = 0;
            OnSelectWavePool(0);
            drdWavePool.RefreshShownValue();
        }
        
        private void DeleteWave()
        {
            if (!currentWave) return;

            // Remove from global waves list
            if (allWaves != null)
                allWaves.Remove(currentWave);

            // Remove from current pool list
            if (allWaveInPool != null)
                allWaveInPool.Remove(currentWave);

            RefreshPossibleWavesToAddToPool();

#if UNITY_EDITOR
            // Delete asset on disk if it exists
            var wavePath = AssetDatabase.GetAssetPath(currentWave);
            if (!string.IsNullOrEmpty(wavePath))
            {
                AssetDatabase.DeleteAsset(wavePath);
                AssetDatabase.Refresh();
            }

            if (allWavePools != null)
            {
                foreach (var pool in allWavePools)
                {
                    if (pool != null && pool.allWaves != null && pool.allWaves.Contains(currentWave))
                    {
                        var newWaves = new List<WaveEndlessConfig>();
                        foreach (var w in pool.allWaves)
                        {
                            if (w == currentWave) continue;
                            newWaves.Add(w);
                        }

                        pool.allWaves = newWaves.ToArray();
                        
                        EditorUtility.SetDirty(pool);
                    }
                }
                
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
#endif

            currentWave = null;

            // Destroy editor UI
            if (currentWaveEditor)
            {
                Destroy(currentWaveEditor.gameObject);
                currentWaveEditor = null;
            }

            // Rebuild waves-in-pool dropdown
            var options = new List<TMP_Dropdown.OptionData> { new TMP_Dropdown.OptionData("None") };
            if (allWaveInPool != null)
            {
                foreach (var wave in allWaveInPool)
                {
                    if (!wave) continue;
                    options.Add(new TMP_Dropdown.OptionData { text = wave.name });
                }
            }

            drdWaves.options = options;
            drdWaves.value = 0;
            OnSelectWave(0);
            drdWaves.RefreshShownValue();
        }

        #endregion
        
        public void LoadLevel(LevelEndlessConfig level)
        {
            currentLevel = level;
            if (!currentLevel)
            {
                allWaveInfoInLevel = null;
                drdLevelWaveInfo.options = new List<TMP_Dropdown.OptionData>() { new TMP_Dropdown.OptionData("None")};
                drdLevelWaveInfo.value = 0;
                OnSelectWaveInfo(0);
                drdLevelWaveInfo.RefreshShownValue();
                return;
            }
            
            allWaveInfoInLevel = new List<WaveEndlessInfo>();
            if (currentLevel.waveInfo != null)
            {
                foreach (var waveInfo in currentLevel.waveInfo)
                {
                    allWaveInfoInLevel.Add(new WaveEndlessInfo()
                    {
                        scaleHp = waveInfo.scaleHp,
                        scaleDmg = waveInfo.scaleDmg,
                        scaleSpe = waveInfo.scaleSpe,
                        expRatio = waveInfo.expRatio,
                        darkRatio = waveInfo.darkRatio,
                        darkUnitValue = waveInfo.darkUnitValue,
                        sigils = waveInfo.sigils,
                        ashes = waveInfo.ashes,
                        wavePool = waveInfo.wavePool
                    });
                }
            }
            
            // Add options for all waves in current level
            var options = new List<TMP_Dropdown.OptionData>() { new TMP_Dropdown.OptionData("None")};
            if (allWaveInfoInLevel != null)
            {
                for (var i = 0; i < allWaveInfoInLevel.Count; i++)
                {
                    options.Add(new TMP_Dropdown.OptionData()
                    {
                        text = $"wave {i}"
                    });
                }
            }
            drdLevelWaveInfo.options = options;
            drdLevelWaveInfo.value = 0;
            OnSelectWaveInfo(0);
            drdLevelWaveInfo.RefreshShownValue();
            
            txtLevel?.SetText($"Level: {level.name}");
        }

        public void PlaySelectingLevel()
        {
            if (!currentLevel) return;
            
#if UNITY_EDITOR
            LevelManager.isLoadFromInit = true;
#endif
            this.DelayCall(0.5f, () =>
            {
                Loading.Instance.QuickLoadScene(SceneConstants.SceneInGame, () =>
                {
                    LevelManager.Instance.LoadEndlessLevel();
                });
            });
        }

        #region Save

        public void SaveLevel()
        {
            if (currentWaveEditor)
            {
                currentWaveEditor.SaveWave();
            }

            // Persist newly created in-memory objects first so references can be serialized.
            PersistNewAssets();

            if (currentPool)
            {
                currentPool.allWaves = allWaveInPool.ToArray();
                EditorUtility.SetDirty(currentPool);
            }

            if (currentLevel)
            {
                var allWaveInfos = new List<WaveEndlessInfo>();
                foreach (var waveInfo in allWaveInfoInLevel)
                {
                    allWaveInfos.Add(new WaveEndlessInfo()
                    {
                        scaleHp = waveInfo.scaleHp,
                        scaleDmg = waveInfo.scaleDmg,
                        scaleSpe = waveInfo.scaleSpe,
                        expRatio = waveInfo.expRatio,
                        darkRatio = waveInfo.darkRatio,
                        darkUnitValue = waveInfo.darkUnitValue,
                        sigils = waveInfo.sigils,
                        ashes = waveInfo.ashes,
                        wavePool = waveInfo.wavePool
                    });
                }
                currentLevel.waveInfo = allWaveInfos.ToArray();
                EditorUtility.SetDirty(currentLevel);
            }
                        
#if UNITY_EDITOR
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
        }

        private void PersistNewAssets()
        {
#if UNITY_EDITOR
            var waveFolder = AssetDatabase.GetAssetPath(waveFolderPath);
            var poolFolder = AssetDatabase.GetAssetPath(poolFolderPath);
            var levelFolder = AssetDatabase.GetAssetPath(levelFolderPath);

            if (allWaves != null)
            {
                foreach (var wave in allWaves)
                {
                    EnsureAssetCreated(wave, waveFolder, "WaveEndless");
                }
            }

            if (allWavePools != null)
            {
                foreach (var pool in allWavePools)
                {
                    EnsureAssetCreated(pool, poolFolder, "WaveEndlessPool");
                }
            }

            if (allLevels != null)
            {
                foreach (var level in allLevels)
                {
                    EnsureAssetCreated(level, levelFolder, "LevelEndless");
                }
            }
#endif
        }

#if UNITY_EDITOR
        private static void EnsureAssetCreated<T>(T obj, string folderPath, string fallbackName) where T : ScriptableObject
        {
            if (!obj) return;
            if (string.IsNullOrWhiteSpace(folderPath)) return;
            if (!AssetDatabase.IsValidFolder(folderPath)) return;
            if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(obj))) return;

            var fileName = string.IsNullOrWhiteSpace(obj.name) ? fallbackName : obj.name;
            var path = AssetDatabase.GenerateUniqueAssetPath($"{folderPath}/{fileName}.asset");
            AssetDatabase.CreateAsset(obj, path);
            EditorUtility.SetDirty(obj);
        }
#endif

        #endregion

        #region Editor

        public void GetAllLevels()
        {
            var folderPath = UnityEditor.AssetDatabase.GetAssetPath(levelFolderPath);
            allLevels = AssetUtility.LoadAllScriptableObjectsInFolder<LevelEndlessConfig>(folderPath).ToList();
        }

        public void GetAllWavePools()
        {
            var folderPath = UnityEditor.AssetDatabase.GetAssetPath(poolFolderPath);
            allWavePools = AssetUtility.LoadAllScriptableObjectsInFolder<PoolWaveEndless>(folderPath).ToList();
        }

        public void GetAllWaves()
        {
            var folderPath = UnityEditor.AssetDatabase.GetAssetPath(waveFolderPath);
            allWaves = AssetUtility.LoadAllScriptableObjectsInFolder<WaveEndlessConfig>(folderPath).ToList();
        }

        #endregion
#endif
    }
}