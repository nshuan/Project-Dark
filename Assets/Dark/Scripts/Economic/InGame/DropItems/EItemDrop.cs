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
        
        public WealthType kind;
        public int Quantity { get; set; }
        [NonSerialized] public Vector3 vfxPositionOffset;
        
        public void Drop(Vector2 position)
        {
            visual.gameObject.SetActive(true);
            var targetPos = position + Random.insideUnitCircle * 0.8f;
            shadow.SetParent(null);
            transform.DOJump(targetPos, 0.2f, RandomUtil.Range(1, 4), 0.5f).SetTarget(this)
                .OnComplete(() => shadow.SetParent(transform));
            shadow.DOMove((Vector3)targetPos - transform.position + shadow.position, 0.5f);
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