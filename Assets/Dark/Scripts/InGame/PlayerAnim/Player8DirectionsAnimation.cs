using System;
using System.Collections.Generic;
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
        private DirectionInfo currentDirection;
        private PlayerSpritesAnimationInfo currentAnim;
        private int currentFrame;
        private float timer;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            rotationThresholdGap = 360f / directionInfo.Count;
        }
        
        public void PlayIdle()
        {
            currentAnim = currentDirection.idleAnim;
            currentFrame = 0;
            spriteRenderer.sprite = currentAnim.data.frames[0];
            timer = 0f;
        }

        public (float, float) PlayAttack()
        {
            currentAnim = currentDirection.attackAnim;
            currentFrame = 0;
            spriteRenderer.sprite = currentAnim.data.frames[0];
            timer = 0f;
            return (currentDirection.attackAnim.frameRate * currentDirection.attackAnim.strikeFrameIndex,
                    currentDirection.attackAnim.frameRate * currentDirection.attackAnim.data.frames.Length);
        }
        
        public void PlaySpecialAttack()
        {
            currentAnim = currentDirection.specialAttackAnim;
            currentFrame = 0;
            spriteRenderer.sprite = currentAnim.data.frames[0];
            timer = 0f;
        }

        public void UpdateRotation(Vector2 direction)
        {
            var mag = direction.magnitude;
            if (mag > 1e-5f) {
                var angle = Mathf.Acos(Mathf.Clamp(direction.y / mag, -1f, 1f)) * Mathf.Rad2Deg;
                angle = direction.x < 0 ? 360f - angle : angle;
                currentDirection = directionInfo[(int)(angle / rotationThresholdGap + 1) % directionInfo.Count];
                currentAnim = currentDirection.idleAnim;
            }
        }
        
        private void Update()
        {
            if (currentAnim == null) return;
            timer += Time.deltaTime;

            if (timer >= currentAnim.frameRate)
            {
                currentFrame += 1;
                if (currentFrame >= currentAnim.data.frames.Length)
                {
                    if (currentAnim.isLoop) currentFrame = 0;
                    else if (currentAnim.autoExit)
                    {
                        PlayIdle();
                        return;
                    }
                    else
                    {
                        timer -= currentAnim.frameRate;
                        return;
                    }
                }
                spriteRenderer.sprite = currentAnim.data.frames[currentFrame];
                timer -= currentAnim.frameRate; // subtract instead of reset to avoid drift
            }
        }
        
        [Serializable]
        public class DirectionInfo
        {
            public PlayerSpritesAnimationInfo idleAnim;
            public PlayerSpritesAttackAnimationInfo attackAnim;
            public PlayerSpritesAnimationInfo specialAttackAnim;
        }
    }
}