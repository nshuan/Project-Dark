using UnityEngine;

namespace CustomAnimations
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpritesAnimation : MonoBehaviour
    {
        [SerializeField] private Sprite[] frames;
        [SerializeField] private float frameRate = 0.1f;

        private SpriteRenderer spriteRenderer;
        private int currentFrame;
        private float timer;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            timer += Time.deltaTime;

            if (timer >= frameRate)
            {
                currentFrame = (currentFrame + 1) % frames.Length;
                spriteRenderer.sprite = frames[currentFrame];
                timer -= frameRate; // subtract instead of reset to avoid drift
            }
        }
    }

}