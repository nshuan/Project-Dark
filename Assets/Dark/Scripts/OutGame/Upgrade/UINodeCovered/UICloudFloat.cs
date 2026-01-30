using UnityEngine;
using DG.Tweening;

public class UICloudFloat : MonoBehaviour
{
    public CloudTweenPreset preset;

    RectTransform rect;
    Vector2 startPos;
    Vector3 startScale;

    Sequence seq;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        startPos = rect.anchoredPosition;
        startScale = rect.localScale;
    }

    void Start()
    {
        BuildSequence();
    }

    void BuildSequence()
    {
        float moveOffset = Random.Range(preset.moveRange.x, preset.moveRange.y);
        float scaleOffset = Random.Range(preset.scaleRange.x, preset.scaleRange.y);

        float moveTime = Random.Range(preset.moveDuration.x, preset.moveDuration.y);
        float scaleTime = Random.Range(preset.scaleDuration.x, preset.scaleDuration.y);

        float delay = Random.Range(preset.startDelay.x, preset.startDelay.y);

        seq = DOTween.Sequence()
            .SetUpdate(true)        // UI-safe
            .SetDelay(delay)
            .SetLoops(-1, LoopType.Restart);

        // Position (Yoyo inside the sequence)
        seq.Append(
            rect.DOAnchorPos(
                startPos + new Vector2(moveOffset, 0f),
                moveTime
            ).SetEase(Ease.InOutSine)
        );
        seq.Append(
            rect.DOAnchorPos(startPos, moveTime)
                .SetEase(Ease.InOutSine)
        );

        // Scale breathing (runs in parallel)
        seq.Join(
            rect.DOScale(
                startScale * (1f + scaleOffset),
                scaleTime
            )
            .SetEase(Ease.InOutSine)
            .SetLoops(2, LoopType.Yoyo)
        );

        seq.Play();
    }

    /// <summary>
    /// Temporarily scales animation speed once.
    /// Example: TriggerSpeedScale(2f) → twice as fast
    /// </summary>
    public void TriggerSpeedScale(float speedScale, float recoverTime = 0.6f)
    {
        if (seq == null || !seq.IsActive())
            return;

        speedScale = Mathf.Max(0.01f, speedScale);

        // Kill any previous speed tween
        DOTween.Kill(seq, complete: false);

        // Instantly apply scale
        seq.timeScale = speedScale;

        // Smoothly return to normal speed
        DOTween.To(
            () => seq.timeScale,
            x => seq.timeScale = x,
            1f,
            recoverTime
        ).SetEase(Ease.OutSine);
    }

    void OnDisable()
    {
        seq?.Kill();
    }
}
