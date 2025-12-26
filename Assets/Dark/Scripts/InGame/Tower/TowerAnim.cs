using System;
using System.Collections;
using System.Linq;
using Dark.Scripts.Utils.Skeleton;
using Spine.Unity;
using UnityEngine;

namespace InGame
{
    public class TowerAnim : MonoBehaviour
    {
        private static readonly int OutlineWidth = Shader.PropertyToID("_OutlineWidth");
        private static readonly int Color1 = Shader.PropertyToID("_Color");
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
        [SerializeField] private string animHover100;
        
        [SpineAnimationName(nameof(skeleton))]
        [SerializeField] private string animHover70;
        
        [SpineAnimationName(nameof(skeleton))]
        [SerializeField] private string animHover30;
        
        [Header("Hit")]
        [SerializeField] private Color hitColor;
        [SerializeField] private Color normalColor;
        
        private Material outlineMaterial;
        private Coroutine coroutineHitTransition;

        private void Awake()
        {
            GetOutlineMat();
        }

        private void GetOutlineMat()
        {
            // Get the first original material
            var baseMat = skeleton.CustomMaterialOverride.Count > 0
                ? skeleton.CustomMaterialOverride.Keys.First()
                : skeleton.GetComponent<Renderer>().sharedMaterial;

            // Create your unique copy
            outlineMaterial = new Material(baseMat);

            // Register it with the override system
            skeleton.CustomMaterialOverride.Clear();
            skeleton.CustomMaterialOverride.Add(baseMat, outlineMaterial);
        }
        
        public void SetActiveOutline(bool active)
        {
            if (!outlineMaterial) GetOutlineMat();
            
            if (active)
                outlineMaterial.SetFloat(OutlineWidth, 2f);
            else 
                outlineMaterial.SetFloat(OutlineWidth, 0f);
        }

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
                    else PlayIdle30();
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

        #endregion

        #region Hover

        public void PlayHover(int state)
        {
            switch (state)
            {
                case 3:
                    PlayHover100();
                    break;
                case 2:
                    PlayHover70();
                    break;
                case 1:
                    PlayHover30();
                    break;
                default:
                    PlayHover0();
                    break;
            }
        }
        
        public void PlayHover100()
        {
            if (!skeleton || skeleton.AnimationState == null) return;
            skeleton.Initialize(false);
            skeleton.AnimationState.SetAnimation(0, animHover100, true);
        }

        public void PlayHover70()
        {
            if (!skeleton || skeleton.AnimationState == null) return;
            skeleton.Initialize(false);
            skeleton.AnimationState.SetAnimation(0, animHover70, true);
        }

        public void PlayHover30()
        {
            if (!skeleton || skeleton.AnimationState == null) return;
            skeleton.Initialize(false);
            skeleton.AnimationState.SetAnimation(0, animHover30, true);
        }

        public void PlayHover0()
        {
            if (!skeleton || skeleton.AnimationState == null) return;
            skeleton.Initialize(false);
            skeleton.AnimationState.SetAnimation(0, animIdle0, true);
        }

        #endregion
        
        public void PlayHit()
        {
            if (coroutineHitTransition != null) StopCoroutine(coroutineHitTransition);
            if (gameObject.activeInHierarchy)
                coroutineHitTransition = StartCoroutine(IEColorTransition(hitColor, normalColor, 0.1f));
        }
        
        private IEnumerator IEColorTransition(Color from, Color to, float duration)
        {
            var t = 0f;
            var color = from;
                
            while (t < duration)
            {
                t += Time.deltaTime;
                var lerp = t / duration;

                color = Color.Lerp(from, to, lerp);
                outlineMaterial.SetColor(Color1, color);
                yield return null;
            }

            color = to; // Ensure final color
            outlineMaterial.SetColor(Color1, color);
        }
    }
}
