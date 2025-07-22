using System;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace InGame
{
    public class PlayerSpineAnimation : MonoBehaviour
    {
        [SerializeField] private SkeletonAnimation skeletonAnimation;
        [SpineAnimation(dataField = "skeletonAnimation")] public string animIdle;
        [SpineAnimation(dataField = "skeletonAnimation")] public string animAttackNormal;
        [SpineAnimation(dataField = "skeletonAnimation")] public string animAttackCharge;
        public string[] directionSkins;

        private (float, float) animAttackDuration;
        private float rotationThresholdGap;
        private int currentSkinIndex;
        
        private void Start()
        {
            animAttackDuration = (0.4f, skeletonAnimation.skeleton.Data.FindAnimation(animAttackNormal).Duration);
            rotationThresholdGap = 360f / directionSkins.Length;
            currentSkinIndex = -1;
        }

        public void PlayIdle()
        {
            skeletonAnimation.AnimationState.SetAnimation(0, animIdle, true);
        }

        public (float, float) PlayAttack()
        {
            skeletonAnimation.AnimationState.SetAnimation(0, animAttackNormal, false);
            skeletonAnimation.AnimationState.AddAnimation(0, animIdle, true, 0f);
            return animAttackDuration;
        }
        
        public void PlaySpecialAttack()
        {
            skeletonAnimation.AnimationState.SetAnimation(0, animAttackCharge, false);
            skeletonAnimation.AnimationState.AddAnimation(0, animIdle, true, 0f);
        }
        
        public void UpdateRotation(Vector2 direction)
        {
            var mag = direction.magnitude;
            if (mag > 1e-5f) {
                var angle = Mathf.Acos(Mathf.Clamp(direction.y / mag, -1f, 1f)) * Mathf.Rad2Deg;
                angle = direction.x < 0 ? 360f - angle : angle;
                var newIndex = (int)(angle / rotationThresholdGap + 1) % directionSkins.Length;
                if (newIndex != currentSkinIndex)
                {
                    var skeleton = skeletonAnimation.Skeleton;
                    var skin = skeleton.Data.FindSkin(directionSkins[newIndex]);
                    skeleton.SetSkin(skin);
                    skeleton.SetSlotsToSetupPose();
                    skeletonAnimation.AnimationState.Apply(skeleton);
                    currentSkinIndex = newIndex;
                }
            }
        }
    }
}