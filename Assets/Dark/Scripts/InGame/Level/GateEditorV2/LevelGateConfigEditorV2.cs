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