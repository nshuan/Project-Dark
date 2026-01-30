using System;
using System.Collections;
using UnityEngine;

namespace InGame.EnemyEffect
{
    public class EnemyHitChangeColor : MonoEnemyHitEffect
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Color colorHit;
        [SerializeField] private float delayBackToNormal = 0.1f;

        private Coroutine coroutineHit;
        private Color originalColor;

        private void Awake()
        {
            originalColor = spriteRenderer.color;
        }

        public override void OnHit()
        {
            if (coroutineHit != null) StopCoroutine(coroutineHit);
            coroutineHit = StartCoroutine(IEHit());
        }

        private IEnumerator IEHit()
        {
            spriteRenderer.color = colorHit;
            yield return new WaitForSeconds(delayBackToNormal);
            spriteRenderer.color = originalColor;
        }
    }
}