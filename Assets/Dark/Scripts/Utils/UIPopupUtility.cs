using Dark.Scripts.CoreUI;
using DG.Tweening;
using UnityEngine;

public static class UIPopupUtility
{
    /// <summary>
    /// Open popup by scaling
    /// </summary>
    /// <param name="popup"></param>
    /// <returns></returns>
    public static Tween DoOpenScale(this UIPopup popup)
    {
        var target = !popup.rectContent ? popup.transform : popup.rectContent;
        DOTween.Kill(popup);
        return DOTween.Sequence(popup).SetUpdate(true)
            .AppendCallback(() =>
            {
                target.transform.localScale = 0.3f * Vector3.one;
                target.gameObject.SetActive(true);
                popup.gameObject.SetActive(true);
                popup.groupBlockRaycast.alpha = 0f;
                popup.groupBlockRaycast.gameObject.SetActive(true);
            })
            .Append(target.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack))
            .Join(popup.groupBlockRaycast.DOFade(1f, 0.3f));
    }

    /// <summary>
    /// Open popup by fading in
    /// The target UIPopup should have a Canvas Group component,
    /// if not, add a new component of type Canvas Group
    /// </summary>
    /// <param name="popup"></param>
    /// <returns></returns>
    public static Tween DoOpenFadeIn(this UIPopup popup, float delay = 0f)
    {
        var target = !popup.rectContent ? popup.transform : popup.rectContent;
        if (!target.TryGetComponent<CanvasGroup>(out var targetCanvasGroup))
        {
            targetCanvasGroup = target.gameObject.AddComponent<CanvasGroup>();
        }

        DOTween.Kill(popup);
        return DOTween.Sequence(popup).SetUpdate(true)
            .AppendInterval(delay)
            .AppendCallback(() =>
            {
                targetCanvasGroup.alpha = 0.3f;
                target.gameObject.SetActive(true);
                popup.gameObject.SetActive(true);
                popup.groupBlockRaycast.alpha = 0f;
                popup.groupBlockRaycast.gameObject.SetActive(true);
            })
            .Append(targetCanvasGroup.DOFade(1f, 0.3f))
            .Join(popup.groupBlockRaycast.DOFade(1f, 0.3f));
    }
    
    /// <summary>
    /// Close popup by scaling
    /// </summary>
    /// <param name="popup"></param>
    /// <returns></returns>
    public static Tween DoCloseScale(this UIPopup popup)
    {
        var target = !popup.rectContent ? popup.transform : popup.rectContent;
        DOTween.Kill(popup);
        return DOTween.Sequence(popup).SetUpdate(true)
            .Append(target.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack))
            .Join(popup.groupBlockRaycast.DOFade(0f, 0.3f))
            .AppendCallback(() =>
            {
                target.gameObject.SetActive(false);
                popup.gameObject.SetActive(false);
                popup.groupBlockRaycast.gameObject.SetActive(false);
            });
    }
    
    /// <summary>
    /// Close popup by fading out
    /// The target UIPopup should have a Canvas Group component,
    /// if not, add a new component of type Canvas Group
    /// </summary>
    /// <param name="popup"></param>
    /// <returns></returns>
    public static Tween DoCloseFadeOut(this UIPopup popup)
    {
        var target = !popup.rectContent ? popup.transform : popup.rectContent;
        if (!target.TryGetComponent<CanvasGroup>(out var targetCanvasGroup))
        {
            targetCanvasGroup = target.gameObject.AddComponent<CanvasGroup>();
        }

        DOTween.Kill(popup);
        return DOTween.Sequence(popup).SetUpdate(true)
            .Append(targetCanvasGroup.DOFade(0f, 0.3f))
            .Join(popup.groupBlockRaycast.DOFade(0f, 0.3f))
            .AppendCallback(() =>
            {
                target.gameObject.SetActive(false);
                popup.gameObject.SetActive(false);
                popup.groupBlockRaycast.gameObject.SetActive(false);
            });
    }
}