using System;
using Dark.Scripts.AudioV2;
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
        private const float FlySpeed = 25f;
        
        [SerializeField] private TargetedProjectile targetLogic;
        [SerializeField] private GameObject vfxClaim;
        [SerializeField] private GameObject visual;
        [SerializeField] private AudioPlayComponentV2 sfx;
        [SerializeField] private Transform shadow;
        [SerializeField] private float minDistanceToTower = 1.5f;
        [SerializeField] private float dropSpanAngleToExcludeTower = 240f;
        [SerializeField] private float dropRange = 0.8f;
        
        public WealthType kind;
        public int Quantity { get; set; }
        public bool MarkedNotCollectedByManager { get; set; }
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
                    targetPos *= LevelUtility.GetRelativeRange(dropRange, targetPos - position) / dropRange;
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
        
        public void Drop(Vector2 position, Vector2 direction, float span, float scaleRange)
        {
            visual.gameObject.SetActive(true);
            
            var targetPos = position + RandomUtil.InsideUnitSpan(direction, span) * (dropRange * scaleRange);
            targetPos *= LevelUtility.GetRelativeRange(dropRange, targetPos - position) / dropRange;
            
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
            delayCollect = delay;
            collectCounter = delayCollect;
            Collect(target);
        }

        private bool isCollecting;
        private float delayCollect;
        private float collectStartDuration = 0.24f;
        private float collectCounter;
        private Vector3 targetPosition;
        public void Collect(Transform target)
        {
            targetPosition = target.position;
            targetLogic.InitializeProjectile(targetPosition, FlySpeed, 0.15f);
            targetLogic.InitializeAnimationCurve(ProjectileCurveManifest.GetRandomTrajectoryCurve(),
                ProjectileCurveManifest.GetAxisCorrectionCurve(0), ProjectileCurveManifest.GetProjectileSpeedCurve(1));
            
            isCollecting = true;
        }

        private Vector2 nextPosition;
        private void FixedUpdate()
        {
            if (!isCollecting) return;

            if (delayCollect > 0f)
            {
                delayCollect -= Time.deltaTime;
                return;
            }

            // if (collectCounter > 0f)
            // {
            //     visual.transform.position += new Vector3(0f, 0.5f * Time.deltaTime, 0f);
            //     collectCounter -= Time.deltaTime;
            //     if (collectCounter <= 0f)
            //         shadow.DOScale(0f, 0.2f);
            //     return;
            // }
            
            if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
            {
                visual.gameObject.SetActive(false);
                vfxClaim.transform.position = targetPosition + vfxPositionOffset;
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
            
            nextPosition = targetLogic.GetProjectileNextPosition(targetPosition);
            if ((targetPosition.x - transform.position.x) * (targetPosition.x - nextPosition.x) < 0f) 
                transform.position = targetPosition;
            else if ((targetPosition.y - transform.position.y) * (targetPosition.y - nextPosition.y) < 0f)
                transform.position = targetPosition;
            else
                transform.position = nextPosition;
        }
    }
}