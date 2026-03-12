using System;
using System.Collections;
using Dark.Scripts.AudioV2;
using Dark.Scripts.Utils;
using Dark.Scripts.Utils.Skeleton;
using Data;
using DG.Tweening;
using InGame.UI;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace InGame
{
    public class TowerDestroyedAnim : MonoBehaviour, IEndGameLoseAnimation
    {
        [SerializeField] private Transform mainSkeletonHolder;
        [SerializeField] private SkeletonAnimation mainSkeleton;
        [SpineAnimationName(nameof(mainSkeleton))]
        [SerializeField] private string animationName;
        [SpineAnimationName(nameof(mainSkeleton))] 
        [SerializeField] private string animationReturnName;
        [SerializeField] private ParticleSystem vfxExplode;
        [SerializeField] private ParticleSystem vfxFlash;
        [SerializeField] private float delayShowFlash = 2f;
        [SerializeField] private float durationDestroyTower = 0.8f;
        [SerializeField] private float durationFocusTower = 0.5f;
        [SerializeField] private float durationReturnAnim = 1.5f;
        [SerializeField] private float delayShowAnim = 0.5f;
        [SerializeField] private string skinForArcher = "Archer";
        [SerializeField] private string skinForKnight = "Knight";
        [SerializeField] private AudioPlayComponentV2 sfxDestroyed;

        public static IEndGameLoseAnimation Instance { get; private set; }
        
        private void Awake()
        {
            Instance = this;
            
            mainSkeletonHolder.gameObject.SetActive(false);
        }
        
        public IEnumerator IEPlay()
        {
            sfxDestroyed?.Play();
            
            yield return new WaitForSeconds(delayShowAnim);
            
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
            // trackEntry.TimeScale = 0f; // Pause the animation (don't play)
            mainSkeleton.AnimationState.Update(0f);
            mainSkeleton.Update(0f); // Render first frame
            
            var targetTower = LevelManager.Instance.FirstDestroyedTower ? LevelManager.Instance.FirstDestroyedTower : LevelManager.Instance.Towers[0];
            transform.position = targetTower.transform.position;

            yield return DOTween.Sequence(mainSkeletonHolder)
                .Append(mainSkeletonHolder.DOMove(new Vector3(0f, -1.5f, 0f), durationFocusTower).SetEase(Ease.OutQuad))
                .Join(mainSkeletonHolder.DOScale(1.5f, durationFocusTower))
                .WaitForCompletion();
            
            // trackEntry.TimeScale = 1f; // Resume the animation
            vfxExplode.Play();
        }

        public float Play()
        {
            mainSkeleton.skeleton.SetSkin(PlayerDataManager.Instance.Data.Class == CharacterClass.CharacterClass.Knight
                ? skinForKnight
                : skinForArcher);
            mainSkeleton.Skeleton.SetSlotsToSetupPose();
            mainSkeleton.AnimationState.Apply(mainSkeleton.Skeleton);
            StartCoroutine(IEPlay());
            return durationFocusTower + durationDestroyTower + delayShowAnim; // Thấy duration của vfx là 2s
        }

        public float PlayReturn()
        {
            mainSkeleton.Initialize(false);
            var trackEntry = mainSkeleton.AnimationState.SetAnimation(0, animationReturnName, false);
            trackEntry.TrackTime = 0f; // Set to first frame
            mainSkeleton.AnimationState.Update(0f);
            mainSkeleton.Update(0f); // Render first frame
            this.DelayCall(durationReturnAnim, () => vfxFlash.Play(true));
            return durationReturnAnim;
        }
    }
}