using System;
using System.Collections.Generic;
using Dark.Scripts.FrameByFrameAnimation;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace InGame
{
    public class Player8DirectionsAnimation : SerializedMonoBehaviour
    {
        [NonSerialized, OdinSerialize] private List<DirectionInfo> directionInfo;
        
        private float rotationThresholdGap; // Angle from up Vector
        private SpriteRenderer spriteRenderer;
        private int currentDirection;

        private PlayerSpritesAnimationInfo CurrentAnim { get; set; }
        private int currentFrame;
        private float timer;
        private bool charging;
        private Transform chargeFxLower;
        private Transform chargeFxUpper;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            rotationThresholdGap = 360f / directionInfo.Count;

            foreach (var info in directionInfo)
            {
                info.chargeAnim = new PlayerSpritesAnimationInfo()
                {
                    data = ScriptableObject.CreateInstance<FrameByFrameAnimation>(),
                    frameRate = info.attackAnim.frameRate,
                    autoExit = false
                };
                info.chargeAnim.data.frames = new Sprite[info.attackAnim.strikeFrameIndex];
                for (var i = 0; i < info.attackAnim.strikeFrameIndex; i++)
                {
                    info.chargeAnim.data.frames[i] = info.attackAnim.data.frames[i];
                }

                info.chargeAttackAnim = new PlayerSpritesAnimationInfo()
                {
                    data = ScriptableObject.CreateInstance<FrameByFrameAnimation>(),
                    frameRate = info.attackAnim.frameRate,
                    autoExit = true
                };
                info.chargeAttackAnim.data.frames = new Sprite[info.attackAnim.data.frames.Length - info.attackAnim.strikeFrameIndex];
                for (var i = info.attackAnim.strikeFrameIndex; i < info.attackAnim.data.frames.Length; i++)
                {
                    info.chargeAttackAnim.data.frames[i - info.attackAnim.strikeFrameIndex] = info.attackAnim.data.frames[i];
                }
            }
        }
        
        public void PlayIdle()
        {
            CurrentAnim = directionInfo[currentDirection].idleAnim;
            currentFrame = 0;
            spriteRenderer.sprite = CurrentAnim.data.frames[0];
            timer = 0f;
        }

        public (float, float) PlayAttack()
        {
            CurrentAnim = directionInfo[currentDirection].attackAnim;
            currentFrame = 0;
            spriteRenderer.sprite = CurrentAnim.data.frames[0];
            timer = 0f;
            return (CurrentAnim.frameRate * directionInfo[currentDirection].attackAnim.strikeFrameIndex,
                CurrentAnim.frameRate * CurrentAnim.data.frames.Length);
        }
        
        public float PlayCharge()
        {
            charging = true;
            CurrentAnim = directionInfo[currentDirection].chargeAnim;
            currentFrame = 0;
            spriteRenderer.sprite = CurrentAnim.data.frames[0];
            timer = 0f;
            return CurrentAnim.frameRate * CurrentAnim.data.frames.Length;
        }

        public void EndChargeAndShoot()
        {
            charging = false;
            CurrentAnim = directionInfo[currentDirection].chargeAttackAnim;
            currentFrame = 0;
            spriteRenderer.sprite = CurrentAnim.data.frames[0];
            timer = 0f;
        }
        
        public void PlaySpecialAttack()
        {
            CurrentAnim = directionInfo[currentDirection].specialAttackAnim;
            currentFrame = 0;
            spriteRenderer.sprite = CurrentAnim.data.frames[0];
            timer = 0f;
        }

        public void UpdateRotation(Vector2 direction)
        {
            var mag = direction.magnitude;
            if (mag > 1e-5f) {
                var angle = Mathf.Acos(Mathf.Clamp(direction.y / mag, -1f, 1f)) * Mathf.Rad2Deg;
                angle = direction.x < 0 ? 360f - angle : angle;
                var newDirection = (int)(angle / rotationThresholdGap + 1) % directionInfo.Count;
                if (newDirection != currentDirection)
                {
                    currentDirection = newDirection;
                    CurrentAnim = charging ? directionInfo[currentDirection].chargeAnim : directionInfo[currentDirection].idleAnim;

                    if (charging)
                    {
                        if (currentFrame < CurrentAnim.data.frames.Length) spriteRenderer.sprite = CurrentAnim.data.frames[currentFrame];
                        var showChargeLower = directionInfo[currentDirection].showChargeFxLower;
                        if (showChargeLower && chargeFxLower)
                        {
                            chargeFxLower.position = directionInfo[currentDirection].chargeFxPosition;
                            if (currentFrame >= CurrentAnim.data.frames.Length - 1)
                                chargeFxLower.gameObject.SetActive(true);
                            chargeFxUpper?.gameObject.SetActive(false);
                        }
                        else if (!showChargeLower && chargeFxUpper)
                        {
                            chargeFxUpper.position = directionInfo[currentDirection].chargeFxPosition;
                            if (currentFrame >= CurrentAnim.data.frames.Length - 1)
                                chargeFxUpper.gameObject.SetActive(true);
                            chargeFxLower?.gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        chargeFxLower?.gameObject.SetActive(false);
                        chargeFxUpper?.gameObject.SetActive(false);
                    }
                }
            }
        }
        
        private void Update()
        {
            if (CurrentAnim == null) return;
            timer += Time.deltaTime;

            if (charging)
            {
                
            }
            if (timer >= CurrentAnim.frameRate)
            {
                currentFrame += 1;
                if (currentFrame >= CurrentAnim.data.frames.Length)
                {
                    if (CurrentAnim.isLoop) currentFrame = 0;
                    else if (CurrentAnim.autoExit)
                    {
                        PlayIdle();
                        return;
                    }
                    else
                    {
                        timer -= CurrentAnim.frameRate;
                        currentFrame -= 1;
                        return;
                    }
                }
                spriteRenderer.sprite = CurrentAnim.data.frames[currentFrame];
                timer -= CurrentAnim.frameRate; // subtract instead of reset to avoid drift
            }
        }

        public void SetChargeFx(Transform fxLower, Transform fxUpper)
        {
            chargeFxLower = fxLower;
            chargeFxUpper = fxUpper;
        }
        
        [Serializable]
        public class DirectionInfo
        {
            public PlayerSpritesAnimationInfo idleAnim;
            public PlayerSpritesAttackAnimationInfo attackAnim;
            public PlayerSpritesAnimationInfo specialAttackAnim;
            public PlayerSpritesAnimationInfo chargeAnim;
            public PlayerSpritesAnimationInfo chargeAttackAnim;
            public Vector2 chargeFxPosition;
            public bool showChargeFxLower;
        }
    }
}