using System.Collections;
using InGame.Effects;
using UnityEngine;

namespace InGame.Trap
{
    public class LightningTrap : MonoTrap
    {
        private Coroutine trapCoroutine;
        
        public override void Setup(Camera cam, Vector3 position, Vector2 size, float damage, float duration)
        {
            if (trapCoroutine != null) StopCoroutine(trapCoroutine);
            trapCoroutine = StartCoroutine(IETrap(cam, position, size.x, damage, duration));
        }

        private IEnumerator IETrap(Camera cam, Vector3 position, float radius, float damage, float duration)
        {
            // Lightning burst effect
            var lightning = RadialLightningPool.Instance.Get(null);
            position.z = 0;
            lightning.transform.position = position;
            lightning.Init();
            lightning.length = radius;
            lightning.Execute(duration);
            
            var checkCount = duration * RefreshRate;
            var cd = 1 / RefreshRate;
            while (checkCount > 0)
            {
                var hits = Physics2D.CircleCastAll(position, radius, Vector2.zero, 0f, LayerMask.GetMask("Entity"));
                foreach (var hit in hits)
                {
                    if (hit.collider != null && hit.transform.TryGetComponent<EnemyEntity>(out var enemyEntity))
                    {
                        enemyEntity.OnHit(damage);
                    }
                }

                checkCount -= 1;
                yield return new WaitForSeconds(cd);
            }
            
            LightningTrapPool.Instance.Release(this);
        }
    }
}