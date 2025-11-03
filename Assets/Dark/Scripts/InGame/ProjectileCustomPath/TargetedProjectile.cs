using System;
using UnityEngine;

namespace InGame.ProjectileCustomPath
{
    public class TargetedProjectile : MonoBehaviour
    {
        // private Transform target;
        private float moveSpeed;
        private float maxMoveSpeed;
        private float trajectoryMaxRelativeHeight;
        private float distanceToTargetToDestroyProjectile = 1f;

        private AnimationCurve trajectoryAnimationCurve;
        private AnimationCurve axisCorrectionAnimationCurve;
        private AnimationCurve projectileSpeedAnimationCurve;

        private Vector3 trajectoryStartPoint;
        private Vector3 projectileMoveDir;
        private Vector3 trajectoryRange;

        private Vector3 nextPosition = new Vector2();
        private Vector2 nextPositionNormalized = new Vector2();
        private Vector2 nextPositionCorrection = new Vector2();
        private Vector2 nextTrajectoryPosition = new Vector2();
        
        private void Start()
        {
            trajectoryStartPoint = transform.position;
        }

        // private void Update()
        // {
        //     transform.position = GetProjectileNextPosition();
        //     transform.rotation = GetProjectileNextRotation();
        // }

        public Vector3 GetProjectileNextPosition(Vector3 targetPosition)
        {
            trajectoryRange = targetPosition - trajectoryStartPoint;

            if (Mathf.Abs(trajectoryRange.normalized.x) < Mathf.Abs(trajectoryRange.normalized.y))
            {
                // Projectile will be curved on the X axis    
                if (trajectoryRange.y < 0)
                    moveSpeed = -moveSpeed;
                return GetPositionWithXCurve();
            }
            else
            {
                // Projectile will be curved on the Y axis
                if (trajectoryRange.x < 0)
                    moveSpeed = -moveSpeed;
                return GetPositionWithYCurve();
            }
        }

        public Quaternion GetProjectileNextRotation()
        {
            return Quaternion.Euler(0f, 0f, Mathf.Atan2(projectileMoveDir.y, projectileMoveDir.x) * Mathf.Rad2Deg);
        }

        private Vector3 GetPositionWithYCurve()
        {
            nextPosition.x = transform.position.x + moveSpeed * Time.deltaTime;
            nextPositionNormalized.x = (nextPosition.x - trajectoryStartPoint.x) / trajectoryRange.x;

            nextPositionNormalized.y = trajectoryAnimationCurve.Evaluate(nextPositionNormalized.x);
            
            nextPositionCorrection.y = axisCorrectionAnimationCurve.Evaluate(nextPositionNormalized.x) * trajectoryRange.y; 
            nextPosition.y = trajectoryStartPoint.y + nextPositionNormalized.y * trajectoryMaxRelativeHeight +
                             nextPositionCorrection.y;
            
            CalculateNextProjectileSpeed(nextPositionNormalized.x);
            projectileMoveDir = nextPosition - transform.position;

            return nextPosition;
        }
        
        private Vector3 GetPositionWithXCurve()
        {
            nextPosition.y = transform.position.y + moveSpeed * Time.deltaTime;
            nextPositionNormalized.y = (nextPosition.y - trajectoryStartPoint.y) / trajectoryRange.y;

            nextPositionNormalized.x = trajectoryAnimationCurve.Evaluate(nextPositionNormalized.y);
            nextTrajectoryPosition.x = nextPositionNormalized.x * trajectoryMaxRelativeHeight;
            
            nextPositionCorrection.x = axisCorrectionAnimationCurve.Evaluate(nextPositionNormalized.y) * trajectoryRange.x;

            if (trajectoryRange is { x: > 0, y: > 0 })
            {
                nextTrajectoryPosition.x = -nextTrajectoryPosition.x;
            }

            if (trajectoryRange is { x: < 0, y: < 0 })
            {
                nextTrajectoryPosition.x = -nextTrajectoryPosition.x;
            }
            
            nextPosition.x = trajectoryStartPoint.x + nextTrajectoryPosition.x + nextPositionCorrection.x;

            CalculateNextProjectileSpeed(nextPositionNormalized.y);
            projectileMoveDir = nextPosition - transform.position;
            
            return nextPosition;
        }
        
        private void CalculateNextProjectileSpeed(float nextPositionXNormalized)
        {
            // moveSpeed = nextMoveSpeedNormalized * maxMoveSpeed;
            moveSpeed = projectileSpeedAnimationCurve.Evaluate(nextPositionXNormalized) * maxMoveSpeed;
        }

        public void InitializeProjectile(Vector3 targetPosition, float maxMoveSpeed, float trajectoryMaxHeight)
        {
            this.maxMoveSpeed = maxMoveSpeed;
            
            var xDistanceToTarget = targetPosition.x - transform.position.x;
            this.trajectoryMaxRelativeHeight = Mathf.Abs(xDistanceToTarget) * trajectoryMaxHeight;
        }

        public void InitializeAnimationCurve(AnimationCurve trajectorynimationCurve, AnimationCurve axisCorrectionAnimationCurve, AnimationCurve projectileSpeedAnimationCurve)
        {
            this.trajectoryAnimationCurve = trajectorynimationCurve;
            this.axisCorrectionAnimationCurve = axisCorrectionAnimationCurve;
            this.projectileSpeedAnimationCurve = projectileSpeedAnimationCurve;
        }
    }
}