using System;
using UnityEngine;

namespace InGame.EnemyEffect
{
    public class EnemySpritesAnimation : MonoBehaviour
    {
        [SerializeField] private EnemySpritesAnimationInfo idleAnim;
        [SerializeField] private EnemySpritesAnimationInfo runAnim;
        [SerializeField] private EnemySpritesAnimationInfo attackAnim;
        [SerializeField] private EnemySpritesAnimationInfo hitAnim;
        [SerializeField] private EnemySpritesAnimationInfo dieAnim;
        [SerializeField] private EnemySpritesAnimationInfo spawnAnim;
        public bool isDefaultRun;
        
        private SpriteRenderer spriteRenderer;
        private EnemySpritesAnimationInfo currentAnim;
        private int currentFrame;
        private float timer;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public float PlaySpawn()
        {
            currentAnim = spawnAnim;
            currentFrame = 0;
            spriteRenderer.sprite = currentAnim.frames[0];
            timer = 0;
            return spawnAnim.frames.Length * spawnAnim.frameRate;
        }
        
        public void PlayIdle()
        {
            currentAnim = idleAnim;
            currentFrame = 0;
            spriteRenderer.sprite = currentAnim.frames[0];
            timer = 0f;
        }
        
        public void PlayRun()
        {
            currentAnim = runAnim;
            currentFrame = 0;
            spriteRenderer.sprite = currentAnim.frames[0];
            timer = 0f;
        }

        public void PlayAttack()
        {
            currentAnim = attackAnim;
            currentFrame = 0;
            spriteRenderer.sprite = currentAnim.frames[0];
            timer = 0f;
        }
        
        public void PlayHit()
        {
            currentAnim = hitAnim;
            currentFrame = 0;
            spriteRenderer.sprite = currentAnim.frames[0];
            timer = 0f;
        }

        public void PlayDie()
        {
            currentAnim = dieAnim;
            currentFrame = 0;
            spriteRenderer.sprite = currentAnim.frames[0];
            timer = 0f;
        }

        private void Update()
        {
            if (currentAnim == null) return;
            timer += Time.deltaTime;

            if (timer >= currentAnim.frameRate)
            {
                currentFrame += 1;
                if (currentFrame >= currentAnim.frames.Length)
                {
                    if (currentAnim.isLoop) currentFrame = 0;
                    else if (currentAnim.autoExit)
                    {
                        if (isDefaultRun) PlayRun();
                        else PlayIdle();
                        return;
                    }
                    else
                    {
                        timer -= currentAnim.frameRate;
                        return;
                    }
                }
                spriteRenderer.sprite = currentAnim.frames[currentFrame];
                timer -= currentAnim.frameRate; // subtract instead of reset to avoid drift
            }
        }
    }

    [Serializable]
    public class EnemySpritesAnimationInfo
    {
        public Sprite[] frames;
        public bool isLoop;
        public float frameRate = 0.1f;
        public bool autoExit = true;
    }
}