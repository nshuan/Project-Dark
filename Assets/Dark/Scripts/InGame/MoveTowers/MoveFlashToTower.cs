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
        [SerializeField] private float explodeSize = 2f;
        [SerializeField] private LayerMask enemyLayer;
        [SerializeField] private int damage;
        [SerializeField] private float stagger;
        
        private RaycastHit2D[] hits = new RaycastHit2D[50];
        private IDamageable hitTarget;
        private PlayerCharacter characterRef;
        private CameraShake cameraShake;
        
        public IEnumerator IEMove(PlayerCharacter character, Vector2 startPos, Vector2 endPos, Action onComplete)
        {
            hits ??= new RaycastHit2D[50];
            characterRef = character;
            
            yield return character.PLayFlashEffect().WaitForCompletion(); 
            
            character.transform.position = endPos;
            yield return new WaitForEndOfFrame();
            
            yield return character.StopFlashEffect(() =>
            {
                cameraShake ??= new CameraShake() { Cam = VisualEffectHelper.Instance.DefaultCamera };
                VisualEffectHelper.Instance.PlayEffect(cameraShake);
                
                var count = Physics2D.CircleCastNonAlloc(character.FlashExplodeCenter, explodeSize, Vector2.zero, hits,
                    0f,
                    enemyLayer);
                if (count > 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        FlashHit(hits[i].transform, damage);
                    }
                }
            }).WaitForCompletion();
            
            yield return new WaitForEndOfFrame();
            onComplete?.Invoke();
        }
        
        private void FlashHit(Transform hitTransform, float value)
        {
            if (hitTransform)
            {
                if (hitTransform.TryGetComponent(out hitTarget))
                {
                    hitTarget.HitDirectionX = hitTransform.position.x - characterRef.FlashExplodeCenter.x;
                    hitTarget.HitDirectionY = hitTransform.position.y - characterRef.FlashExplodeCenter.y;
                    hitTarget.Damage((int)value, characterRef.FlashExplodeCenter, stagger);
                }
            }
        }
    }
}