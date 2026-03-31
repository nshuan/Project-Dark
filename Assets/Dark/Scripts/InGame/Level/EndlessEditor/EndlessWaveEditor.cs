using System;
using System.Collections.Generic;
using System.Linq;
using InGame.EndlessLevel;
using InGame.GateEditorV2;
using Sirenix.OdinInspector;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace InGame.EndlessEditor
{
    public class EndlessWaveEditor : MonoBehaviour
    {
#if UNITY_EDITOR
        public Transform parentGates;
        public LevelGatePrefabEditorV2 prefabGate;
        public Transform parentGateConfig;
        public LevelGateConfigEditorV2 prefabGateConfig;
        public Button btnDeleteGate;
        public Button btnAddGate;
        public TMP_Dropdown drdGateType;
        public Button btnEditGateSpawnPosition;
        public LevelGateSpawnPositionEditorV2 spawnPositionEditor;
        public TMP_Dropdown drdMapType;
        public TMP_Dropdown drdBgIndex;
        public Button btnEditTower;
        public LevelTowerPositionEditorV2 towerPositionEditor;
        
        [FolderOnly] public DefaultAsset configFolderPath;

        public List<EnemyBehaviour> availableEnemies;
        public Dictionary<int, GateEntity> gateMap;

        public WaveEndlessConfig waveConfig;
        private Transform[] targetPositions;
        public bool Selecting { get; set; }
        public int currentGateType;
        public int currentMapType;
        public int currentBgIndex;

        private List<EnemyBehaviour> AvailableEnemies
        {
            get
            {
                if (availableEnemies == null)
                {
                    availableEnemies = new List<EnemyBehaviour>();
                    GetAvailableEnemies();
                }
                return availableEnemies;
            }    
        }

        [Button]
        public void GetAvailableEnemies()
        {
            var folderPath = UnityEditor.AssetDatabase.GetAssetPath(configFolderPath);
            availableEnemies = AssetUtility.LoadAllScriptableObjectsInFolder<EnemyBehaviour>(folderPath).ToList();
            availableEnemies.Sort((enemy1, enemy2) => enemy1.enemyId.CompareTo(enemy2.enemyId));
        }

        private void Awake()
        {
            if (Camera.main)
                targetPositions = FindObjectsByType<TowerEntity>(FindObjectsInactive.Include, FindObjectsSortMode.None).Select((tower) => tower.transform).ToArray();
        }

        private void Start()
        {
            _ = AvailableEnemies;
            
            btnDeleteGate.onClick.RemoveAllListeners();
            btnDeleteGate.onClick.AddListener(DeleteSelectingGate);
            
            btnAddGate.onClick.RemoveAllListeners();
            btnAddGate.onClick.AddListener(() => AddGate(
                new GateConfig() { targetBaseIndex = Array.Empty<int>(), spawnType = AvailableEnemies[0], spawnLogic = new GateSpawnSingle() },
                currentGateType));
            
            btnEditGateSpawnPosition.onClick.RemoveAllListeners();
            btnEditGateSpawnPosition.onClick.AddListener(OpenSpawnPositionEditor);
            
            drdGateType.ClearOptions();
            drdGateType.options = gateMap.Select(pair => 
                new TMP_Dropdown.OptionData($"{pair.Key} - {pair.Value.name}")).ToList();
            drdGateType.onValueChanged.RemoveAllListeners();
            drdGateType.onValueChanged.AddListener((value) => currentGateType = value);
            
            drdMapType.ClearOptions();
            var options = new List<TMP_Dropdown.OptionData>();
            for (var i = 0; i < Enum.GetValues(typeof(LevelMapType)).Length; i++)
            {
                options.Add(new TMP_Dropdown.OptionData($"{(LevelMapType)i}"));
            }
            drdMapType.options = options;
            drdMapType.onValueChanged.RemoveAllListeners();
            drdMapType.onValueChanged.AddListener((value) => currentMapType = value);
            
            drdBgIndex.ClearOptions();
            drdBgIndex.options = new List<int>() { 1, 2, 3 }.Select(index => 
                new TMP_Dropdown.OptionData($"Bg {index}")).ToList();
            drdBgIndex.onValueChanged.RemoveAllListeners();
            drdBgIndex.onValueChanged.AddListener((value) => currentBgIndex = value);
            
            btnEditTower.onClick.RemoveAllListeners();
            btnEditTower.onClick.AddListener(() =>
            {
                if (!waveConfig) return;
                towerPositionEditor.Setup(LevelTowerEditorV2.Instance.GetTowers((LevelMapType)currentMapType));
                towerPositionEditor.gameObject.SetActive(true);
            });
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Delete))
            {
                DeleteSelectingGate();
            }
        }

        public void UpdateUI(WaveEndlessConfig waveInfo)
        {
            gateMap ??= GateManifest.GetAll();
            
            waveConfig = waveInfo;
            if (!waveInfo) return;
            
            LevelTowerEditorV2.Instance.SetPosition((LevelMapType)currentMapType, waveConfig.towerPositions);
            
            // Delete all gate
            foreach (Transform child in parentGates)
                Destroy(child.gameObject);
            
            if (waveInfo.gateConfigs == null) return;
            foreach (var gateConfig in waveInfo.gateConfigs)
            {
                AddGate(gateConfig, GetGatePrefabId(gateConfig.gatePrefab));
            }
        }

        public int GetGatePrefabId(GateEntity gatePrefab)
        {
            if (gateMap == null) return 0;
            if (!gatePrefab) return 0;
            
            return gateMap.FirstOrDefault((pair) => pair.Value == gatePrefab).Key;
        }
        
        public void AddGate(GateConfig gateConfig, int gatePrefabId)
        {
            var newGate = Instantiate(prefabGate, parentGates);
            newGate.UpdateUI(gateConfig, gatePrefabId);
            newGate.TargetPositions = targetPositions;
            newGate.OnClick = SelectGate;
            newGate.OnDragging = null;
            var newGateConfig = Instantiate(prefabGateConfig, parentGateConfig);
            newGateConfig.AvailableEnemies = new Dictionary<int, EnemyBehaviour>();
            foreach (var enemy in AvailableEnemies)
            {
                newGateConfig.AvailableEnemies[enemy.enemyId] = enemy;
            }
            newGateConfig.GateMap = gateMap;
            newGateConfig.Setup(newGate);
        }
        
        public void SelectGate(LevelGatePrefabEditorV2 gate)
        {
            foreach (Transform child in parentGates)
            {
                var childScript = child.GetComponent<LevelGatePrefabEditorV2>();
                if (!childScript) continue;
                childScript.DeselectGate();
            }

            foreach (Transform child in parentGateConfig)
            {
                if (child.TryGetComponent<LevelGateConfigEditorV2>(out var childGateConfig))
                {
                    if (childGateConfig.targetGate == gate)
                        childGateConfig.Select();
                    else
                        childGateConfig.Deselect();
                }
            }
            
            gate.SelectGate();
        }

        private void OpenSpawnPositionEditor()
        {
            foreach (Transform child in parentGateConfig)
            {
                if (child.TryGetComponent<LevelGateConfigEditorV2>(out var childGateConfig))
                {
                    if (childGateConfig.targetGate.Selecting)
                    {
                        spawnPositionEditor.Setup(childGateConfig.targetGate);
                        return;
                    }
                }
            }
        }
        
        private void DeleteSelectingGate()
        {
            foreach (Transform child in parentGateConfig)
            {
                if (child.TryGetComponent<LevelGateConfigEditorV2>(out var childGateConfig))
                {
                    if (childGateConfig.targetGate.Selecting)
                    {
                        DeleteGate(childGateConfig.targetGate);
                        Destroy(childGateConfig.gameObject);
                    }
                }
            }
        }
        
        public void DeleteGate(LevelGatePrefabEditorV2 gate)
        {
            Destroy(gate.gameObject);
        }
        
        #region Save

        public void SaveWave()
        {
            if (!waveConfig) return;

            waveConfig.mapType = (LevelMapType)currentMapType;
            waveConfig.backgroundIndex = currentBgIndex;
            waveConfig.towerPositions = LevelTowerEditorV2.Instance.GetPositions((LevelMapType)currentMapType);
            
            waveConfig.gateConfigs = new List<GateConfig>();
            foreach (Transform gate in parentGates)
            {
                if (gate.TryGetComponent<LevelGatePrefabEditorV2>(out var gateConfigEditor))
                {
                    gateConfigEditor.Config.position = gateConfigEditor.Position;
                    
                    var newTargets = new List<int>();
                    if (gateConfigEditor.Config.targetBaseIndex != null)
                    {
                        newTargets.AddRange(gateConfigEditor.Config.targetBaseIndex);
                    }

                    IGateSpawner newSpawnLogic;
                    if (gateConfigEditor.Config.spawnLogic is GateSpawnSingle existingSingle)
                    {
                        newSpawnLogic = new GateSpawnSingle() { radius = existingSingle.radius, randomSpanAngle = existingSingle.randomSpanAngle };
                    }
                    else if (gateConfigEditor.Config.spawnLogic is GateSpawnTriangle existingTriangle)
                    {
                        newSpawnLogic = new GateSpawnTriangle() {  radius = existingTriangle.radius, randomSpanAngle = existingTriangle.randomSpanAngle };
                    }
                    else if (gateConfigEditor.Config.spawnLogic is GateSpawnMultiple existingMultiple)
                    {
                        newSpawnLogic = new GateSpawnMultiple() { amount = existingMultiple.amount, randomSpanAngle = existingMultiple.randomSpanAngle, maxRadius = existingMultiple.maxRadius };
                    }
                    else if (gateConfigEditor.Config.spawnLogic is GateSpawnPositions existingPositions)
                    {
                        newSpawnLogic = new GateSpawnPositions() { amount = existingPositions.amount };
                
                        if (gateConfigEditor.SpawnPositions != null)
                        {
                            var newSpawnPositions = new List<Vector2>();
                            var newSpawnPositionInfos = new List<GateSpawnPositionInfo>();
                            
                            foreach (var position in gateConfigEditor.SpawnPositions)
                            {
                                newSpawnPositions.Add(position.Value.spawnPosition);
                                var attackPos = new List<Vector2>();
                                if (position.Value.attackPositions != null)
                                {
                                    foreach (var pos in position.Value.attackPositions)
                                    {
                                        attackPos.Add(pos);
                                    }
                                }
                                newSpawnPositionInfos.Add(new GateSpawnPositionInfo()
                                {
                                    spawnPosition = position.Value.spawnPosition,
                                    attackPositions = attackPos.ToArray()
                                });
                            }
                    
                            newSpawnLogic = new GateSpawnPositions()
                            {
                                amount = existingPositions.amount, 
                                spawnPositions = newSpawnPositions.ToArray(),
                                spawnPositionInfos = newSpawnPositionInfos.ToArray()
                            };
                        }
                    }
                    else
                    {
                        newSpawnLogic = new GateSpawnCenter();
                    }
                    
                    waveConfig.gateConfigs.Add(new GateConfig()
                    {
                        isBossGate = gateConfigEditor.Config.isBossGate,
                        position = gateConfigEditor.Position,
                        targetBaseIndex = newTargets.ToArray(),
                        startTime = gateConfigEditor.Config.startTime,
                        duration = gateConfigEditor.Config.duration,
                        spawnType = gateConfigEditor.Config.spawnType,
                        intervalLoop = gateConfigEditor.Config.intervalLoop,
                        spawnLogic = newSpawnLogic,
                        startTimeVisual = gateConfigEditor.Config.startTimeVisual,
                        durationVisual = gateConfigEditor.Config.durationVisual,
                        gatePrefab = gateConfigEditor.Config.gatePrefab,
                        hideOrb = gateConfigEditor.Config.hideOrb
                    });
                }
            }
            
            EditorUtility.SetDirty(waveConfig);
            AssetDatabase.SaveAssets();
        }
        
        #endregion
                
#endif
    }
}