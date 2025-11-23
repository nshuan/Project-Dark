using System;
using System.Collections;
using Dark.Scripts.Utils;
using Dark.Scripts.Utils.Skeleton;
using DG.Tweening;
using InGame.UI;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace InGame
{
    public class TowerDestroyedAnim : MonoBehaviour
    {
        [SerializeField] private Transform mainSkeletonHolder;
        [SerializeField] private SkeletonAnimation mainSkeleton;
        [SpineAnimationName(nameof(mainSkeleton))]
        [SerializeField] private string animationName;
        [SerializeField] private ParticleSystem vfxExplode;
        [SerializeField] private ParticleSystem vfxFlash;
        [SerializeField] private float delayShowFlash = 2f;

        private void Awake()
        {
            mainSkeletonHolder.gameObject.SetActive(false);
            LevelManager.Instance.OnLose += OnLose;
        }

        private void OnLose()
        {
            StartCoroutine(IEPlay());
        }

        public IEnumerator IEPlay()
        {
            BackgroundInGame.Instance.SetActiveBlackAll(true);
            CanvasInGame.Instance.HideUI();
            foreach (var tower in LevelManager.Instance.Towers)
            {
                tower.gameObject.SetActive(false);
            }
            
            mainSkeletonHolder.gameObject.SetActive(true);
            mainSkeleton.Initialize(false);
            var trackEntry = mainSkeleton.AnimationState.SetAnimation(0, animationName, false);
            trackEntry.TrackTime = 0f; // Set to first frame
            trackEntry.TimeScale = 0f; // Pause the animation (don't play)
            mainSkeleton.AnimationState.Update(0f);
            mainSkeleton.Update(0f); // Render first frame
            // mainSkeleton.AnimationState.Event += HandleEvent;
            
            foreach (var tower in LevelManager.Instance.Towers)
            {
                if (tower.IsDestroyed)
                {
                    transform.position = tower.transform.position;
                    break;
                }
            }

            yield return DOTween.Sequence(mainSkeletonHolder)
                .Append(mainSkeletonHolder.DOMove(new Vector3(0f, -1.5f, 0f), 0.5f).SetEase(Ease.OutQuad))
                .Join(mainSkeletonHolder.DOScale(1.5f, 0.5f))
                .WaitForCompletion();
            
            trackEntry = mainSkeleton.AnimationState.GetCurrent(0);
            if (trackEntry != null)
            {
                trackEntry.TimeScale = 1f; // Resume the animation
            }
                    
            vfxExplode.Play();
            
            this.DelayCall(delayShowFlash, () => vfxFlash.Play());
        }

        private void HandleEvent(TrackEntry trackEntry, Spine.Event e)
        {
            DebugUtility.Log("Event fired: " + e.Data.Name);
            vfxFlash.Play(true);
        }
    }
}