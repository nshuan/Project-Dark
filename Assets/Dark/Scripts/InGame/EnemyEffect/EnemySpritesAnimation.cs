using System;
using System.Collections;
using CustomAnimations;
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
        [SerializeField] private SpritesAnimation spawnEffect;
        [SerializeField] private SpriteRenderer spriteRenderer;
		
        public bool isDefaultRun;
        
        private EnemySpritesAnimationInfo currentAnim;
        private int currentFrame;
        private float timer;
        private bool isSpawningEffect = false;
        private float spawningTimer;
        private Vector3 originalScale;
        private bool isPause;
        
        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            originalScale = spriteRenderer.transform.localScale;
        }

        public float PlayCustomAnim(EnemySpritesAnimationInfo anim)
        {
            currentAnim = anim;
            currentFrame = 0;
            spriteRenderer.transform.localScale = originalScale * currentAnim.scale;
            spriteRenderer.sprite = currentAnim.frames[0];
            timer = 0f;
            return anim.frames.Length * anim.frameRate;
        }
        
        public float PlaySpawn()
        {
            currentAnim = spawnAnim;
            currentFrame = 0;
            spriteRenderer.transform.localScale = originalScale * currentAnim.scale;
            spriteRenderer.sprite = currentAnim.frames[0];
            timer = 0;
            spawningTimer = 0f;
            if (spawnEffect)
            {
                isSpawningEffect = true;
                spriteRenderer.enabled = false;
                spawnEffect.gameObject.SetActive(true);
                spawningTimer = spawnEffect.Play();
            }
            return spawningTimer + spawnAnim.frames.Length * spawnAnim.frameRate;
        }
        
        public void PlayIdle()
        {
            currentAnim = idleAnim;
            currentFrame = 0;
            spriteRenderer.transform.localScale = originalScale * currentAnim.scale;
            spriteRenderer.sprite = currentAnim.frames[0];
            timer = 0f;
        }
        
        public void PlayRun()
        {
            currentAnim = runAnim;
            currentFrame = 0;
            spriteRenderer.transform.localScale = originalScale * currentAnim.scale;
            spriteRenderer.sprite = currentAnim.frames[0];
            timer = 0f;
        }

        public float PlayAttack()
        {
            currentAnim = attackAnim;
            currentFrame = 0;
            spriteRenderer.transform.localScale = originalScale * currentAnim.scale;
            spriteRenderer.sprite = currentAnim.frames[0];
            timer = 0f;
            return attackAnim.frames.Length * attackAnim.frameRate;
        }
        
        public void PlayHit()
        {
            currentAnim = hitAnim;
            currentFrame = 0;
            spriteRenderer.transform.localScale = originalScale * currentAnim.scale;
            spriteRenderer.sprite = currentAnim.frames[0];
            timer = 0f;
        }

        public float PlayDie()
        {
            currentAnim = dieAnim;
            currentFrame = 0;
            spriteRenderer.transform.localScale = originalScale * currentAnim.scale;
            spriteRenderer.sprite = currentAnim.frames[0];
            timer = 0f;
            return dieAnim.frames.Length * dieAnim.frameRate;
        }

        private void Update()
        {
            if (isPause) return;
            if (isSpawningEffect)
            {
                if (spawningTimer > 0)
                {
                    spawningTimer -= Time.deltaTime;
                    return;
                }
                else
                {
                    isSpawningEffect = false;
                    spriteRenderer.enabled = true;
                    spawnEffect.gameObject.SetActive(false);
                }
            }
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

        public void Pause()
        {
            isPause = true;
        }

        public void Resume()
        {
            isPause = false;
        }
        
    }

    [Serializable]
    public class EnemySpritesAnimationInfo
    {
        public Sprite[] frames;
        public bool isLoop;
        public float frameRate = 0.1f;
        public float scale = 1f;
        public bool autoExit = true;
    }
}