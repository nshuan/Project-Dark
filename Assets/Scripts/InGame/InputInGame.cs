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
        private bool teleKeyPressing;
        private bool teleKeyPressed;
        public float holdTeleThreshold = 0.5f;
        private float holdTeleDelayTime;

        #endregion
        
        private void Awake()
        {
            cam = Camera.main;

            LevelManager.Instance.OnLevelLoaded += (level) =>
            {
                PlayerStats = LevelManager.Instance.PlayerStats;
                teleMouseInput = new MoveToTower(cam, LevelManager.Instance.shortTeleConfig, LevelManager.Instance.longTeleConfig, LevelManager.Instance.Towers, LevelManager.Instance.CurrentTower.Id, DelayCall);
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
            if (Input.GetKey(activateTeleKey))
            {
                IsMousePressingStarted = false;
                IsMousePressing = false;
                mouseInput.ResetChargeVariable();
                mouseInput.OnHoldReleased();

                holdTeleDelayTime += Time.deltaTime;
                if (teleKeyPressing == false)
                {
                    if (holdTeleDelayTime >= holdTeleThreshold)
                    {
                        teleKeyPressing = true;
                        teleMouseInput.OnActivated();
                    }
                }
            }

            if (Input.GetKeyUp(activateTeleKey))
            {
                teleKeyPressing = false;
                if (holdTeleDelayTime < holdTeleThreshold)
                {
                    teleKeyPressed = true;
                    teleMouseInput.OnActivated();
                }
                else
                {
                    teleMouseInput?.OnDeactivated();
                }

                holdTeleDelayTime = 0f;
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
                if (teleKeyPressing) return;
                
                holdDelayTime = 0f;
                IsMousePressing = false;
                IsMousePressingStarted = true;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (teleKeyPressing)
                {
                    IsMousePressingStarted = false;
                    IsMousePressing = false;
                    return;
                }
                
                if (teleKeyPressed)
                {
                    teleKeyPressed = false;
                    teleMouseInput?.OnDeactivated();
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
                teleMouseInput?.OnDeactivated();
                
                if (teleKeyPressing)
                {
                    teleKeyPressing = false;
                    teleKeyPressed = false;
                    teleMouseInput?.OnMouseClick(isLongTele: true);
                    DebugUtility.LogWarning("Use LONG teleport!!!");
                    return;
                }

                if (teleKeyPressed)
                {
                    teleKeyPressing = false;
                    teleKeyPressed = false;
                    teleMouseInput?.OnMouseClick(isLongTele: false);
                    DebugUtility.LogWarning("Use SHORT teleport!!!");
                    return;
                }
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
    }
}