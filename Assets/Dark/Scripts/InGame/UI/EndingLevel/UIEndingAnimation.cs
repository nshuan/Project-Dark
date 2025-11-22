using System;
using Coffee.UIExtensions;
using Dark.Scripts.Utils.Skeleton;
using DG.Tweening;
using Sirenix.OdinInspector;
using Spine.Unity;
using UnityEngine;

namespace InGame.UI.EndingLevel
{
    public class UIEndingAnimation : MonoBehaviour
    {
        [SerializeField] private CanvasGroup groupEffect;
        [SerializeField] private Transform mainSkeletonHolder;
        [SerializeField] private SkeletonGraphic mainSkeleton;
        [SpineGraphicName(nameof(mainSkeleton))]
        [SerializeField] private string animationName;
        [SerializeField] private UIParticle vfxExplode;

        private Camera cam;
        private Vector3 mainSkeletonHolderOriginalScale;
        
        private void Awake()
        {
            cam = Camera.main;
            mainSkeletonHolderOriginalScale = mainSkeletonHolder.localScale;
            mainSkeleton.UnscaledTime = true;
        }

        public void Play()
        {
            groupEffect.alpha = 0f;
            groupEffect.gameObject.SetActive(true);
            mainSkeletonHolder.localScale = mainSkeletonHolderOriginalScale;
            mainSkeleton.Initialize(false);
            var trackEntry = mainSkeleton.AnimationState.SetAnimation(0, animationName, false);
            trackEntry.TrackTime = 0f; // Set to first frame
            trackEntry.TimeScale = 0f; // Pause the animation (don't play)
            mainSkeleton.AnimationState.Update(0f);
            mainSkeleton.Update(0f); // Render first frame
            
            foreach (var tower in LevelManager.Instance.Towers)
            {
                if (tower.IsDestroyed)
                {
                    mainSkeletonHolder.position = cam.WorldToScreenPoint(tower.transform.position);
                    break;
                }
            }
            
            DOTween.Kill(this);
            DOTween.Sequence(this).SetUpdate(true)
                .Append(groupEffect.DOFade(1f, 0.1f))
                // .Append(mainSkeletonHolder.DOScale(0.25f, 0.5f).SetRelative(true))
                // .Join(mainSkeletonHolder.DOLocalMove(Vector3.zero, 0.5f))
                .AppendCallback(() =>
                {
                    // Start playing the animation
                    var trackEntry = mainSkeleton.AnimationState.GetCurrent(0);
                    if (trackEntry != null)
                    {
                        trackEntry.TimeScale = 1f; // Resume the animation
                    }
                    
                    vfxExplode.Play();
                });
        }
    }
}