using System;
using System.Collections;
using UnityEngine;

namespace InGame
{
    [Serializable]
    public class MoveDashToTower : IMoveTowersLogic
    {
        [SerializeField] private AnimationCurve speedCurve;
        [SerializeField] private float duration;

        [Space] [Header("Combat")] 
        [SerializeField] private LayerMask enemyLayer;
        
        private int damage;
        private float stagger;
        private int maxHitEachTrigger = 5;
        private float hitRadius = 2f;
        
        private RaycastHit2D[] hits = new RaycastHit2D[10];
        private IDamageable hitTarget;
        private Vector2 direction;
        private PlayerCharacter characterRef;

        public void SetStats(int damage, float stagger, int maxHitEachTrigger, float size)
        {
            this.damage = damage;
            this.stagger = stagger;
            this.maxHitEachTrigger = maxHitEachTrigger;
            this.hitRadius = size;
        }

        public IEnumerator IEMove(PlayerCharacter character, Vector2 startPos, Vector2 endPos, Action onComplete)
        {
            hits ??= new RaycastHit2D[50];
            characterRef = character;
            direction = endPos - startPos;
            character.PlayDashEffect(endPos - startPos);
            
            var timeElapsed = 0f;
            while (timeElapsed / duration < 1f)
            {
                timeElapsed += Time.deltaTime;
                var speed = speedCurve.Evaluate(Mathf.Clamp01(timeElapsed / duration));
                character.transform.position = Vector2.Lerp(startPos, endPos, speed);
                
                var count = Physics2D.CircleCastNonAlloc(character.FlashExplodeCenter, hitRadius, Vector2.zero, hits,
                    0f,
                    enemyLayer);
                if (count > 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        DashHit(hits[i].transform, damage);
                    }
                }
                
                yield return null;
            }
                
            character.transform.position = endPos;
            character.StopDashEffect();
            yield return new WaitForEndOfFrame();
            onComplete?.Invoke();
        }
        
        private void DashHit(Transform hitTransform, float value)
        {
            if (hitTransform)
            {
                if (hitTransform.TryGetComponent(out hitTarget))
                {
                    hitTarget.HitDirectionX = direction.x;
                    hitTarget.HitDirectionY = direction.y;
                    hitTarget.Damage((int)value, characterRef.FlashExplodeCenter, stagger);
                }
            }
        }
    }
}