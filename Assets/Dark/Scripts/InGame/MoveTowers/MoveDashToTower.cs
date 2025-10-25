using System;
using System.Collections;
using UnityEngine;

namespace InGame
{
    [Serializable]
    public class MoveDashToTower : IMoveTowersLogic
    {
        public AnimationCurve speedCurve;
        public float duration;

        [Space] [Header("Combat")] 
        public LayerMask enemyLayer;
        
        public int damage;
        public float stagger;
        public int maxHitEachTrigger = 5;
        public float hitRadius = 2f;
        
        protected RaycastHit2D[] hits = new RaycastHit2D[10];
        protected IDamageable hitTarget;
        protected Vector2 direction;
        protected PlayerCharacter characterRef;

        protected Vector2 startPos;
        protected Vector2 endPos;
        
        public void SetStats(int damage, float stagger, int maxHitEachTrigger, float size)
        {
            this.damage = damage;
            this.stagger = stagger;
            this.maxHitEachTrigger = maxHitEachTrigger;
            this.hitRadius = size;
        }

        public virtual void SetStatsFuse(int damage, float stagger, int maxHitEachTrigger, float size)
        {
            
        }

        public virtual IEnumerator IEMove(PlayerCharacter character, TowerEntity fromTower, TowerEntity toTower, Action onComplete)
        {
            hits ??= new RaycastHit2D[50];
            startPos.x = fromTower.transform.position.x + fromTower.GetTowerHeight().x;
            startPos.y = fromTower.transform.position.y + fromTower.GetTowerHeight().y;
            endPos.x = toTower.transform.position.x + toTower.GetTowerHeight().x;
            endPos.y = toTower.transform.position.y + toTower.GetTowerHeight().y;
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
        
        protected void DashHit(Transform hitTransform, float value)
        {
            if (hitTransform)
            {
                if (hitTransform.TryGetComponent(out hitTarget))
                {
                    hitTarget.HitDirectionX = direction.x;
                    hitTarget.HitDirectionY = direction.y;
                    hitTarget.Damage((int)value, characterRef.FlashExplodeCenter, stagger, DamageType.Normal);
                }
            }
        }
    }
}