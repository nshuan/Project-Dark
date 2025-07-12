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
        public PlayerStats PlayerStats { get; set; }
        public PlayerSkillConfig CurrentSkillConfig { get; set; }
        public Transform CursorRangeCenter => playerVisual.transform;
        public float CursorRangeRadius { get; set; }
        private bool IsMousePressing;
        private bool IsMousePressingStarted;
        private float holdDelayTime;
        private bool blockInput;
        private IMouseInput mouseInput;
        private IRightMouseInput teleMouseInput;

        #region Move Towers

        [Space, Header("Move Towers")] 
        private KeyCode activateTeleKey = KeyCode.LeftShift;
        private bool teleKeyPressed;

#if UNITY_EDITOR
        [Space, Header("Editor cheat")] 
        [SerializeField] private bool forceEnableDash;
#endif

        #endregion
        
        private void Awake()
        {
            cam = Camera.main;

            LevelManager.Instance.OnLevelLoaded += (level) =>
            {
#if UNITY_EDITOR
                if (forceEnableDash) LevelUtility.BonusInfo.unlockedLongMoveToTower = true;
#endif
                PlayerStats = LevelManager.Instance.PlayerStats;
                teleMouseInput = new MoveToTower(cam, playerVisual, LevelManager.Instance.shortTeleConfig, LevelManager.Instance.longTeleConfig, LevelManager.Instance.Towers, LevelManager.Instance.CurrentTower.Id, DelayCall);
            };
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
            mouseInput.Initialize(this);
            mouseInput.ResetChargeVariable();
        }

        private void Update()
        {
#if UNITY_EDITOR
            // Test tower change
            for (var i = 1; i <= 3; i++)
            {
                if (Input.GetKeyDown(i.ToString()))
                {
                    teleMouseInput?.OnActivated();
                    teleMouseInput?.OnMouseClick(false);
                    return;
                }
            }
#endif
            
            if (Input.GetKeyDown(activateTeleKey))
            {
                if (teleMouseInput.CanMove)
                {
                    IsMousePressingStarted = false;
                    IsMousePressing = false;
                    mouseInput.ResetChargeVariable();
                    mouseInput.OnHoldReleased();
                    teleKeyPressed = true;
                    FreezeTimeScale();
                }
            }

            if (Input.GetKeyUp(activateTeleKey))
            {
                teleMouseInput.OnActivated();
            }
            
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
            teleMouseInput?.OnUpdate();
        }

        private void OnDrawGizmos()
        {
            mouseInput?.OnDrawGizmos();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (teleKeyPressed) return;
                
                holdDelayTime = 0f;
                IsMousePressing = false;
                IsMousePressingStarted = true;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (teleKeyPressed)
                {
                    teleKeyPressed = false;
                    teleMouseInput?.OnMouseClick(false);
                    teleMouseInput?.OnDeactivated();
                    ResetTimeScale();
                    return;
                }
                
                IsMousePressingStarted = false;
                
                if (!IsMousePressing)
                {
                    mouseInput?.ResetChargeVariable();
                    mouseInput?.OnMouseClick();
                    return;
                }
                
                IsMousePressing = false;
                
                mouseInput?.OnHoldReleased();
                mouseInput?.OnMouseClick();
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                teleKeyPressed = false;
                teleMouseInput?.OnMouseClick(true);
                teleMouseInput?.OnDeactivated();
                ResetTimeScale();
            }
        }

        public bool DelayCall(float delay, Action callback)
        {
            StartCoroutine(IEDelayCall(delay, callback));
            return true;
        }

        private IEnumerator IEDelayCall(float delay, Action callback)
        {
            yield return new WaitForSeconds(delay);
            callback?.Invoke();
        }

        private void FreezeTimeScale()
        {
            Time.timeScale = 0.1f;
        }

        private void ResetTimeScale()
        {
            Time.timeScale = 1f;
        }
    }
}