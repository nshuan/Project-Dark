using System;
using DG.Tweening;
using UnityEngine;

namespace Dark.Scripts.Tutorial
{
    public class UITutorialInstruction : MonoBehaviour
    {
        private void OnEnable()
        {
            DoShow();
        }

        private Tween DoShow()
        {
            DOTween.Kill(this);
            
            transform.localScale = 1.1f * Vector3.one;
            gameObject.SetActive(true);

            return transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.InBack).SetTarget(this).SetUpdate(true);
        }
    }
}