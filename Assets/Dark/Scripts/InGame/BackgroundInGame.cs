using System;
using DG.Tweening;
using UnityEngine;

namespace InGame
{
    public class BackgroundInGame : MonoBehaviour
    {
        private static readonly int MatDisolveValue = Shader.PropertyToID("Disolve_Value");
        
        [SerializeField] private GameObject groupBgNormal;
        [SerializeField] private GameObject groupBgBoss;
        [SerializeField] private GameObject vfxBgBoss;
        [SerializeField] private Material matBgBoss;

        private void Start()
        {
            LevelManager.Instance.OnBossWaveStart += OnStartWaveBoss;
        }

        private void OnStartWaveBoss()
        {
            matBgBoss.SetFloat(MatDisolveValue, 1f);
            vfxBgBoss.SetActive(false);
            groupBgBoss.SetActive(true);
            DOTween.Sequence(this).SetDelay(1.2f)
                .Append(DOTween.To(() => 1f, (x) => matBgBoss.SetFloat(MatDisolveValue, x), 0f, 2f))
                .AppendCallback(() =>
                {
                    groupBgNormal.SetActive(false);
                    vfxBgBoss.SetActive(true);
                });
        }
    }
}