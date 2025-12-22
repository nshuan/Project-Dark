using System;
using System.Collections;
using UnityEngine;

namespace InGame.EnemyEffect
{
    public class EnemyHitFlash : MonoEnemyHitEffect
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Material matHit;
        [SerializeField] private float delayBackToNormal = 0.1f;

        private Coroutine coroutineHit;
        private Material matNormal;

        private void Awake()
        {
            matNormal = spriteRenderer.sharedMaterial;
        }

        public override void OnHit()
        {
            if (coroutineHit != null) StopCoroutine(coroutineHit);
            coroutineHit = StartCoroutine(IEHit());
        }

        private IEnumerator IEHit()
        {
            spriteRenderer.sharedMaterial = matHit;
            yield return new WaitForSeconds(delayBackToNormal);
            spriteRenderer.sharedMaterial = matNormal;
        }
    }
}