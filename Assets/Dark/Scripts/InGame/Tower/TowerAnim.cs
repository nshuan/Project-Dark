using Dark.Scripts.Utils.Skeleton;
using Spine.Unity;
using Spine.Unity.Editor;
using UnityEngine;
using AnimationState = Spine.AnimationState;

namespace InGame
{
    public class TowerAnim : MonoBehaviour
    {
        public SkeletonAnimation skeleton;
        
        [SpineAnimationName(nameof(skeleton))]
        [SerializeField] private string animIdle100;
        
        [SpineAnimationName(nameof(skeleton))]
        [SerializeField] private string animIdle70;
        
        [SpineAnimationName(nameof(skeleton))]
        [SerializeField] private string animIdle30;
        
        [SpineAnimationName(nameof(skeleton))]
        [SerializeField] private string animIdle0;

        [SpineAnimationName(nameof(skeleton))] 
        [SerializeField] private string animBreakTo70;
        
        [SpineAnimationName(nameof(skeleton))] 
        [SerializeField] private string animBreakTo30;
        
        [SpineAnimationName(nameof(skeleton))] 
        [SerializeField] private string animBreakTo0;
        
        [SpineAnimationName(nameof(skeleton))] 
        [SerializeField] private string animHealTo100;
        
        [SpineAnimationName(nameof(skeleton))]
        [SerializeField] private string animHealTo70;
        
        [SpineAnimationName(nameof(skeleton))]
        [SerializeField] private string animHealTo30;

        #region Idle
        
        public void PlayIdle(int state)
        {
            switch (state)
            {
                case 3:
                    PlayIdle100();
                    break;
                case 2:
                    PlayIdle70();
                    break;
                case 1:
                    PlayIdle30();
                    break;
                default:
                    PlayIdle0();
                    break;
            }
        }
        
        public void PlayIdle100()
        {
            if (!skeleton || skeleton.AnimationState == null) return;
            skeleton.Initialize(false);
            skeleton.AnimationState.SetAnimation(0, animIdle100, true);
        }

        public void PlayIdle70()
        {
            if (!skeleton || skeleton.AnimationState == null) return;
            skeleton.Initialize(false);
            skeleton.AnimationState.SetAnimation(0, animIdle70, true);
        }

        public void PlayIdle30()
        {
            if (!skeleton || skeleton.AnimationState == null) return;
            skeleton.Initialize(false);
            skeleton.AnimationState.SetAnimation(0, animIdle30, true);
        }

        public void PlayIdle0()
        {
            if (!skeleton || skeleton.AnimationState == null) return;
            skeleton.Initialize(false);
            skeleton.AnimationState.SetAnimation(0, animIdle30, true);
        }
        
        #endregion

        #region Transition

        public void TransitionToIdle(int state, bool forward)
        {
            switch (state)
            {
                case 3:
                    if (forward) PlayIdle100();
                    else PLayHealTo100();
                    break;
                case 2:
                    if (forward) PlayBreakTo70();
                    else PlayHealTo70();
                    break;
                case 1:
                    if (forward) PlayBreakTo30();
                    else PlayHealTo30();
                    break;
                default:
                    if (forward) PlayBreakTo0();
                    else PlayIdle0();
                    break;
            }
        }
        
        public void PlayBreakTo70()
        {
            if (!skeleton || skeleton.AnimationState == null) return;
            skeleton.Initialize(false);
            skeleton.AnimationState.SetAnimation(0, animBreakTo70, false);
            skeleton.AnimationState.AddAnimation(0, animIdle70, true, 0f);
        }
        
        public void PlayBreakTo30()
        {
            if (!skeleton || skeleton.AnimationState == null) return;
            skeleton.Initialize(false);
            skeleton.AnimationState.SetAnimation(0, animBreakTo30, false);
            skeleton.AnimationState.AddAnimation(0, animIdle30, true, 0f);
        }
        
        public void PlayBreakTo0()
        {
            if (!skeleton || skeleton.AnimationState == null) return;
            skeleton.Initialize(false);
            skeleton.AnimationState.SetAnimation(0, animBreakTo0, false);
            skeleton.AnimationState.AddAnimation(0, animIdle0, true, 0f);
        }

        public void PLayHealTo100()
        {
            if (!skeleton || skeleton.AnimationState == null) return;
            skeleton.Initialize(false);
            skeleton.AnimationState.SetAnimation(0, animHealTo100, false);
            skeleton.AnimationState.AddAnimation(0, animIdle100, true, 0f);
        }

        public void PlayHealTo70()
        {
            if (!skeleton || skeleton.AnimationState == null) return;
            skeleton.Initialize(false);
            skeleton.AnimationState.SetAnimation(0, animHealTo70, false);
            skeleton.AnimationState.AddAnimation(0, animIdle70, true, 0f);
        }

        public void PlayHealTo30()
        {
            if (!skeleton || skeleton.AnimationState == null) return;
            skeleton.Initialize(false);
            skeleton.AnimationState.SetAnimation(0, animHealTo30, false);
            skeleton.AnimationState.AddAnimation(0, animIdle30, true, 0f);
        }

        #endregion

        public void PlayHit()
        {
            
        }
    }
}
