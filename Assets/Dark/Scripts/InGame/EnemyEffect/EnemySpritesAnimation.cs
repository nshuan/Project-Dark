using System;
using System.Collections;
using System.Collections.Generic;
using CustomAnimations;
using Sirenix.OdinInspector;
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
            UpdateFollowerPosition();
            timer = 0f;
            return anim.frames.Length * anim.frameRate;
        }

        public float GetCustomAnimDuration(EnemySpritesAnimationInfo anim) => anim.frames.Length * anim.frameRate;
        
        public float PlaySpawn()
        {
            currentAnim = spawnAnim;
            currentFrame = 0;
            spriteRenderer.transform.localScale = originalScale * currentAnim.scale;
            spriteRenderer.sprite = currentAnim.frames[0];
            UpdateFollowerPosition();
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
        
        public float GetSpawnDuration() => spawnAnim.frames.Length * spawnAnim.frameRate;
        
        public void PlayIdle()
        {
            currentAnim = idleAnim;
            currentFrame = 0;
            spriteRenderer.transform.localScale = originalScale * currentAnim.scale;
            spriteRenderer.sprite = currentAnim.frames[0];
            UpdateFollowerPosition();
            timer = 0f;
        }
        
        public void PlayRun()
        {
            currentAnim = runAnim;
            currentFrame = 0;
            spriteRenderer.transform.localScale = originalScale * currentAnim.scale;
            spriteRenderer.sprite = currentAnim.frames[0];
            UpdateFollowerPosition();
            timer = 0f;
        }

        public float PlayAttack()
        {
            currentAnim = attackAnim;
            currentFrame = 0;
            spriteRenderer.transform.localScale = originalScale * currentAnim.scale;
            spriteRenderer.sprite = currentAnim.frames[0];
            UpdateFollowerPosition();
            timer = 0f;
            return attackAnim.frames.Length * attackAnim.frameRate;
        }
        
        public float GetAttackDuration() => attackAnim.frames.Length * attackAnim.frameRate;
        
        public void PlayHit()
        {
            currentAnim = hitAnim;
            currentFrame = 0;
            spriteRenderer.transform.localScale = originalScale * currentAnim.scale;
            spriteRenderer.sprite = currentAnim.frames[0];
            UpdateFollowerPosition();
            timer = 0f;
        }

        public float PlayDie()
        {
            currentAnim = dieAnim;
            currentFrame = 0;
            spriteRenderer.transform.localScale = originalScale * currentAnim.scale;
            spriteRenderer.sprite = currentAnim.frames[0];
            UpdateFollowerPosition();
            timer = 0f;
            return dieAnim.frames.Length * dieAnim.frameRate;
        }
        
        public float GetDieDuration() => dieAnim.frames.Length * dieAnim.frameRate;

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
                UpdateFollowerPosition();
                timer -= currentAnim.frameRate; // subtract instead of reset to avoid drift
            }
        }

        private void UpdateFollowerPosition()
        {
            // Update position of follow objects
            if (currentAnim.followers is { Count: > 0 }
                && currentAnim.followers[0].localPositions is { Count: > 0 })
            {
                foreach (var follower in currentAnim.followers)
                {
                    if (currentFrame < follower.localPositions.Count)
                        follower.follower.localPosition = follower.localPositions[currentFrame];
                    else follower.follower.localPosition = follower.localPositions[^1];
                }
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

        [Button]
        public void AddFollowerPosition()
        {
            if (followers == null) return;
            foreach (var follower in followers)
            {
                follower.localPositions ??= new List<Vector2>();
                follower.localPositions.Add(new Vector2(follower.follower.localPosition.x, follower.follower.localPosition.y));
            }
        }

        [Button]
        public void Inverse()
        {
            if (followers == null) return;
            for (var i = 0; i < followers.Count; i++)
            {
                var temp = new List<Vector2>();
                for (var positionIndex = followers[i].localPositions.Count - 1; positionIndex >= 0; positionIndex--)
                {
                    temp.Add(followers[i].localPositions[positionIndex]);
                }
                followers[i].localPositions = temp;
            }
        }
        
        [Space]
        [Header("Follow objects")]
        [TableList]
        public List<EnemySpritesObjectFollow> followers;
    }

    [Serializable]
    public class EnemySpritesObjectFollow
    {
        public Transform follower;
        public List<Vector2> localPositions;
    }
}