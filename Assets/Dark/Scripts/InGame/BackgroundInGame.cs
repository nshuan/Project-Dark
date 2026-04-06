using System;
using Core;
using Dark.Scripts.Utils;
using DG.Tweening;
using Sirenix.OdinInspector;
using Spine.Unity;
using UnityEngine;

namespace InGame
{
    public class BackgroundInGame : MonoBehaviour
    {
        private static readonly int MatDisolveValue = Shader.PropertyToID("Disolve_Value");
        
        [SerializeField] private GameObject groupBgNormal;
        [SerializeField] private GameObject groupBgBoss;
        [SerializeField] private GameObject groupBgBlack;
        [SerializeField] private GameObject groupBlackAll;
        [SerializeField] private GameObject vfxBgBoss;
        [SerializeField] private SpriteRenderer[] groupBgBossSpriteRenderer;
        [SerializeField] private Material matBgBoss;
        [SerializeField] private GameObject vfxAppearCrack;
        [SerializeField] private SkeletonAnimation objTransitionModel;
        [SerializeField, ShowIf("objTransitionModel")]
        private SpriteRenderer[] cracks;

        private void OnEnable()
        {
            LevelManager.Instance.OnBossWaveStart += OnStartWaveBoss;
        }

        private void OnDisable()
        {
            Reset();
            objTransitionModel?.gameObject.SetActive(false);
            LevelManager.Instance.OnBossWaveStart -= OnStartWaveBoss;
        }

        private void OnStartWaveBoss()
        {
            matBgBoss.SetFloat(MatDisolveValue, 1f);
            vfxBgBoss.SetActive(false);
            vfxAppearCrack.SetActive(false);
            foreach (var obj in groupBgBossSpriteRenderer)
            {
                obj.gameObject.SetActive(false);
                obj.color = new Color(1f, 1f, 1f, 0f);
            }
            groupBgBoss.SetActive(true);
            DOTween.Sequence(this).SetDelay(1.2f)
                .Append(DOTween.To(() => 1f, (x) => matBgBoss.SetFloat(MatDisolveValue, x), 0f, 2f))
                .AppendCallback(() =>
                {
                    groupBgNormal.SetActive(false);
                    vfxBgBoss.SetActive(true);
                    vfxAppearCrack.SetActive(true);
                    foreach (var obj in groupBgBossSpriteRenderer)
                    {
                        obj.gameObject.SetActive(true);
                        obj.DOFade(1f, 0.5f);
                    }
                });
        }

        public void ForceChangeBg(bool isBoss, bool ignoreTimeScale = false)
        {
            DOTween.Kill(this);
            
            if (isBoss)
            {
                matBgBoss.SetFloat(MatDisolveValue, 1f);
                vfxBgBoss.SetActive(false);
                groupBgBoss.SetActive(true);
                groupBgNormal.SetActive(true);
                var seq = DOTween.Sequence(this);
                if (ignoreTimeScale) seq.SetUpdate(true);
                seq.Append(DOTween.To(() => 1f, (x) => matBgBoss.SetFloat(MatDisolveValue, x), 0f, 2f))
                    .AppendCallback(() =>
                    {
                        groupBgNormal.SetActive(false);
                        vfxBgBoss.SetActive(true);
                        foreach (var obj in groupBgBossSpriteRenderer)
                        {
                            obj.gameObject.SetActive(true);
                            obj.DOFade(1f, 0.5f);
                        }
                    });
            }
            else
            {
                matBgBoss.SetFloat(MatDisolveValue, 0f);
                vfxBgBoss.SetActive(false);
                groupBgBoss.SetActive(true);
                groupBgNormal.SetActive(true);
                var seq = DOTween.Sequence(this);
                if (ignoreTimeScale) seq.SetUpdate(true);
                seq.Append(DOTween.To(() => 0f, (x) => matBgBoss.SetFloat(MatDisolveValue, x), 1f, 2f))
                    .AppendCallback(() =>
                    {
                        groupBgBoss.SetActive(false);
                    });
            }
        }

        public void SetActiveBlackBg(bool active)
        {
            groupBgBlack.SetActive(active);
            if (active)
            {
                groupBgNormal.SetActive(false);
                groupBgBoss.SetActive(false);
            }
        }

        public void SetActiveBlackAll(bool active)
        {
            groupBlackAll.SetActive(active);
            if (active)
            {
                groupBgNormal.SetActive(false);
                groupBgBoss.SetActive(false);
            }
        }

        public void PlayTransition()
        {
            if (objTransitionModel)
            {
                if (cracks != null)
                {
                    foreach (var crack in cracks)
                    {
                        crack.gameObject.SetActive(false);
                    }
                }

                if (objTransitionModel)
                {
                    objTransitionModel.state?.SetAnimation(0, "animation", false);
                    objTransitionModel.Update(0);
                    objTransitionModel.LateUpdate();
                    objTransitionModel.gameObject.SetActive(true);
                }
            }
        }

        public void Reset()
        {
            matBgBoss.SetFloat(MatDisolveValue, 1f);
            vfxBgBoss.SetActive(false);
            vfxAppearCrack.SetActive(false);
            foreach (var obj in groupBgBossSpriteRenderer)
            {
                obj.gameObject.SetActive(false);
                obj.color = new Color(1f, 1f, 1f, 0f);
            }
            groupBgBoss.SetActive(false);
            groupBgNormal.SetActive(true);
        }
    }
}