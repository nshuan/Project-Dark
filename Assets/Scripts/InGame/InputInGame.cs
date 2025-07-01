using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace InGame
{
    public class InputInGame : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private Camera cam;
        [SerializeField] private Canvas canvas;
        [SerializeField] public PlayerCharacter playerVisual;
        public float holdThreshold = 0.5f;
        [SerializeField] private InputInGameDelayInfo delayInfo;
        private IMouseInput mouseInput;
        public PlayerStats PlayerStats { get; set; }
        public PlayerSkillConfig CurrentSkillConfig { get; set; }
        public Transform CursorRangeCenter => playerVisual.transform;
        public float CursorRangeRadius { get; set; }
        private bool IsMousePressing;
        private bool IsMousePressingStarted;
        private float holdDelayTime;
        
        private void Awake()
        {
            cam = Camera.main;

            LevelManager.Instance.OnLevelLoaded += (level) => { PlayerStats = LevelManager.Instance.PlayerStats; };
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

        private void Update()
        {
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
        }

        private void LateUpdate()
        {
            mouseInput?.OnUpdate();
        }

        // public void OnPointerClick(PointerEventData eventData)
        // {
        //     mouseInput?.OnMouseClick(delayInfo.skillDelay);
        // }

        private void OnDrawGizmos()
        {
            mouseInput?.OnDrawGizmos();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            holdDelayTime = 0f;
            IsMousePressing = false;
            IsMousePressingStarted = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            IsMousePressingStarted = false;
            
            if (!IsMousePressing)
            {
                mouseInput?.ResetChargeVariable();
                mouseInput?.OnMouseClick(delayInfo.skillDelay);
                return;
            }
            
            IsMousePressing = false;
            
            mouseInput?.OnHoldReleased();
            mouseInput?.OnMouseClick(delayInfo.skillDelay);
        }

        public void DelayCall(float delay, Action callback)
        {
            StartCoroutine(IEDelayCall(delay, callback));
        }

        private IEnumerator IEDelayCall(float delay, Action callback)
        {
            yield return new WaitForSeconds(delay);
            callback?.Invoke();
        }
    }

    [Serializable]
    public class InputInGameDelayInfo
    {
        public float skillDelay;
        public float specialSkillDelay;
    }
}