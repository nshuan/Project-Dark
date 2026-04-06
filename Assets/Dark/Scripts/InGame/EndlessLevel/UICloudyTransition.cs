using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace InGame.EndlessLevel
{
    /// <summary>
    /// Controls a group of cloud images under this GameObject to perform
    /// a cloudy transition effect.
    ///
    /// Place this script on a parent object whose children are cloud
    /// `RectTransform`s. Their authored positions in the scene are
    /// treated as the "spread" state.
    ///
    /// - Transition In  : clouds spread outward from their original positions.
    /// - Transition Out : clouds return from spread positions to their original positions.
    /// </summary>
    public class UICloudyTransition : MonoBehaviour
    {
        [Header("Cloud Selection")]
        [Tooltip("If true, search all descendants for RectTransforms, not only direct children.")]
        [SerializeField] private bool includeInactiveChildren = false;
        [SerializeField] private bool searchDeepHierarchy = true;

        [Header("Animation")]
        [Tooltip("Duration of the transition animation in seconds.")]
        [SerializeField] private float duration = 0.6f;

        [Tooltip("Curve controlling easing over the duration.")]
        [SerializeField] private AnimationCurve easeCurve =
            AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Tooltip("Slight random offset to each cloud's timing for a more organic feel.")]
        [SerializeField] private float perCloudTimeOffset = 0.1f;

        [Header("Center Point")]
        [Tooltip("Center point used as spread origin reference. If null, uses this RectTransform's origin (0,0).")]
        [SerializeField] private RectTransform centerPoint;

        [Header("Spread")]
        [Tooltip("How far each cloud moves away from center during transition in.")]
        [SerializeField] private float spreadDistance = 400f;

        [Header("Events")]
        [SerializeField] private UnityEvent onTransitionInComplete;
        [SerializeField] private UnityEvent onTransitionOutComplete;

        [Header("Initial State")]
        [Tooltip("Hide all cloud graphics when this component initializes.")]
        [SerializeField] private bool hideCloudsOnInitialize = true;

        private struct CloudData
        {
            public RectTransform rect;
            public Vector3 originalLocalPos;
            public float timeOffset;
            public Graphic graphic;
            public float originalAlpha;
        }

        private readonly List<CloudData> _clouds = new List<CloudData>();
        private Coroutine _runningCoroutine;
        private RectTransform _selfRect;

        private void Awake()
        {
            _selfRect = GetComponent<RectTransform>();
            CacheClouds();

            if (hideCloudsOnInitialize)
                SetCloudsVisible(false);
        }

        private void CacheClouds()
        {
            _clouds.Clear();

            if (searchDeepHierarchy)
            {
                var rects = GetComponentsInChildren<RectTransform>(includeInactiveChildren);
                foreach (var r in rects)
                {
                    if (r == _selfRect) continue;

                    var data = new CloudData
                    {
                        rect = r,
                        originalLocalPos = r.localPosition,
                        timeOffset = Random.Range(-perCloudTimeOffset, perCloudTimeOffset),
                        graphic = r.GetComponent<Graphic>(),
                        originalAlpha = r.TryGetComponent<Graphic>(out var graphic) ? graphic.color.a : 1f
                    };
                    _clouds.Add(data);
                }
            }
            else
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    var child = transform.GetChild(i) as RectTransform;
                    if (child == null) continue;

                    var data = new CloudData
                    {
                        rect = child,
                        originalLocalPos = child.localPosition,
                        timeOffset = Random.Range(-perCloudTimeOffset, perCloudTimeOffset),
                        graphic = child.GetComponent<Graphic>(),
                        originalAlpha = child.TryGetComponent<Graphic>(out var graphic) ? graphic.color.a : 1f
                    };
                    _clouds.Add(data);
                }
            }
        }

        /// <summary>
        /// Immediately place all clouds in the "spread" layout (their authored positions).
        /// </summary>
        public void SnapToSpread()
        {
            StopRunning();
            for (int i = 0; i < _clouds.Count; i++)
            {
                var c = _clouds[i];
                if (c.rect == null) continue;
                c.rect.localPosition = c.originalLocalPos;
            }
        }

        /// <summary>
        /// Immediately place all clouds at the center point.
        /// </summary>
        public void SnapToCenter()
        {
            StopRunning();
            var center = GetCenterLocalPosition();
            for (int i = 0; i < _clouds.Count; i++)
            {
                var c = _clouds[i];
                if (c.rect == null) continue;
                c.rect.localPosition = center;
            }
        }

        /// <summary>
        /// Plays a transition-in: clouds spread outward from their original positions.
        /// </summary>
        [Button]
        public void PlayTransitionIn()
        {
            SetCloudsVisible(true);
            RestartCoroutine(AnimateClouds(isTransitionIn: true));
        }

        /// <summary>
        /// Plays a transition-out: clouds return to their original positions.
        /// </summary>
        [Button]
        public void PlayTransitionOut()
        {
            SetCloudsVisible(true);
            SnapToSpreaded();
            RestartCoroutine(AnimateClouds(isTransitionIn: false));
        }

        private void SetCloudsVisible(bool visible)
        {
            for (int i = 0; i < _clouds.Count; i++)
            {
                var c = _clouds[i];
                if (c.graphic == null) continue;
                c.graphic.enabled = visible;
            }
        }

        private static void SetGraphicAlpha(Graphic graphic, float alpha)
        {
            var color = graphic.color;
            color.a = alpha;
            graphic.color = color;
        }

        private void RestartCoroutine(IEnumerator routine)
        {
            StopRunning();
            _runningCoroutine = StartCoroutine(routine);
        }

        private void StopRunning()
        {
            if (_runningCoroutine != null)
            {
                StopCoroutine(_runningCoroutine);
                _runningCoroutine = null;
            }
        }

        private Vector3 GetCenterLocalPosition()
        {
            if (centerPoint != null)
            {
                if (centerPoint.transform == transform)
                    return centerPoint.localPosition;

                return transform.InverseTransformPoint(centerPoint.TransformPoint(Vector3.zero));
            }

            return Vector3.zero;
        }

        private Vector3 GetSpreadedPosition(CloudData c, int index, Vector3 center)
        {
            Vector3 fromCenter = c.originalLocalPos - center;
            if (fromCenter.sqrMagnitude < 0.0001f)
            {
                // Fallback direction for clouds near center.
                float angle = (360f / Mathf.Max(1, _clouds.Count)) * index * Mathf.Deg2Rad;
                fromCenter = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);
            }

            return c.originalLocalPos + fromCenter.normalized * spreadDistance;
        }

        private void SnapToSpreaded()
        {
            var center = GetCenterLocalPosition();
            for (int i = 0; i < _clouds.Count; i++)
            {
                var c = _clouds[i];
                if (c.rect == null) continue;
                c.rect.localPosition = GetSpreadedPosition(c, i, center);
            }
        }

        private IEnumerator AnimateClouds(bool isTransitionIn)
        {
            var center = GetCenterLocalPosition();

            var starts = new Vector3[_clouds.Count];
            var ends = new Vector3[_clouds.Count];
            var offsets = new float[_clouds.Count];
            var startAlphas = new float[_clouds.Count];
            var endAlphas = new float[_clouds.Count];

            for (int i = 0; i < _clouds.Count; i++)
            {
                var c = _clouds[i];
                if (c.rect == null) continue;

                if (isTransitionIn)
                {
                    starts[i] = c.rect.localPosition;
                    ends[i] = GetSpreadedPosition(c, i, center);
                }
                else
                {
                    starts[i] = c.rect.localPosition;
                    ends[i] = c.originalLocalPos;
                }

                offsets[i] = c.timeOffset;

                if (c.graphic != null)
                {
                    c.graphic.enabled = true;

                    if (isTransitionIn)
                    {
                        float currentAlpha = c.graphic.color.a;
                        startAlphas[i] = currentAlpha > 0.001f ? currentAlpha : c.originalAlpha;
                        endAlphas[i] = 0f;
                    }
                    else
                    {
                        startAlphas[i] = 0f;
                        endAlphas[i] = c.originalAlpha;
                    }

                    SetGraphicAlpha(c.graphic, startAlphas[i]);
                }
            }

            float t = 0f;
            float total = Mathf.Max(0.01f, duration);

            while (t < total)
            {
                t += Time.unscaledDeltaTime;

                for (int i = 0; i < _clouds.Count; i++)
                {
                    var c = _clouds[i];
                    if (c.rect == null) continue;

                    float normalized = Mathf.Clamp01((t + offsets[i]) / total);
                    float eased = easeCurve != null ? easeCurve.Evaluate(normalized) : normalized;
                    c.rect.localPosition = Vector3.LerpUnclamped(starts[i], ends[i], eased);

                    if (c.graphic != null)
                    {
                        float alpha = Mathf.LerpUnclamped(startAlphas[i], endAlphas[i], eased);
                        SetGraphicAlpha(c.graphic, alpha);
                    }
                }

                yield return null;
            }

            for (int i = 0; i < _clouds.Count; i++)
            {
                var c = _clouds[i];
                if (c.rect == null) continue;
                c.rect.localPosition = isTransitionIn ? ends[i] : c.originalLocalPos;

                if (c.graphic != null)
                {
                    SetGraphicAlpha(c.graphic, isTransitionIn ? 0f : c.originalAlpha);
                    if (isTransitionIn)
                        c.graphic.enabled = false;
                }
            }

            _runningCoroutine = null;

            if (isTransitionIn)
                onTransitionInComplete?.Invoke();
            else
                onTransitionOutComplete?.Invoke();
        }
    }
}
