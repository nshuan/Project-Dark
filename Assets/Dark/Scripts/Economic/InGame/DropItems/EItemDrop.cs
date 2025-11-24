using System;
using Dark.Scripts.Audio;
using Dark.Scripts.Utils;
using DG.Tweening;
using InGame;
using InGame.ProjectileCustomPath;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Economic.InGame.DropItems
{
    public class EItemDrop : MonoBehaviour, ICollectible
    {
        private const float FlySpeed = 22f;
        
        [SerializeField] private TargetedProjectile targetLogic;
        [SerializeField] private GameObject vfxClaim;
        [SerializeField] private GameObject visual;
        [SerializeField] private AudioComponent sfx;
        [SerializeField] private Transform shadow;
        [SerializeField] private float minDistanceToTower = 1.5f;
        [SerializeField] private float dropSpanAngleToExcludeTower = 240f;
        [SerializeField] private float dropRange = 0.8f;
        
        public WealthType kind;
        public int Quantity { get; set; }
        [NonSerialized] public Vector3 vfxPositionOffset;
        
        public void Drop(Vector2 position)
        {
            visual.gameObject.SetActive(true);

            var calculatedTargetPosition = false;
            var targetPos = position;
            foreach (var tower in LevelManager.Instance.Towers)
            {
                if (Vector2.Distance(tower.transform.position, position) < minDistanceToTower + dropRange)
                {
                    targetPos = position + RandomUtil.InsideUnitSpan(position - (Vector2)tower.transform.position, dropSpanAngleToExcludeTower) * dropRange;
                    calculatedTargetPosition = true;
                    break;
                }
            }
            if (!calculatedTargetPosition)
                targetPos = position + Random.insideUnitCircle * dropRange;
            
            var dropJumps = RandomUtil.Range(1, 4);
            var dropDuration = Mathf.Max(dropJumps * 0.2f, 0.36f);
            shadow.SetParent(null);
            shadow.localScale = Vector3.one;
            transform.DOJump(targetPos, 0.16f, dropJumps, dropDuration).SetTarget(this)
                .OnComplete(() => shadow.SetParent(transform));
            shadow.DOMove((Vector3)targetPos - transform.position + shadow.position, dropDuration);
        }
        
        public void Drop(Vector2 position, Vector2 direction, float span)
        {
            visual.gameObject.SetActive(true);
            
            var targetPos = position + RandomUtil.InsideUnitSpan(direction, span) * dropRange;
            
            var dropJumps = RandomUtil.Range(1, 4);
            var dropDuration = Mathf.Max(dropJumps * 0.2f, 0.36f);
            shadow.SetParent(null);
            shadow.localScale = Vector3.one;
            transform.DOJump(targetPos, 0.16f, dropJumps, dropDuration).SetTarget(this)
                .OnComplete(() => shadow.SetParent(transform));
            shadow.DOMove((Vector3)targetPos - transform.position + shadow.position, dropDuration);
        }
        
        public void Collect(Transform target, float delay)
        {
            Collect(target);
        }

        private bool isCollecting;
        private Transform target;
        public void Collect(Transform target)
        {
            this.target = target;
            targetLogic.InitializeProjectile(target.transform.position, FlySpeed, 0.15f);
            targetLogic.InitializeAnimationCurve(ProjectileCurveManifest.GetRandomTrajectoryCurve(),
                ProjectileCurveManifest.GetAxisCorrectionCurve(0), ProjectileCurveManifest.GetProjectileSpeedCurve(1));
            
            isCollecting = true;

            shadow.DOScale(0f, 0.2f);
        }

        private Vector2 nextPosition;
        private void Update()
        {
            if (!isCollecting) return;

            if (Vector2.Distance(transform.position, target.transform.position) < 0.1f)
            {
                visual.gameObject.SetActive(false);
                vfxClaim.transform.position = target.transform.position + vfxPositionOffset;
                vfxClaim.SetActive(true);
                sfx.Play();
                isCollecting = false;
                this.DelayCall(1f, () =>
                {
                    vfxClaim.SetActive(false);
                    EItemDropPool.Instance.Release(this);
                });
                return;
            }
            
            nextPosition = targetLogic.GetProjectileNextPosition(target.transform.position);
            if ((target.transform.position.x - transform.position.x) * (target.transform.position.x - nextPosition.x) < 0f) 
                transform.position = target.transform.position;
            else if ((target.transform.position.y - transform.position.y) * (target.transform.position.y - nextPosition.y) < 0f)
                transform.position = target.transform.position;
            else
                transform.position = nextPosition;
        }
    }
}