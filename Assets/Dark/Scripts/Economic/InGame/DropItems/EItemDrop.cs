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
        private const float FlySpeed = 15f;

        [SerializeField] private TargetedProjectile targetLogic;
        [SerializeField] private GameObject vfxClaim;
        [SerializeField] private GameObject visual;
        [SerializeField] private AudioComponent sfx;
        
        public WealthType kind;
        public int Quantity { get; set; }
        [NonSerialized] public Vector3 vfxPositionOffset;

        private void OnLevelCompleted()
        {
            ResetAction();
            Collect(EItemDropManager.Instance.CollectTarget, 0f);
        }

        private void OnResourceCollectorDamaged(EItemDropCollector target)
        {
            ResetAction();
            Collect(target, 0f);
        }
        
        public void Drop(Vector2 position)
        {
            visual.gameObject.SetActive(true);
            var targetPos = position + Random.insideUnitCircle.normalized * 0.6f;
            transform.DOJump(targetPos, 0.2f, 1, 0.5f).SetTarget(this);

            CombatActions.OnResourceCollectorDamaged += OnResourceCollectorDamaged;
            LevelManager.Instance.OnWin += OnLevelCompleted;
            LevelManager.Instance.OnLose += OnLevelCompleted;
        }

        private void ResetAction()
        {
            CombatActions.OnResourceCollectorDamaged -= OnResourceCollectorDamaged;
            LevelManager.Instance.OnWin -= OnLevelCompleted;
            LevelManager.Instance.OnLose -= OnLevelCompleted;
        }
        
        public void Collect(EItemDropCollector target, float delay)
        {
            Collect(target);
        }

        private bool isCollecting;
        private EItemDropCollector target;
        public void Collect(EItemDropCollector target)
        {
            this.target = target;
            targetLogic.InitializeProjectile(target.transform.position, FlySpeed, 0.15f);
            targetLogic.InitializeAnimationCurve(ProjectileCurveManifest.GetRandomTrajectoryCurve(),
                ProjectileCurveManifest.GetAxisCorrectionCurve(0), ProjectileCurveManifest.GetProjectileSpeedCurve(0));

            target.RegisterItem(this);
            isCollecting = true;
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
                    target.CollectItem();
                    ResetAction();
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