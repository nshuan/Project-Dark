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
            damage = baseLogic.damage;
            stagger = baseLogic.stagger;
            explodeSize = baseLogic.explodeSize;
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
                cameraShake ??= new CameraShake() { Cam = VisualEffectHelper.Instance.DefaultCamera, Magnitude = 0.08f};
                VisualEffectHelper.Instance.PlayEffect(cameraShake);
                
                var count = Physics2D.CircleCastNonAlloc(toTower.GetBaseCenter(), explodeSize, Vector2.zero, hits,
                    0f,
                    enemyLayer);
                if (count > 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        var dir = hits[i].point - (Vector2)toTower.transform.position;
                        if (dir.magnitude > LevelUtilityV2.GetRelativeRange(explodeSize, dir))
                            continue;
                        
                        FlashHit(hits[i].transform, damage, toTower);
                    }
                }
            }).WaitForCompletion();

            dashDirection.x = toTower.GetBaseCenter().x - fromTower.GetBaseCenter().x;
            dashDirection.y = toTower.GetBaseCenter().y - fromTower.GetBaseCenter().y;
            var dashLine = MoveTowerHelper.Instance.GetTowerLine(fromTower, toTower, dashHitRadius);
            yield return dashLine.transform.DOScaleY(1.8f, 0.4f).SetEase(Ease.InQuad).WaitForCompletion();
            yield return new WaitForSeconds(0.1f);
            var count = Physics2D.CircleCastNonAlloc(fromTower.GetBaseCenter(), dashHitRadius, dashDirection, hits,
                dashDirection.magnitude);
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    DashHit(hits[i].transform, dashDamage, fromTower.GetBaseCenter());
                }
            }
            VisualEffectHelper.Instance.PlayEffect(cameraShake);
            yield return dashLine.transform.DOPunchScale(new Vector3(0f, 0.2f, 0f), 0.2f).WaitForCompletion();
            
            yield return new WaitForEndOfFrame();
            onComplete?.Invoke();
        }
        
        private void DashHit(Transform hitTransform, float value, Vector2 hitCenter)
        {
            if (hitTransform)
            {
                if (hitTransform.TryGetComponent(out hitTarget))
                {
                    hitTarget.HitDirectionX = dashDirection.x;
                    hitTarget.HitDirectionY = dashDirection.y;
                    hitTarget.Damage((int)value, hitCenter, dashStagger, DamageType.Normal);
                    PassiveEffectManager.Instance.TriggerEffect(PassiveTriggerType.DameByMoveSKill, hitTarget);
                }
            }
        }
    }
}