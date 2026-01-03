using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace InGame.GateEditorV2
{
    public class LevelWavePrefabEditorV2 : MonoBehaviour
    {
        public Transform parentGates;
        public LevelGatePrefabEditorV2 prefabGate;
        public Transform parentGateConfig;
        public LevelGateConfigEditorV2 prefabGateConfig;
        public Button btnDeleteGate;
        public Button btnAddGate;

#if UNITY_EDITOR
        [FolderOnly] public DefaultAsset configFolderPath;
#endif
        public List<EnemyBehaviour> availableEnemies;
        // [Space] [Header("Gate info")]
        // public GameObject panelGateInfo;
        // public Toggle txtIsBossGate;
        // public TextMeshProUGUI txtPosition;
        // public InputField inpTargetTower;
        // public InputField inpStartTime;
        // public InputField inpDuration;
        // public Dropdown drdEnemy;
        // public InputField inpIntervalLoop;
        // public InputField inpStartTimeVisual;
        // public InputField inpDurationVisual;

        public WaveConfig waveConfig;
        private Vector2[] targetPositions;
        public bool Selecting { get; set; }

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
#if UNITY_EDITOR
            var folderPath = UnityEditor.AssetDatabase.GetAssetPath(configFolderPath);
            availableEnemies = AssetUtility.LoadAllScriptableObjectsInFolder<EnemyBehaviour>(folderPath).ToList();
            availableEnemies.Sort((enemy1, enemy2) => enemy1.enemyId.CompareTo(enemy2.enemyId));
#endif
        }

        private void Awake()
        {
            if (Camera.main)
                targetPositions = FindObjectsOfType<TowerEntity>().Select((tower) => (Vector2)Camera.main.WorldToScreenPoint(tower.transform.position)).ToArray();
        }

        private void Start()
        {
            _ = AvailableEnemies;
            
            btnDeleteGate.onClick.RemoveAllListeners();
            btnDeleteGate.onClick.AddListener(DeleteSelectingGate);
            
            btnAddGate.onClick.RemoveAllListeners();
            btnAddGate.onClick.AddListener(() => AddGate(new GateConfig() { targetBaseIndex = Array.Empty<int>(), spawnType = AvailableEnemies[0], spawnLogic = new GateSpawnSingle() }));
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Delete))
            {
                DeleteSelectingGate();
            }
        }

        public void UpdateUI(WaveConfig waveInfo)
        {
            waveConfig = waveInfo;
            if (!waveInfo) return;
            
            // Delete all gate
            foreach (Transform child in parentGates)
                Destroy(child.gameObject);
            
            if (waveInfo.gateConfigs == null) return;
            foreach (var gateConfig in waveInfo.gateConfigs)
            {
                AddGate(gateConfig);
            }
        }

        public void AddGate(GateConfig gateConfig)
        {
            var newGate = Instantiate(prefabGate, parentGates);
            newGate.UpdateUI(gateConfig);
            newGate.TargetPositions = targetPositions;
            newGate.OnClick = SelectGate;
            newGate.OnDragging = null;
            var newGateConfig = Instantiate(prefabGateConfig, parentGateConfig);
            newGateConfig.AvailableEnemies = AvailableEnemies;
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
            
            waveConfig.gateConfigs = new List<GateConfig>();
            foreach (Transform gate in parentGates)
            {
                if (gate.TryGetComponent<LevelGatePrefabEditorV2>(out var gateConfigEditor))
                {
                    // waveConfig.gateConfigs.Add(new GateConfig()
                    // {
                    //     isBossGate = gateConfigEditor.IsBossGate,
                    //     position = gateConfigEditor.Position,
                    //     targetBaseIndex = gateConfigEditor.StrTargetTowers.Trim(' ').Split(",").Select(int.Parse).ToArray(),
                    //     startTime = gateConfigEditor.StartTime,
                    //     duration = gateConfigEditor.Duration,
                    //     spawnType = AvailableEnemies[gateConfigEditor.SpawnType],
                    //     intervalLoop = gateConfigEditor.Interval,
                    //     spawnLogic = gateConfigEditor.Config.spawnLogic,
                    //     startTimeVisual = gateConfigEditor.StartTimeVisual,
                    //     durationVisual = gateConfigEditor.DurationVisual,
                    // });
                    gateConfigEditor.Config.position = gateConfigEditor.Position;
                    waveConfig.gateConfigs.Add(gateConfigEditor.Config);
                }
            }
            
#if UNITY_EDITOR
            EditorUtility.SetDirty(waveConfig);
            AssetDatabase.SaveAssets();
#endif
        }

        #endregion
    }
}