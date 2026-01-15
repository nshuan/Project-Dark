using System;
using System.Collections;
using DG.Tweening;
using InGame.Effects;
using UnityEngine;

namespace InGame
{
    [Serializable]
    public class MoveFlashToTower : IMoveTowersLogic
    {
        [Space] [Header("Combat")] 
        public LayerMask enemyLayer;
        
        public float explodeSize = 2f;
        public int damage;
        public float stagger;
        
        protected RaycastHit2D[] hits = new RaycastHit2D[50];
        protected IDamageable hitTarget;
        protected PlayerCharacter characterRef;
        protected CameraShake cameraShake;

        public void SetStats(int damage, float stagger, int maxHitEachTrigger, float size)
        {
            this.damage = damage;
            this.stagger = stagger;
            this.explodeSize = size;
        }

        public virtual void SetStatsFuse(int damage, float stagger, int maxHitEachTrigger, float size)
        {
            
        }

        public virtual IEnumerator IEMove(PlayerCharacter character, TowerEntity fromTower, TowerEntity toTower, Action onComplete)
        {
            hits ??= new RaycastHit2D[50];
            characterRef = character;
                
            yield return character.PLayFlashEffect().WaitForCompletion(); 
            
            character.transform.position = toTower.transform.position + toTower.GetTowerHeight();
            yield return new WaitForEndOfFrame();
            
            yield return character.StopFlashEffect(() =>
            {
                cameraShake ??= new CameraShake() { Cam = VisualEffectHelper.Instance.DefaultCamera, Magnitude = 0.05f};
                VisualEffectHelper.Instance.PlayEffect(cameraShake);
                
                var count = Physics2D.CircleCastNonAlloc(toTower.GetBaseCenter(), explodeSize, Vector2.zero, hits,
                    0f,
                    enemyLayer);
                if (count > 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        FlashHit(hits[i].transform, damage, toTower);
                    }
                }
            }).WaitForCompletion();
            
            yield return new WaitForEndOfFrame();
            onComplete?.Invoke();
        }
        
        protected void FlashHit(Transform hitTransform, float value, TowerEntity hitTower)
        {
            if (hitTransform)
            {
                if (hitTransform.TryGetComponent(out hitTarget))
                {
                    if (Vector2.Distance(hitTransform.position, hitTower.transform.position) >
                        LevelUtilityV2.GetRelativeRange(explodeSize,
                            hitTransform.position - hitTransform.transform.position))
                    {
                        return;
                    }
                    
                    var aoeCenter = hitTower.GetBaseCenter();
                    hitTarget.HitDirectionX = hitTransform.position.x - aoeCenter.x;
                    hitTarget.HitDirectionY = hitTransform.position.y - aoeCenter.y;
                    hitTarget.Damage((int)value, aoeCenter, stagger, DamageType.Normal);
                }
            }
        }
    }
}