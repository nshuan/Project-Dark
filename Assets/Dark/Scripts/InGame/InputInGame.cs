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
        public Transform ProjectileSpawnPos => PlayerVisual.transform;
        private List<MoveTowersConfig> availableTeleConfigs;
        private bool BlockAllInput { get; set; }
        public bool BlockTeleport { get; set; }
        public float CursorRangeRadius { get; set; }
        private bool IsMousePressing;
        private bool IsMousePressingStarted;
        private float holdDelayTime;
        private IMouseInput mouseInput;
        private IMoveTowerMouseInput teleMouseInput;
        private IMoveMouseInput collectorMouseInput;
        private IMouseInput mouseAutoAttack;
        private MonoCursor cursor;
        private PointerEventData.InputButton pressingButton = PointerEventData.InputButton.Middle;

        #region Move Towers

        [Space, Header("Move Towers")] 
        private KeyCode activateTeleKey = KeyCode.LeftShift;

        #endregion

        #region Charge

        [SerializeField] private MoveChargeController chargeControllerArcher;

        #endregion
        
        private void Awake()
        {
            BlockAllInput = true;
            
            
            LevelManager.Instance.OnLevelLoaded += (level) =>
            {
                PlayerVisual = LevelManager.Instance.Player;
                
                availableTeleConfigs = new List<MoveTowersConfig>();
                if (LevelUtility.BonusInfo.unlockedMoveToTower == null || LevelUtility.BonusInfo.unlockedMoveToTower.Count == 0)
                    availableTeleConfigs.Add(LevelManager.Instance.defaultTeleConfig);
                else
                {
                    foreach (var moveId in LevelUtility.BonusInfo.unlockedMoveToTower)
                    {
                        if (moveId == 1) availableTeleConfigs.Add(LevelManager.Instance.flashConfig);
                        else if (moveId == 2) availableTeleConfigs.Add(LevelManager.Instance.dashConfig);
                    }
                }
                teleMouseInput = new MoveToTower(cam, this, PlayerVisual, availableTeleConfigs[0], availableTeleConfigs.Count > 1 ? availableTeleConfigs[1] : null, LevelManager.Instance.Towers, LevelManager.Instance.CurrentTower.Id, this.TryDelayCall);
                teleMouseInput.OnActivated();
                BlockAllInput = false;
                
                // Setup skill config and mouse input
                CursorRangeRadius = LevelUtility.CurrentSkill.range;
            
                if (mouseInput != null)
                {
                    mouseInput.Dispose();
                    mouseInput = null;
                }
                cursor ??= ShotCursorManager.Instance.GetPrefab(LevelUtility.CurrentSkill.shootLogic.cursorType, canvas.transform);
                cursor.gameObject.SetActive(true);
                mouseInput = ShotCursorManager.Instance.GetCursorMoveLogic(LevelUtility.CurrentSkill.shootLogic.cursorType, cam, cursor);
                mouseInput.Initialize(this, chargeControllerArcher);
                mouseInput.ResetChargeVariable();

                if (mouseAutoAttack != null)
                {
                    mouseAutoAttack.Dispose();
                    mouseAutoAttack = null;
                }
                mouseAutoAttack = new MoveAutoAttack(cam, cursor);
                mouseAutoAttack.Initialize(this, null);
                
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

            ResetMotionBlur();
            ResetTimeScale();
        }

        private void Update()
        {
            if (BlockAllInput)
            {
                return;
            }

            if (!BlockTeleport)
            {
                // if (Input.GetKey(activateTeleKey))
                // {
                //     if (teleMouseInput.CanMove)
                //     {
                //         ResetMousePressing();
                //         
                //         IsMousePressingStarted = false;
                //         IsMousePressing = false;
                //         mouseInput.ResetChargeVariable();
                //         mouseInput.OnHoldReleased();
                //         teleKeyPressed = true;
                //         
                //         teleMouseInput.OnActivated();
                //     
                //         DOTween.Kill(motionBlur);
                //         DOTween.Sequence(motionBlur).AppendCallback(() =>
                //             {
                //                 foreach (var tower in LevelManager.Instance.Towers)
                //                 {
                //                     tower.OnMotionBlur();
                //                 }
                //                 PlayerVisual.OnMotionBlur();
                //             
                //                 motionBlur.gameObject.SetActive(true);
                //             }).Append(motionBlur.DOFade(1f, 0.16f))
                //             .OnComplete(FreezeTimeScale);
                //     }
                // }
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
                    if (mouseInput.CanCharge) mouseAutoAttack?.OnHoldStarted();
                    mouseInput?.OnHoldStarted();
                }
            }
        }

        private void LateUpdate()
        {
            if (BlockAllInput) return;
            
            mouseInput?.OnUpdate();
            mouseAutoAttack?.OnUpdate();
            teleMouseInput?.OnUpdate();
        }

        private void OnDrawGizmos()
        {
            mouseInput?.OnDrawGizmos();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (BlockAllInput) return;
            
            // Check nếu đang giữ chuột trái thì không bấm được chuột phải
            // Còn nếu đang dí chuột phải auto mà bấm chuột trái thì được
            if (pressingButton == PointerEventData.InputButton.Left && eventData.button == PointerEventData.InputButton.Right) return;
            
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                pressingButton = PointerEventData.InputButton.Left;
                
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
                // Reset biên lưu nút chuột đang nhấn
                if (pressingButton == PointerEventData.InputButton.Left)
                    pressingButton = PointerEventData.InputButton.Middle;
                
                if (!BlockTeleport)
                    teleMouseInput?.OnMouseClick(false);
                teleMouseInput?.OnDeactivated();
                
                IsMousePressingStarted = false;
                
                if (!IsMousePressing)
                {
                    mouseInput?.OnMouseClick();
                    mouseInput?.ResetChargeVariable();
                    return;
                }
                
                IsMousePressing = false;
                
                mouseAutoAttack?.OnHoldReleased();
                mouseInput?.OnHoldReleased();
                mouseInput?.OnMouseClick();
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

        public void ActiveMotionBlur()
        {
            DOTween.Kill(motionBlur);
            DOTween.Sequence(motionBlur).AppendCallback(() =>
                {
                    foreach (var tower in LevelManager.Instance.Towers)
                    {
                        tower.OnMotionBlur();
                    }
                    PlayerVisual.OnMotionBlur();
                            
                    motionBlur.gameObject.SetActive(true);
                }).Append(motionBlur.DOFade(1f, 0.32f))
                .OnComplete(FreezeTimeScale);    
        }
        
        public void ResetMotionBlur()
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

        private void ResetMousePressing()
        {
            // Reset biên lưu nút chuột đang nhấn
            if (pressingButton == PointerEventData.InputButton.Left)
            {
                pressingButton = PointerEventData.InputButton.Middle;
                    
                IsMousePressingStarted = false;
                mouseAutoAttack?.OnHoldReleased();
                    
                if (!IsMousePressing)
                {
                    mouseInput?.OnMouseClick();
                    mouseInput?.ResetChargeVariable();
                    return;
                }
                    
                IsMousePressing = false;
                    
                mouseInput?.OnHoldReleased();
                mouseInput?.OnMouseClick();
            }
        }

        #region Pause Game

        public void OnPause(bool isPaused)
        {
            ResetMousePressing();
            BlockAllInput = isPaused;
            cursor?.gameObject.SetActive(!isPaused);
        }
        
        #endregion
    }
}