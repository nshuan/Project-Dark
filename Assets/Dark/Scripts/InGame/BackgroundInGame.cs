using System;
using Core;
using DG.Tweening;
using UnityEngine;

namespace InGame
{
    public class BackgroundInGame : MonoSingleton<BackgroundInGame>
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

        private void Start()
        {
            LevelManager.Instance.OnBossWaveStart += OnStartWaveBoss;
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
    }
}