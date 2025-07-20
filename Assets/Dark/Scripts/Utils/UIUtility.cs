using Dark.Scripts.CoreUI;
using DG.Tweening;
using UnityEngine;

public static class UIUtility
{
    public static Tween DoOpen(this UIPopup popup)
    {
        return DOTween.Sequence(popup)
            .AppendCallback(() =>
            {
                popup.transform.localScale = 0.3f * Vector3.one;
                popup.gameObject.SetActive(true);
            })
            .Append(popup.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack));
    }
}