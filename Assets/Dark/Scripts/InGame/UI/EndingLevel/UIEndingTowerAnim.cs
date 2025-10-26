using System.Linq;
using Dark.Scripts.Utils.Skeleton;
using Spine.Unity;
using UnityEngine;

namespace InGame.UI.EndingLevel
{
    public class UIEndingTowerAnim : MonoBehaviour
    {
        public SkeletonGraphic skeleton;
        
        [SpineGraphicName(nameof(skeleton))]
        [SerializeField] private string animIdle100;
        
        [SpineGraphicName(nameof(skeleton))]
        [SerializeField] private string animIdle70;
        
        [SpineGraphicName(nameof(skeleton))]
        [SerializeField] private string animIdle30;
        
        [SpineGraphicName(nameof(skeleton))]
        [SerializeField] private string animIdle0;

        [SpineGraphicName(nameof(skeleton))] 
        [SerializeField] private string animBreakTo70;
        
        [SpineGraphicName(nameof(skeleton))] 
        [SerializeField] private string animBreakTo30;
        
        [SpineGraphicName(nameof(skeleton))] 
        [SerializeField] private string animBreakTo0;
        
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
        
        public void BreakFromState(int state)
        {
            switch (state)
            {
                case 3:
                    BreakFrom100();
                    break;
                case 2:
                    BreakFrom70();
                    break;
                case 1:
                    BreakFrom30();
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

        public void BreakFrom100()
        {
            if (!skeleton || skeleton.AnimationState == null) return;
            skeleton.Initialize(false);
            skeleton.AnimationState.SetAnimation(0, animBreakTo70, false);
            skeleton.AnimationState.AddAnimation(0, animBreakTo30, false, 0f);
            skeleton.AnimationState.AddAnimation(0, animBreakTo0, false, 0f);
        }

        public void BreakFrom70()
        {
            if (!skeleton || skeleton.AnimationState == null) return;
            skeleton.Initialize(false);
            skeleton.AnimationState.SetAnimation(0, animBreakTo30, false);
            skeleton.AnimationState.AddAnimation(0, animBreakTo0, false, 0f);
        }

        public void BreakFrom30()
        {
            if (!skeleton || skeleton.AnimationState == null) return;
            skeleton.Initialize(false);
            skeleton.AnimationState.SetAnimation(0, animBreakTo0, false);
        }
    }
}