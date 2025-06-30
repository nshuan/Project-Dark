using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace InGame
{
    public class InputInGame : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        private Camera cam;
        [SerializeField] private Canvas canvas;
        private IMouseInput mouseInput;
        public PlayerStats PlayerStats { get; set; }
        public PlayerSkillConfig CurrentSkillConfig { get; set; }
        public Transform CursorRangeCenter { get; set; }
        public float CursorRangeRadius { get; set; }
        public float holdThreshold = 0.5f;
        private bool IsMousePressing;
        private bool IsMousePressingStarted;
        private float holdTime;
        private float holdDelayTime;

        private void Awake()
        {
            cam = Camera.main;

            LevelManager.Instance.OnLevelLoaded += (level) => { PlayerStats = LevelManager.Instance.PlayerStats; };
            LevelManager.Instance.OnChangeTower += OnTowerChanged;
            LevelManager.Instance.OnChangeSkill += OnSkillChanged;
        }

        private void OnSkillChanged(PlayerSkillConfig skillConfig)
        {
            if (!skillConfig) return;

            CurrentSkillConfig = skillConfig;
            CursorRangeRadius = skillConfig.range;
            
            if (mouseInput != null)
            {
                mouseInput.Dispose();
                mouseInput = null;
            }
            var cursor = ShotCursorManager.Instance.GetPrefab(skillConfig.shootLogic.cursorType, canvas.transform);
            mouseInput = ShotCursorManager.Instance.GetCursorMoveLogic(skillConfig.shootLogic.cursorType, cam, cursor);
            mouseInput.InputManager = this;
            mouseInput.ResetChargeVariable();
        }

        private void OnTowerChanged(Transform towerTransform)
        {
            CursorRangeCenter = towerTransform;
        }

        private void Update()
        {
            mouseInput?.OnUpdate();

            if (IsMousePressingStarted)
            {
                if (holdDelayTime < holdThreshold)
                {
                    holdDelayTime += Time.deltaTime;
                }
                else
                {
                    IsMousePressingStarted = false;
                    IsMousePressing = true;
                    mouseInput?.OnHoldStarted();
                }
            }

            if (IsMousePressing)
            {
                holdTime += Time.deltaTime;
            }
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            mouseInput?.OnMouseClick();
        }

        private void OnDrawGizmos()
        {
            mouseInput?.OnDrawGizmos();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            holdTime = 0f;
            holdDelayTime = 0f;
            IsMousePressingStarted = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!IsMousePressing) return;
            IsMousePressing = false;

            mouseInput?.OnHoldReleased();
        }
    }
}