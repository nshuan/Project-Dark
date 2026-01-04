using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace InGame.GateEditorV2
{
    public class LevelGatePrefabEditorV2 : MonoBehaviour, IBeginDragHandler, IDragHandler, IPointerClickHandler
    {
        public GameObject parentVfx;
        public RectTransform linePrefab;
        public TextMeshProUGUI txtLineDistancePrefab;
        public TextMeshProUGUI txtGateLabel;
        
        public bool Selecting { get; private set; }
        public Action<LevelGatePrefabEditorV2> OnClick { get; set; }
        public Action<Vector2> OnDragging { get; set; }
        public GateConfig Config { get; set; }
        public Vector2[] TargetPositions { get; set; }
        
        // public bool IsBossGate { get; set; }
        public Vector2 Position { get; set; }
        // public string StrTargetTowers { get; set; }
        // public float StartTime { get; set; }
        // public float Duration { get; set; }
        // public int SpawnType { get; set; }
        // public float Interval { get; set; }
        // public float StartTimeVisual { get; set; }
        // public float DurationVisual { get; set; }

        public Transform vfx;
        private Camera camera;
        private RectTransform rectTransform;
        private GameObject objSelect;
        private List<RectTransform> lines;
        private List<TextMeshProUGUI> txtLines;
        
        private void Awake()
        {
            vfx = Instantiate(parentVfx, null).transform;
            objSelect = vfx.Find("SpriteSelect").gameObject;
            camera = Camera.main;
            rectTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            if (TargetPositions == null || TargetPositions.Length == 0) return;
            if (lines == null) InitLines();
            for (var i = 0; i < TargetPositions.Length; i++)
            {
                var line = lines[i];
                var direction = TargetPositions[i] - (Vector2)transform.position;
                line.sizeDelta = new Vector2(5f, direction.magnitude);
                line.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f);

                var txtLine = txtLines[i];
                txtLine.transform.position = (TargetPositions[i] + (Vector2)transform.position) / 2;
                txtLine.transform.rotation = Quaternion.identity;
                txtLine.SetText($"{((Vector2)camera.ScreenToWorldPoint(TargetPositions[i]) - Position).magnitude.ToString(GameConst.FloatFormat)}");
            }
        }

        public void UpdateUI(GateConfig gate)
        {
            var newTargets = new List<int>();
            if (gate.targetBaseIndex != null)
            {
                newTargets.AddRange(gate.targetBaseIndex);
            }

            IGateSpawner newSpawnLogic;
            if (gate.spawnLogic is GateSpawnSingle existingSingle)
            {
                newSpawnLogic = new GateSpawnSingle() { radius = existingSingle.radius, randomSpanAngle = existingSingle.randomSpanAngle };
            }
            else if (gate.spawnLogic is GateSpawnTriangle existingTriangle)
            {
                newSpawnLogic = new GateSpawnTriangle() {  radius = existingTriangle.radius, randomSpanAngle = existingTriangle.randomSpanAngle };
            }
            else if (gate.spawnLogic is GateSpawnMultiple existingMultiple)
            {
                newSpawnLogic = new GateSpawnMultiple() { amount = existingMultiple.amount, randomSpanAngle = existingMultiple.randomSpanAngle, maxRadius = existingMultiple.maxRadius };
            }
            else
            {
                newSpawnLogic = new GateSpawnCenter();
            }
            

            Config = new GateConfig()
            {
                isBossGate = gate.isBossGate,
                position = new Vector2(gate.position.x, gate.position.y),
                targetBaseIndex = newTargets.ToArray(),
                startTime = gate.startTime,
                duration = gate.duration,
                spawnType = gate.spawnType,
                intervalLoop = gate.intervalLoop,
                spawnLogic = newSpawnLogic,
                startTimeVisual = gate.startTimeVisual,
                durationVisual = gate.durationVisual,
            };
            // Position = gate.position;
            // IsBossGate = gate.isBossGate;
            // StrTargetTowers = string.Join(", ", gate.targetBaseIndex);
            // StartTime = gate.startTime;
            // Duration = gate.duration;
            // SpawnType = gate.spawnType.enemyId;
            // Interval = gate.intervalLoop;
            // StartTimeVisual = gate.startTimeVisual;
            // DurationVisual = gate.durationVisual;
            
            transform.position = camera.WorldToScreenPoint(gate.position);
            vfx.position = gate.position;
            Position = vfx.position;
        }

        private void InitLines()
        {
            lines = new List<RectTransform>();
            txtLines = new List<TextMeshProUGUI>();
            for (var i = 0; i < TargetPositions.Length; i++)
            {
                var line = Instantiate(linePrefab, transform);
                var direction = TargetPositions[i] - (Vector2)transform.position;
                line.localPosition = Vector3.zero;
                line.sizeDelta = new Vector2(5f, direction.magnitude);
                line.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f);
                lines.Add(line);
                
                var txtLine = Instantiate(txtLineDistancePrefab, line.transform);
                txtLine.transform.position = (TargetPositions[i] + (Vector2)transform.position) / 2;
                txtLine.transform.rotation = Quaternion.identity;
                txtLine.SetText($"{((Vector2)camera.ScreenToWorldPoint(TargetPositions[i]) - Position).magnitude.ToString(GameConst.FloatFormat)}");
                txtLines.Add(txtLine);
            }
        }
        
        public void SelectGate()
        {
            Selecting = true;
            objSelect.SetActive(true);
        }

        public void DeselectGate()
        {
            Selecting = false;
            objSelect.SetActive(false);
        }

        private void OnEnable()
        {
            vfx?.gameObject.SetActive(true);
        }

        private void OnDisable()
        {
            if (vfx && vfx.gameObject)
                vfx.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            if (vfx && vfx.gameObject)
                Destroy(vfx.gameObject);
        }

        public void OnDrag(PointerEventData eventData)
        {
            rectTransform.position += (Vector3)eventData.delta;
            var pos = camera.ScreenToWorldPoint(rectTransform.position);
            pos.z = 0;
            vfx.position = pos;
            Position = pos;
            OnDragging?.Invoke(pos);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            OnClick?.Invoke(this);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick?.Invoke(this);
        }
    }
}