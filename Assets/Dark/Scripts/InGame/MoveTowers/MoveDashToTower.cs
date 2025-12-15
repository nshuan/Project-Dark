using System;
using System.Collections;
using System.Collections.Generic;
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
        
        protected List<Transform> hitHistory = new List<Transform>();
        protected int currentHitHistoryIndex;
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
            hitHistory ??= new List<Transform>();
            currentHitHistoryIndex = 0;
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
                var positionRatio = (character.transform.position.x - startPos.x) / (endPos.x - startPos.x);
                // var count = Physics2D.CircleCastNonAlloc(character.FlashExplodeCenter, hitRadius, Vector2.zero, hits,
                //     0f,
                //     enemyLayer);
                var count = Physics2D.CircleCastNonAlloc(
                    fromTower.GetBaseCenter() + positionRatio * (toTower.GetBaseCenter() - fromTower.GetBaseCenter()), 
                    hitRadius, Vector2.zero, hits,
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

            currentHitHistoryIndex = 0;
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
                    if (hitHistory.Contains(hitTransform)) return;

                    if (currentHitHistoryIndex >= hitHistory.Count)
                    {
                        hitHistory.Add(hitTransform);
                        currentHitHistoryIndex += 1;
                    }
                    else
                    {
                        hitHistory[currentHitHistoryIndex] = hitTransform;
                        currentHitHistoryIndex += 1;
                    }
                    hitTarget.HitDirectionX = direction.x;
                    hitTarget.HitDirectionY = direction.y;
                    hitTarget.Damage((int)value, characterRef.FlashExplodeCenter, stagger, DamageType.Normal);
                }
            }
        }
    }
}