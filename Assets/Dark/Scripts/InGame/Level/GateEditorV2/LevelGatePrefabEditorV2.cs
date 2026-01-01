using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace InGame.GateEditorV2
{
    public class LevelGatePrefabEditorV2 : MonoBehaviour, IBeginDragHandler, IDragHandler, IPointerClickHandler
    {
        public GameObject parentVfx;
        
        public bool Selecting { get; private set; }
        public Action<LevelGatePrefabEditorV2> OnClick { get; set; }
        public Action<Vector2> OnDragging { get; set; }
        public GateConfig Config { get; set; }
        
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
        
        private void Awake()
        {
            vfx = Instantiate(parentVfx, null).transform;
            objSelect = vfx.Find("SpriteSelect").gameObject;
            camera = Camera.main;
            rectTransform = GetComponent<RectTransform>();
        }

        public void UpdateUI(GateConfig gate)
        {
            Config = new GateConfig()
            {
                isBossGate = gate.isBossGate,
                position = gate.position,
                targetBaseIndex = gate.targetBaseIndex,
                startTime = gate.startTime,
                duration = gate.duration,
                spawnType = gate.spawnType,
                intervalLoop = gate.intervalLoop,
                spawnLogic = gate.spawnLogic,
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
            var pos = camera.ScreenToWorldPoint(eventData.position);
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