using System;
using System.Collections;
using System.Collections.Generic;
using InGame.Effects;
using UnityEngine;

namespace InGame
{
    public class MoveDashFuseToTower : MoveDashToTower
    {
        private float aoeSize = 2f;
        private int damageAoe;
        private float aoeStagger;
        private int aoeMaxHitEachTrigger;
        
        private CameraShake cameraShake;

        public MoveDashFuseToTower(MoveDashToTower baseLogic)
        {
            damage = baseLogic.damage;
            stagger = baseLogic.stagger;
            maxHitEachTrigger = baseLogic.maxHitEachTrigger;
            hitRadius = baseLogic.hitRadius;
            speedCurve = baseLogic.speedCurve;
            duration = baseLogic.duration;
            enemyLayer = baseLogic.enemyLayer;
        }
        
        public override void SetStatsFuse(int damage, float stagger, int maxHitEachTrigger, float size)
        {
            damageAoe = damage;
            aoeStagger = stagger;
            aoeSize = size;
            aoeMaxHitEachTrigger = maxHitEachTrigger;
        }

        public override IEnumerator IEMove(PlayerCharacter character, TowerEntity fromTower, TowerEntity toTower, Action onComplete)
        {
            hits ??= new RaycastHit2D[50];
            hitHistory ??= new List<Transform>();
            hitHistory.Clear();
            currentHitHistoryIndex = 0;
            startPos.x = fromTower.transform.position.x + fromTower.GetTowerHeight().x;
            startPos.y = fromTower.transform.position.y + fromTower.GetTowerHeight().y;
            endPos.x = toTower.transform.position.x + toTower.GetTowerHeight().x;
            endPos.y = toTower.transform.position.y + toTower.GetTowerHeight().y;
            characterRef = character;
            direction = endPos - startPos;
            character.PlayDashEffect(endPos - startPos);

            var count = 0;
            var timeElapsed = 0f;
            while (timeElapsed / duration < 1f)
            {
                timeElapsed += Time.deltaTime;
                var speed = speedCurve.Evaluate(Mathf.Clamp01(timeElapsed / duration));
                
                var lastPos = character.transform.position;
                character.transform.position = Vector2.Lerp(startPos, endPos, speed);
                var lastPosOnGround = fromTower.GetBaseCenter() + (lastPos.x - startPos.x) / (endPos.x - startPos.x) *
                    (toTower.GetBaseCenter() - fromTower.GetBaseCenter());
                var currentPosOnGround = fromTower.GetBaseCenter() + (character.transform.position.x - startPos.x) /
                    (endPos.x - startPos.x) * (toTower.GetBaseCenter() - fromTower.GetBaseCenter());
                var movePath = currentPosOnGround - lastPosOnGround;
                count = Physics2D.CircleCastNonAlloc(
                    lastPosOnGround, 
                    hitRadius, movePath, hits,
                    movePath.magnitude,
                    enemyLayer);

                if (count > 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        DashHit(hits[i].transform, damage, lastPosOnGround);
                    }
                }
                
                yield return null;
            }
                
            character.transform.position = endPos;
            character.StopDashEffect();
            
            // Do aoe damage
            character.PlayAoe();
            cameraShake ??= new CameraShake() { Cam = VisualEffectHelper.Instance.DefaultCamera, Magnitude = 0.08f };
            VisualEffectHelper.Instance.PlayEffect(cameraShake);
                
            count = Physics2D.CircleCastNonAlloc(toTower.GetBaseCenter(), aoeSize, Vector2.zero, hits,
                0f,
                enemyLayer);
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    AoeHit(hits[i].transform, damageAoe, toTower);
                }
            }
            
            currentHitHistoryIndex = 0;

            yield return new WaitForEndOfFrame();
            onComplete?.Invoke();
        }
        
        private void AoeHit(Transform hitTransform, float value, TowerEntity targetTower)
        {
            if (hitTransform)
            {
                if (hitTransform.TryGetComponent(out hitTarget))
                {
                    if (hitHistory.Contains(hitTransform)) return;

                    if (currentHitHistoryIndex >= hitHistory.Count)
                        hitHistory.Add(hitTransform);
                    else
                        hitHistory[currentHitHistoryIndex] = hitTransform;

                    var aoeCenter = targetTower.GetBaseCenter();
                    hitTarget.HitDirectionX = hitTransform.position.x - aoeCenter.x;
                    hitTarget.HitDirectionY = hitTransform.position.y - aoeCenter.y;
                    hitTarget.Damage((int)value, aoeCenter, aoeStagger, DamageType.Normal);
                    PassiveEffectManager.Instance.TriggerEffect(PassiveTriggerType.DameByMoveSKill, hitTarget);
                }
            }
        }
    }
}