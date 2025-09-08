using Dark.Scripts.CoreUI;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public static class UIUtility
{
    public static Tween DoOpen(this UIPopup popup)
    {
        DOTween.Kill(popup);
        return DOTween.Sequence(popup).SetUpdate(true)
            .AppendCallback(() =>
            {
                popup.transform.localScale = 0.3f * Vector3.one;
                popup.gameObject.SetActive(true);
            })
            .Append(popup.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack));
    }
    
    public static Tween DoClose(this UIPopup popup)
    {
        DOTween.Kill(popup);
        return DOTween.Sequence(popup).SetUpdate(true)
            .Append(popup.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack))
            .AppendCallback(() =>
            {
                popup.gameObject.SetActive(false);
            });
    }


    public static void SetAlpha(this Image image, float alpha)
    {
        var color = image.color;
        color.a = alpha;
        image.color = color;
    }
}