using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace InGame.UI.Waves
{
    public class UIWaveProcessItemV2 : MonoBehaviour
    {
        [SerializeField] private Image imgPassed;
        [SerializeField] private Image imgGradient;

        private void Start()
        {
            imgGradient.SetAlpha(0f);
        }

        public void DoPassed()
        {
            imgPassed.SetAlpha(0f);
            imgPassed.gameObject.SetActive(true);
            imgPassed.DOFade(1f, 0.5f);
        }

        public Tween DoUpdateGradient(float alpha, float duration)
        {
            return imgGradient.DOFade(alpha, duration);
        }
    }
}