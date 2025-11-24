using System;
using UnityEngine;

namespace CustomAnimations
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpritesAnimation : MonoBehaviour
    {
        [SerializeField] private Sprite[] frames;
        [SerializeField] private float frameRate = 0.1f;
        [SerializeField] private int loop = -1; // -1 = loop forever
        [SerializeField] private bool autoPlay = true;

        private SpriteRenderer spriteRenderer;
        private int currentFrame;
        private float timer;
        private int loopCount;
        private bool isPlaying;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void OnEnable()
        {
            loopCount = loop;
            if (autoPlay) isPlaying = true;
        }

        private void OnDisable()
        {
            currentFrame = 0;
            timer = 0f;
            spriteRenderer.sprite = frames[0];
        }

        private void Update()
        {
            if (!isPlaying) return;
            if (loopCount == 0) return;
            
            timer += Time.deltaTime;

            if (timer >= frameRate)
            {
                if (currentFrame + 1 >= frames.Length)
                {
                    loopCount -= 1;
                    if (loopCount == 0) return;
                    currentFrame = 0;
                }
                else
                    currentFrame += 1;
                
                spriteRenderer.sprite = frames[currentFrame];
                timer -= frameRate; // subtract instead of reset to avoid drift
            }
        }

        public float Play()
        {
            isPlaying = true;
            return frameRate * frames.Length;
        }
    }

}