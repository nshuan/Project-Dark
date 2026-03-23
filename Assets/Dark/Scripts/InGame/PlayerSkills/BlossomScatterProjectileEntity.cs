using System.Collections.Generic;
using UnityEngine;

namespace InGame
{
    public class BlossomScatterProjectileEntity : BlossomProjectileEntity
    {
        [Space] [Header("Blossom Scatter")] 
        [SerializeField] private float durationScatterMin = 0.85f;
        [SerializeField] private float durationScatterMax = 1.15f;
        [SerializeField] private AnimationCurve dropYCurve;
        
        private float scatterTimer;
        private Vector3 targetDropPosition;
        private float durationScatter;

        public override void Init(Vector2 rangeCenter, Vector2 direction, float range, float size, float speedScale, int damage,
            int criticalDamage, float criticalRate, float stagger, bool isCharge, int maxHit, List<IProjectileActivate> activateActions,
            List<IProjectileHit> hitActions, ProjectileType damageType)
        {
            base.Init(rangeCenter, direction, range, size, speedScale, damage, criticalDamage, criticalRate, stagger, isCharge, maxHit, activateActions, hitActions, damageType);

            scatterTimer = 0f;
            targetDropPosition = transform.position + (Vector3)direction * RandomUtil.Range(Range - 0.3f, Range + 0.1f);
            collider.CanTrigger = false;
            MaxHit = 1;
            durationScatter = RandomUtil.Range(durationScatterMin, durationScatterMax);
        }

        protected override void FixedUpdate()
        {
            if (!activated) return;

            scatterTimer += Time.fixedDeltaTime;
                        
            var t = Mathf.Clamp01(scatterTimer / durationScatter);

            // horizontal position (isometric: usually XZ plane)
            var horizontalPos = SpawnPosition + (targetDropPosition - SpawnPosition) * t;

            // height offset using curve
            var curveY = dropYCurve.Evaluate(t) * 3f;
                            
            // final position
            horizontalPos.y += curveY;
            horizontalPos.z = 0;

            transform.position = horizontalPos;
            
            if (scatterTimer >= durationScatter)
                ProjectileHit(null);
        }
    }
}