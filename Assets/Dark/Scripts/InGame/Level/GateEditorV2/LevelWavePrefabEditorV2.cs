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
        public Button btnDeleteGate;
        public Button btnAddGate;

        [Space] [Header("Gate info")]
#if UNITY_EDITOR
        [FolderOnly] public DefaultAsset configFolderPath;
#endif
        public List<EnemyBehaviour> availableEnemies;
        public GameObject panelGateInfo;
        public Toggle txtIsBossGate;
        public TextMeshProUGUI txtPosition;
        public InputField inpTargetTower;
        public InputField inpStartTime;
        public InputField inpDuration;
        public Dropdown drdEnemy;
        public InputField inpIntervalLoop;
        public InputField inpStartTimeVisual;
        public InputField inpDurationVisual;

        private WaveConfig waveConfig;
        private Vector2[] targetPositions;

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
            drdEnemy.options = AvailableEnemies.Select(enemy => 
                new Dropdown.OptionData($"{enemy.enemyId} - {enemy.displayName}")).ToList();
            
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
            if (waveInfo == null) return;
            
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
            newGate.OnClick = (gate) =>
            {
                SelectGate(gate);
                UpdateSelectingGate(gate);
            };
            newGate.OnDragging = UpdateGatePositionDisplay;
        }
        
        public void SelectGate(LevelGatePrefabEditorV2 gate)
        {
            foreach (Transform child in parentGates)
            {
                var childScript = child.GetComponent<LevelGatePrefabEditorV2>();
                if (!childScript) continue;
                childScript.DeselectGate();
            }
            
            gate.SelectGate();
        }

        private void DeleteSelectingGate()
        {
            foreach (Transform child in parentGates)
            {
                var childScript = child.GetComponent<LevelGatePrefabEditorV2>();
                if (!childScript) continue;
                if (childScript.Selecting)
                {
                    DeleteGate(childScript);
                }
            }
        }
        
        public void DeleteGate(LevelGatePrefabEditorV2 gate)
        {
            Destroy(gate.gameObject);
        }

        public void UpdateSelectingGate(LevelGatePrefabEditorV2 gate)
        {
            txtIsBossGate.onValueChanged.RemoveAllListeners();
            inpTargetTower.onValueChanged.RemoveAllListeners();
            inpStartTime.onValueChanged.RemoveAllListeners();
            inpDuration.onValueChanged.RemoveAllListeners();
            drdEnemy.onValueChanged.RemoveAllListeners();
            inpIntervalLoop.onValueChanged.RemoveAllListeners();
            inpStartTimeVisual.onValueChanged.RemoveAllListeners();
            inpDurationVisual.onValueChanged.RemoveAllListeners();
            
            var gateConfig = gate.Config;
            txtIsBossGate.isOn = gateConfig.isBossGate;
            inpTargetTower.text = string.Join(", ", gateConfig.targetBaseIndex);
            inpStartTime.text = gateConfig.startTime.ToString(GameConst.FloatFormat);
            inpDuration.text = gateConfig.duration.ToString(GameConst.FloatFormat);
            drdEnemy.value = gateConfig.spawnType.enemyId;
            inpIntervalLoop.text = gateConfig.intervalLoop.ToString(GameConst.FloatFormat);
            inpStartTimeVisual.text = gateConfig.startTimeVisual.ToString(GameConst.FloatFormat);
            inpDurationVisual.text = gateConfig.durationVisual.ToString(GameConst.FloatFormat);

            // txtIsBossGate.onValueChanged.AddListener((isOn) => gate.IsBossGate = isOn);
            txtIsBossGate.onValueChanged.AddListener((isOn) => gate.Config.isBossGate = isOn);
            inpTargetTower.onValueChanged.AddListener((value) =>
            {
                value = value.Trim(' ');
                if (value == "0,1,2" || value == "0,2,1" || value == "1,0,2" || value == "1,2,0" || value == "2,0,1" ||
                    value == "2,1,0"
                    || value == "0,1" || value == "1,0" || value == "0,2" || value == "2,0" || value == "1,2" ||
                    value == "2,1"
                    || value == "0" || value == "1" || value == "2")
                {
                    // gate.StrTargetTowers = value;
                    gate.Config.targetBaseIndex = value.Split(",").Select(int.Parse).ToArray();
                }
                else
                {
                    value = "0";
                    // gate.StrTargetTowers = value;
                    gate.Config.targetBaseIndex = value.Split(",").Select(int.Parse).ToArray();
                }
            });
            inpStartTime.onValueChanged.AddListener((value) =>
            {
                // if (float.TryParse(value, out var time)) gate.StartTime = time;
                if (float.TryParse(value, out var time)) gate.Config.startTime = time;
            });
            inpDuration.onValueChanged.AddListener((value) =>
            {
                // if (float.TryParse(value, out var time)) gate.Duration = time;
                if (float.TryParse(value, out var time)) gate.Config.duration = time;
            });
            drdEnemy.onValueChanged.AddListener((value) =>
            {
                // gate.SpawnType = value;
                gate.Config.spawnType = AvailableEnemies[value];
            });
            inpIntervalLoop.onValueChanged.AddListener((value) =>
            {
                // if (float.TryParse(value, out var time)) gate.Interval = time;
                if (float.TryParse(value, out var time)) gate.Config.intervalLoop = time;
            });
            inpStartTimeVisual.onValueChanged.AddListener((value) =>
            {
                // if (float.TryParse(value, out var time)) gate.StartTimeVisual = time;
                if (float.TryParse(value, out var time)) gate.Config.startTimeVisual = time;
            });
            inpDurationVisual.onValueChanged.AddListener((value) =>
            {
                // if (float.TryParse(value, out var time)) gate.DurationVisual = time;
                if (float.TryParse(value, out var time)) gate.Config.durationVisual = time;
            });
        }

        public void UpdateGatePositionDisplay(Vector2 position)
        {
            txtPosition?.SetText($"X: {position.x.ToString(GameConst.FloatFormat)}\nY: {position.y.ToString(GameConst.FloatFormat)}");
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