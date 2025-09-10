using System;
using System.Collections;
using System.Collections.Generic;
using Dark.Scripts.Utils;
using DG.Tweening;
using InGame.Pause;
using UnityEngine;
using UnityEngine.EventSystems;

namespace InGame
{
    public class InputInGame : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private Camera cam;
        [SerializeField] private Canvas canvas;
		[SerializeField] private CanvasGroup motionBlur;
        public float holdThreshold = 0.5f;
        public PlayerCharacter PlayerVisual { get; set; }
        public PlayerStats PlayerStats { get; set; }
        public PlayerSkillConfig CurrentSkillConfig { get; set; }
        public Transform CursorRangeCenter => PlayerVisual.transform;
        private List<MoveTowersConfig> availableTeleConfigs;
        private bool BlockAllInput { get; set; }
        public bool BlockTeleport { get; set; }
        public float CursorRangeRadius { get; set; }
        private bool IsMousePressing;
        private bool IsMousePressingStarted;
        private float holdDelayTime;
        private IMouseInput mouseInput;
        private IRightMouseInput teleMouseInput;
        private MonoCursor cursor;

        #region Move Towers

        [Space, Header("Move Towers")] 
        private KeyCode activateTeleKey = KeyCode.LeftShift;
        private bool teleKeyPressed;

        #endregion

        #region Charge

        [SerializeField] private MoveChargeController chargeControllerArcher;

        #endregion
        
        private void Awake()
        {
            BlockAllInput = true;
            
            availableTeleConfigs = new List<MoveTowersConfig>();
            
            LevelManager.Instance.OnLevelLoaded += (level) =>
            {
                PlayerVisual = LevelManager.Instance.Player;
                PlayerStats = LevelManager.Instance.PlayerStats;
                
                if (LevelUtility.BonusInfo.unlockedMoveToTower == null || LevelUtility.BonusInfo.unlockedMoveToTower.Count == 0)
                    availableTeleConfigs.Add(LevelManager.Instance.defaultTeleConfig);
                else
                {
                    foreach (var moveId in LevelUtility.BonusInfo.unlockedMoveToTower)
                    {
                        if (moveId == 1) availableTeleConfigs.Add(LevelManager.Instance.shortTeleConfig);
                        else if (moveId == 2) availableTeleConfigs.Add(LevelManager.Instance.longTeleConfig);
                    }
                }
                teleMouseInput = new MoveToTower(cam, PlayerVisual, availableTeleConfigs[0], availableTeleConfigs.Count > 1 ? availableTeleConfigs[1] : null, LevelManager.Instance.Towers, LevelManager.Instance.CurrentTower.Id, this.TryDelayCall);
                BlockAllInput = false;
                
                // Setup skill config and mouse input
                CurrentSkillConfig = LevelManager.Instance.SkillConfig;
                CursorRangeRadius = CurrentSkillConfig.range;
            
                if (mouseInput != null)
                {
                    mouseInput.Dispose();
                    mouseInput = null;
                }
                cursor ??= ShotCursorManager.Instance.GetPrefab(CurrentSkillConfig.shootLogic.cursorType, canvas.transform);
                cursor.gameObject.SetActive(true);
                mouseInput = ShotCursorManager.Instance.GetCursorMoveLogic(CurrentSkillConfig.shootLogic.cursorType, cam, cursor);
                mouseInput.Initialize(this, chargeControllerArcher);
                mouseInput.ResetChargeVariable();
                
                LevelManager.Instance.OnWin += OnLevelCompleted;
                LevelManager.Instance.OnLose += OnLevelCompleted;
            };

            PauseGame.Instance.onPause -= OnPause;
            PauseGame.Instance.onPause += OnPause;
        }

        private void OnLevelCompleted()
        {
            PauseGame.Instance.onPause -= OnPause;
            
            BlockAllInput = true;
            IsMousePressing = false;
            IsMousePressingStarted = false;
            teleKeyPressed = false;

            ResetMotionBlur();
            ResetTimeScale();
        }

        private void Update()
        {
            if (BlockAllInput)
            {
                return;
            }
            
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

            if (!BlockTeleport)
            {
                if (Input.GetKey(activateTeleKey))
                {
                    if (teleMouseInput.CanMove)
                    {
                        IsMousePressingStarted = false;
                        IsMousePressing = false;
                        mouseInput.ResetChargeVariable();
                        mouseInput.OnHoldReleased();
                        teleKeyPressed = true;
                        
                        teleMouseInput.OnActivated();
                    
                        DOTween.Kill(motionBlur);
                        DOTween.Sequence(motionBlur).AppendCallback(() =>
                            {
                                foreach (var tower in LevelManager.Instance.Towers)
                                {
                                    tower.OnMotionBlur();
                                }
                                PlayerVisual.OnMotionBlur();
                            
                                motionBlur.gameObject.SetActive(true);
                            }).Append(motionBlur.DOFade(1f, 0.16f))
                            .OnComplete(FreezeTimeScale);
                    }
                }
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
            if (BlockAllInput) return;
            
            mouseInput?.OnUpdate();
            teleMouseInput?.OnUpdate();
        }

        private void OnDrawGizmos()
        {
            mouseInput?.OnDrawGizmos();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (BlockAllInput) return;
            
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
            if (BlockAllInput) return;
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (teleKeyPressed)
                {
                    teleKeyPressed = false;
                    if (!BlockTeleport)
                        teleMouseInput?.OnMouseClick(false);
                    teleMouseInput?.OnDeactivated();
                    ResetMotionBlur();
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
                if (teleKeyPressed)
                {
                    teleKeyPressed = false;
                    if (!BlockTeleport)
                        teleMouseInput?.OnMouseClick(true);
                    teleMouseInput?.OnDeactivated();
                    ResetMotionBlur();
                    ResetTimeScale();
                }
            }
        }

        private void FreezeTimeScale()
        {
            Time.timeScale = 0.1f;
        }

        private void ResetTimeScale()
        {
            Time.timeScale = 1f;
        }

        private void ResetMotionBlur()
        {
            DOTween.Kill(motionBlur);
            DOTween.Sequence(motionBlur).Append(motionBlur.DOFade(0f, 0.16f))
                .OnComplete(() =>
                {
                    motionBlur.gameObject.SetActive(false);
                    foreach (var tower in LevelManager.Instance.Towers)
                    {
                        tower.OnEndMotionBlur();
                    }
                    PlayerVisual.OnEndMotionBlur();
                })
                .Play();
        }

        #region Pause Game

        public void OnPause(bool isPaused)
        {
            BlockAllInput = isPaused;
            cursor?.gameObject.SetActive(!isPaused);
        }
        
        #endregion
    }
}