using System;
using System.Collections;
using DG.Tweening;
using InGame.Effects;
using UnityEngine;

namespace InGame
{
    public class MoveFlashFuseToTower : MoveFlashToTower
    {
        private float dashHitRadius = 2f;
        private int dashDamage;
        private float dashStagger;
        private int dashMaxHitEachTrigger;
        private Vector2 dashDirection;
        
        public MoveFlashFuseToTower(MoveFlashToTower baseLogic)
        {
            enemyLayer = baseLogic.enemyLayer;
        }

        public override void SetStatsFuse(int damage, float stagger, int maxHitEachTrigger, float size)
        {
            dashDamage = damage;
            dashStagger = stagger;
            dashMaxHitEachTrigger = maxHitEachTrigger;
            dashHitRadius = size;
        }

        public override IEnumerator IEMove(PlayerCharacter character, TowerEntity fromTower, TowerEntity toTower, Action onComplete)
        {
            hits ??= new RaycastHit2D[50];
            characterRef = character;
                
            yield return character.PLayFlashEffect().WaitForCompletion(); 
            
            character.transform.position = toTower.transform.position + toTower.GetTowerHeight();
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

            dashDirection.x = toTower.GetBaseCenter().x - fromTower.GetBaseCenter().x;
            dashDirection.y = toTower.GetBaseCenter().y - fromTower.GetBaseCenter().y;
            var dashLine = MoveTowerHelper.Instance.GetTowerLine(fromTower.Id, toTower.Id);
            yield return dashLine.transform.DOScaleY(1.8f, 0.4f).SetEase(Ease.InQuad).WaitForCompletion();
            yield return new WaitForSeconds(0.1f);
            dashLine.color = new Color(1f, 1f, 1f, 0.7f);
            var count = Physics2D.CircleCastNonAlloc(fromTower.GetBaseCenter(), dashHitRadius, dashDirection, hits,
                dashDirection.magnitude);
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    DashHit(hits[i].transform, dashDamage);
                }
            }
            VisualEffectHelper.Instance.PlayEffect(cameraShake);
            yield return dashLine.transform.DOPunchScale(new Vector3(0f, 0.2f, 0f), 0.2f).WaitForCompletion();

            dashLine.DOFade(0f, 0.2f);
            
            yield return new WaitForEndOfFrame();
            onComplete?.Invoke();
        }
        
        private void DashHit(Transform hitTransform, float value)
        {
            if (hitTransform)
            {
                if (hitTransform.TryGetComponent(out hitTarget))
                {
                    hitTarget.HitDirectionX = dashDirection.x;
                    hitTarget.HitDirectionY = dashDirection.y;
                    hitTarget.Damage((int)value, characterRef.FlashExplodeCenter, dashStagger);
                }
            }
        }
    }
}