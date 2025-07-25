using System;
using Dark.Scripts.FrameByFrameAnimation;
using UnityEngine;

namespace InGame
{
    public class PlayerSpritesAnimation : MonoBehaviour
    {
        [SerializeField] private PlayerSpritesAnimationInfo idleAnim;
        [SerializeField] private PlayerSpritesAttackAnimationInfo attackAnim;
        [SerializeField] private PlayerSpritesAnimationInfo specialAttackAnim;
        
        private SpriteRenderer spriteRenderer;
        private PlayerSpritesAnimationInfo currentAnim;
        private int currentFrame;
        private float timer;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
        public void PlayIdle()
        {
            currentAnim = idleAnim;
            currentFrame = 0;
            spriteRenderer.sprite = currentAnim.data.frames[0];
            timer = 0f;
        }

        public float PlayAttack()
        {
            currentAnim = attackAnim;
            currentFrame = 0;
            spriteRenderer.sprite = currentAnim.data.frames[0];
            timer = 0f;
            return attackAnim.frameRate * attackAnim.strikeFrameIndex;
        }
        
        public void PlaySpecialAttack()
        {
            currentAnim = specialAttackAnim;
            currentFrame = 0;
            spriteRenderer.sprite = currentAnim.data.frames[0];
            timer = 0f;
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
    }

    [Serializable]
    public class PlayerSpritesAnimationInfo
    {
        public FrameByFrameAnimation data;
        public bool isLoop;
        public float frameRate = 0.1f;
        public bool autoExit = true;
    }

    [Serializable]
    public class PlayerSpritesAttackAnimationInfo : PlayerSpritesAnimationInfo
    {
        public int strikeFrameIndex;
    }
}