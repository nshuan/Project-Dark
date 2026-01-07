using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InGame.GateEditorV2
{
    public class LevelGateConfigEditorV2 : MonoBehaviour
    {
        public GameObject objSelected;

        [Space] [Header("Gate info")] 
        public TMP_InputField inpGateLabel;
        public Toggle txtIsBossGate;
        public TextMeshProUGUI txtPosition;
        public TMP_InputField inpTargetTower;
        public TMP_InputField inpStartTime;
        public TMP_InputField inpDuration;
        public TMP_Dropdown drdEnemy;
        public TMP_InputField inpIntervalLoop;
        public TMP_InputField inpStartTimeVisual;
        public TMP_InputField inpDurationVisual;
        
        [Space] [Header("Spawn Logic")]
        public TMP_Dropdown drdSpawnLogic;
        
        [Space] [Header("Spawn Logic Variables")]
        public GameObject panelSpawnLogicVars;
        // GateSpawnSingle & GateSpawnTriangle
        public GameObject panelRadius;
        public TMP_InputField inpRadius;
        public GameObject panelRandomSpanAngle;
        public TMP_InputField inpRandomSpanAngle;
        // GateSpawnMultiple
        public GameObject panelAmount;
        public TMP_InputField inpAmount;
        public GameObject panelMaxRadius;
        public TMP_InputField inpMaxRadius;
        // GateSpawnCenter - no variables

        public List<EnemyBehaviour> AvailableEnemies { get; set; }
        public LevelGatePrefabEditorV2 targetGate;

        public void Setup(LevelGatePrefabEditorV2 gate)
        {
            targetGate = gate;
            gate.OnDragging += (position) =>
            {
                txtPosition?.SetText(
                    $"X: {position.x.ToString(GameConst.FloatFormat)}\nY: {position.y.ToString(GameConst.FloatFormat)}");
            };
            gate.txtGateLabel.SetText($"Gate {transform.GetSiblingIndex()}");
            inpGateLabel.text = $"Gate {transform.GetSiblingIndex()}";
            inpGateLabel.onValueChanged.RemoveAllListeners();
            inpGateLabel.onValueChanged.AddListener((value) =>
            {
                gate.txtGateLabel.SetText(value);
            });
            
            drdEnemy.options = AvailableEnemies.Select(enemy => 
                new TMP_Dropdown.OptionData($"{enemy.enemyId} - {enemy.displayName}")).ToList();
            
            txtIsBossGate.onValueChanged.RemoveAllListeners();
            inpTargetTower.onValueChanged.RemoveAllListeners();
            inpStartTime.onValueChanged.RemoveAllListeners();
            inpDuration.onValueChanged.RemoveAllListeners();
            drdEnemy.onValueChanged.RemoveAllListeners();
            inpIntervalLoop.onValueChanged.RemoveAllListeners();
            inpStartTimeVisual.onValueChanged.RemoveAllListeners();
            inpDurationVisual.onValueChanged.RemoveAllListeners();
            if (drdSpawnLogic != null) drdSpawnLogic.onValueChanged.RemoveAllListeners();
            if (inpRadius != null) inpRadius.onValueChanged.RemoveAllListeners();
            if (inpRandomSpanAngle != null) inpRandomSpanAngle.onValueChanged.RemoveAllListeners();
            if (inpAmount != null) inpAmount.onValueChanged.RemoveAllListeners();
            if (inpMaxRadius != null) inpMaxRadius.onValueChanged.RemoveAllListeners();
            
            var gateConfig = gate.Config;
            txtIsBossGate.isOn = gateConfig.isBossGate;
            inpTargetTower.text = string.Join(", ", gateConfig.targetBaseIndex);
            inpStartTime.text = gateConfig.startTime.ToString(GameConst.FloatFormat);
            inpDuration.text = gateConfig.duration.ToString(GameConst.FloatFormat);
            drdEnemy.value = gateConfig.spawnType.enemyId;
            inpIntervalLoop.text = gateConfig.intervalLoop.ToString(GameConst.FloatFormat);
            inpStartTimeVisual.text = gateConfig.startTimeVisual.ToString(GameConst.FloatFormat);
            inpDurationVisual.text = gateConfig.durationVisual.ToString(GameConst.FloatFormat);
            
            // Setup spawn logic dropdown
            if (drdSpawnLogic != null)
            {
                drdSpawnLogic.options = new List<TMP_Dropdown.OptionData>
                {
                    new TMP_Dropdown.OptionData("Single"),
                    new TMP_Dropdown.OptionData("Triangle"),
                    new TMP_Dropdown.OptionData("Multiple"),
                    new TMP_Dropdown.OptionData("Center")
                };
                
                // Initialize spawn logic if null
                if (gateConfig.spawnLogic == null)
                {
                    gateConfig.spawnLogic = new GateSpawnSingle();
                }
                
                // Set dropdown value based on current spawn logic type
                int spawnLogicIndex = GetSpawnLogicIndex(gateConfig.spawnLogic);
                drdSpawnLogic.value = spawnLogicIndex;
                UpdateSpawnLogicPanels(spawnLogicIndex);
                LoadSpawnLogicValues(gateConfig.spawnLogic);
            }

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
            
            // Spawn logic dropdown handler
            if (drdSpawnLogic != null)
            {
                drdSpawnLogic.onValueChanged.AddListener((value) =>
                {
                    UpdateSpawnLogicType(gate, value);
                    UpdateSpawnLogicPanels(value);
                    LoadSpawnLogicValues(gate.Config.spawnLogic);
                });
            }
            
            // Spawn logic variable handlers
            if (inpRadius != null)
            {
                inpRadius.onValueChanged.AddListener((value) =>
                {
                    if (float.TryParse(value, out var radius))
                    {
                        if (gate.Config.spawnLogic is GateSpawnSingle single)
                        {
                            single.radius = Mathf.Clamp(radius, 1f, 2f);
                        }
                        else if (gate.Config.spawnLogic is GateSpawnTriangle triangle)
                        {
                            triangle.radius = Mathf.Clamp(radius, 1f, 2f);
                        }
                    }
                });
            }

            if (inpRandomSpanAngle != null)
            {
                inpRandomSpanAngle.onValueChanged.AddListener((value) =>
                {
                    if (float.TryParse(value, out var angle))
                    {
                        if (gate.Config.spawnLogic is GateSpawnSingle single)
                        {
                            single.randomSpanAngle = angle;
                        }
                        else if (gate.Config.spawnLogic is GateSpawnTriangle triangle)
                        {
                            triangle.randomSpanAngle = angle;
                        }
                        else if (gate.Config.spawnLogic is GateSpawnMultiple multiple)
                        {
                            multiple.randomSpanAngle = angle;
                        }
                    }
                });
            }

            if (inpAmount != null)
            {
                inpAmount.onValueChanged.AddListener((value) =>
                {
                    if (int.TryParse(value, out var amount))
                    {
                        if (gate.Config.spawnLogic is GateSpawnMultiple multiple)
                        {
                            multiple.amount = Mathf.Clamp(amount, 1, 10);
                        }
                    }
                });
            }

            if (inpMaxRadius != null)
            {
                inpMaxRadius.onValueChanged.AddListener((value) =>
                {
                    if (float.TryParse(value, out var maxRadius))
                    {
                        if (gate.Config.spawnLogic is GateSpawnMultiple multiple)
                        {
                            multiple.maxRadius = Mathf.Clamp(maxRadius, 1f, 2f);
                        }
                    }
                });
            }
        }
        
        private int GetSpawnLogicIndex(IGateSpawner spawnLogic)
        {
            if (spawnLogic is GateSpawnSingle) return 0;
            if (spawnLogic is GateSpawnTriangle) return 1;
            if (spawnLogic is GateSpawnMultiple) return 2;
            if (spawnLogic is GateSpawnCenter) return 3;
            return 0;
        }
        
        private void UpdateSpawnLogicType(LevelGatePrefabEditorV2 gate, int index)
        {
            IGateSpawner newSpawnLogic = null;
            
            switch (index)
            {
                case 0: // Single
                    if (gate.Config.spawnLogic is GateSpawnSingle existingSingle)
                    {
                        newSpawnLogic = existingSingle;
                    }
                    else
                    {
                        newSpawnLogic = new GateSpawnSingle();
                    }
                    break;
                case 1: // Triangle
                    if (gate.Config.spawnLogic is GateSpawnTriangle existingTriangle)
                    {
                        newSpawnLogic = existingTriangle;
                    }
                    else
                    {
                        newSpawnLogic = new GateSpawnTriangle();
                    }
                    break;
                case 2: // Multiple
                    if (gate.Config.spawnLogic is GateSpawnMultiple existingMultiple)
                    {
                        newSpawnLogic = existingMultiple;
                    }
                    else
                    {
                        newSpawnLogic = new GateSpawnMultiple();
                    }
                    break;
                case 3: // Center
                    newSpawnLogic = new GateSpawnCenter();
                    break;
            }
            
            gate.Config.spawnLogic = newSpawnLogic;
        }
        
        private void UpdateSpawnLogicPanels(int spawnLogicIndex)
        {
            // Hide all panels first
            if (panelSpawnLogicVars != null) panelSpawnLogicVars.SetActive(false);
            if (panelRadius != null) panelRadius.SetActive(false);
            if (panelRandomSpanAngle != null) panelRandomSpanAngle.SetActive(false);
            if (panelAmount != null) panelAmount.SetActive(false);
            if (panelMaxRadius != null) panelMaxRadius.SetActive(false);
            
            // Show relevant panels based on spawn logic type
            if (panelSpawnLogicVars != null) panelSpawnLogicVars.SetActive(true);
            
            switch (spawnLogicIndex)
            {
                case 0: // Single
                    if (panelRadius != null) panelRadius.SetActive(true);
                    if (panelRandomSpanAngle != null) panelRandomSpanAngle.SetActive(true);
                    break;
                case 1: // Triangle
                    if (panelRadius != null) panelRadius.SetActive(true);
                    if (panelRandomSpanAngle != null) panelRandomSpanAngle.SetActive(true);
                    break;
                case 2: // Multiple
                    if (panelAmount != null) panelAmount.SetActive(true);
                    if (panelMaxRadius != null) panelMaxRadius.SetActive(true);
                    if (panelRandomSpanAngle != null) panelRandomSpanAngle.SetActive(true);
                    break;
                case 3: // Center
                    // No variables, keep panels hidden
                    break;
            }
        }
        
        private void LoadSpawnLogicValues(IGateSpawner spawnLogic)
        {
            if (spawnLogic is GateSpawnSingle single)
            {
                if (inpRadius != null) inpRadius.text = single.radius.ToString(GameConst.FloatFormat);
                if (inpRandomSpanAngle != null) inpRandomSpanAngle.text = single.randomSpanAngle.ToString(GameConst.FloatFormat);
            }
            else if (spawnLogic is GateSpawnTriangle triangle)
            {
                if (inpRadius != null) inpRadius.text = triangle.radius.ToString(GameConst.FloatFormat);
                if (inpRandomSpanAngle != null) inpRandomSpanAngle.text = triangle.randomSpanAngle.ToString(GameConst.FloatFormat);
            }
            else if (spawnLogic is GateSpawnMultiple multiple)
            {
                if (inpAmount != null) inpAmount.text = multiple.amount.ToString();
                if (inpMaxRadius != null) inpMaxRadius.text = multiple.maxRadius.ToString(GameConst.FloatFormat);
                if (inpRandomSpanAngle != null) inpRandomSpanAngle.text = multiple.randomSpanAngle.ToString(GameConst.FloatFormat);
            }
            // GateSpawnCenter has no variables
        }

        public void Select()
        {
            objSelected.SetActive(true);
        }

        public void Deselect()
        {
            objSelected.SetActive(false);
        }
    }
}