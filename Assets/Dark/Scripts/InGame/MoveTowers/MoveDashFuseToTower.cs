using System;
using System.Collections;
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
                character.transform.position = Vector2.Lerp(startPos, endPos, speed);
                
                count = Physics2D.CircleCastNonAlloc(character.FlashExplodeCenter, hitRadius, Vector2.zero, hits,
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
            
            // Do aoe damage
            character.PlayAoe();
            cameraShake ??= new CameraShake() { Cam = VisualEffectHelper.Instance.DefaultCamera };
            VisualEffectHelper.Instance.PlayEffect(cameraShake);
                
            count = Physics2D.CircleCastNonAlloc(character.FlashExplodeCenter, aoeSize, Vector2.zero, hits,
                0f,
                enemyLayer);
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    AoeHit(hits[i].transform, damageAoe);
                }
            }
            
            yield return new WaitForEndOfFrame();
            onComplete?.Invoke();
        }
        
        private void AoeHit(Transform hitTransform, float value)
        {
            if (hitTransform)
            {
                if (hitTransform.TryGetComponent(out hitTarget))
                {
                    hitTarget.HitDirectionX = hitTransform.position.x - characterRef.FlashExplodeCenter.x;
                    hitTarget.HitDirectionY = hitTransform.position.y - characterRef.FlashExplodeCenter.y;
                    hitTarget.Damage((int)value, characterRef.FlashExplodeCenter, aoeStagger);
                }
            }
        }
    }
}