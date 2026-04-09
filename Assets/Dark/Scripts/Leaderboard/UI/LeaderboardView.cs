using System.Collections;
using System.Collections.Generic;
using Dark.Tools.Language.Runtime;
using InGame.CharacterClass;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dark.Scripts.Leaderboard.UI
{
    public sealed class LeaderboardView : MonoBehaviour
    {
        public enum ScrollAlign
        {
            Top,
            Center,
            Bottom
        }
        
        [Header("Config")] 
        [SerializeField] private bool isClassLeaderboard;
        [SerializeField, ShowIf("isClassLeaderboard")] private CharacterClass leaderboardClass;
        [SerializeField] private bool isNavigateToPlayerOnFirstLoad;
        [SerializeField] private bool isKeepingTop1;
        
        [Header("UI")]
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private RectTransform content;
        [SerializeField] private LeaderboardItemView itemPrefab;
        [SerializeField] private LeaderboardItemView top1Item;
        [SerializeField] private TextMeshProUGUI txtPlayerRank;
        [SerializeField] private Button btnNavigatePlayerRank;
        
        private GameCompletionLeaderboardManager manager;

        readonly ComponentPool<LeaderboardItemView> _pool = new ComponentPool<LeaderboardItemView>();
        readonly List<LeaderboardItemView> _active = new List<LeaderboardItemView>(64);
        Coroutine _scrollRoutine;

        private int playerRankIndex = -1;
        private bool shouldNavigatedPlayer;

        private void Awake()
        {
            if (!isClassLeaderboard)
                manager = LeaderboardManager.Instance.GetFullLeaderboard();
            else
                manager = LeaderboardManager.Instance.GetLeaderboard(leaderboardClass);
            
            btnNavigatePlayerRank.onClick.RemoveAllListeners();
            btnNavigatePlayerRank.onClick.AddListener(NavigatePlayer);
            shouldNavigatedPlayer = isNavigateToPlayerOnFirstLoad;
        }

        void OnEnable()
        {
            playerRankIndex = -1;
            top1Item?.SetData(null);
            foreach (Transform child in content.transform)
            {
                child.gameObject.SetActive(false);
            }
            
            if (manager != null)
            {
                manager.OnTopScoresDownloaded += OnTopScoresDownloaded;
                manager.OnPlayerScoresDownloaded += OnPlayerScoresDownloaded;
                manager.DownloadTop(100);
            }
        }

        void OnDisable()
        {
            if (manager != null)
            {
                manager.OnTopScoresDownloaded -= OnTopScoresDownloaded;
                manager.OnPlayerScoresDownloaded -= OnPlayerScoresDownloaded;
            }
        }

        public void OnTopScoresDownloaded(List<LeaderboardEntryData> entries)
        {
            if (entries is { Count: > 0 })
            {
                top1Item?.SetData(entries[0]);
                entries.RemoveAt(0);
            }
            SetEntries(entries);
        }
        
        private void OnPlayerScoresDownloaded(List<LeaderboardEntryData> entries)
        {
            
        }

        public void SetEntries(IReadOnlyList<LeaderboardEntryData> entries)
        {
            if (content == null || itemPrefab == null)
                return;

            for (int i = 0; i < _active.Count; i++)
                _pool.Release(_active[i]);
            _active.Clear();

            if (entries != null)
            {
                for (int i = 0; i < entries.Count; i++)
                {
                    var item = _pool.Get(itemPrefab, content);
                    item.SetData(entries[i]);
                    _active.Add(item);
                }
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(content);

            if (scrollRect != null)
                scrollRect.verticalNormalizedPosition = 1f;
        }

        private void NavigatePlayer()
        {
            if (playerRankIndex > 0) ScrollToIndex(playerRankIndex, ScrollAlign.Top, 0.5f);
        }
        
        [Button]
        private void TestScroll(int index, ScrollAlign align, float duration = 0f)
        {
            ScrollToIndex(index, align, duration);    
        }
        
        public void ScrollToIndex(int index, ScrollAlign align = ScrollAlign.Center, float duration = 0f)
        {
            if (_scrollRoutine != null)
                StopCoroutine(_scrollRoutine);

            _scrollRoutine = StartCoroutine(ScrollToIndexRoutine(index, align, duration));
        }

        // index is based on the "downloaded entries list" where index 0 corresponds to `top1Item`.
        // The scrollable items start at index 1, because `top1Item` is not part of the ScrollRect content.
        public void ScrollToIndexWithTop1(int index, ScrollAlign align = ScrollAlign.Center, float duration = 0f)
        {
            if (index <= 0)
            {
                if (scrollRect != null)
                    scrollRect.verticalNormalizedPosition = 1f; // top
                return;
            }

            // Map downloaded index to scrollable index.
            ScrollToIndex(index - 1, align, duration);
        }

        IEnumerator ScrollToIndexRoutine(int index, ScrollAlign align, float duration)
        {
            if (scrollRect == null || content == null)
                yield break;

            if (_active.Count <= 0)
                yield break;

            // Be resilient to off-by-one mistakes (e.g. caller used downloaded entry index).
            index = Mathf.Clamp(index, 0, _active.Count - 1);

            LayoutRebuilder.ForceRebuildLayoutImmediate(content);
            Canvas.ForceUpdateCanvases();

            // Wait a frame so ScrollRect can apply its internal content movement.
            yield return null;
            Canvas.ForceUpdateCanvases();

            var viewport = scrollRect.viewport != null ? scrollRect.viewport : content;
            var item = _active[index];
            var itemRect = item != null ? item.GetComponent<RectTransform>() : null;
            if (itemRect == null)
                yield break;

            // If content fits inside the viewport, there's nowhere to scroll.
            float viewportHeight = viewport.rect.height;
            float contentHeight = content.rect.height;
            float scrollableHeight = contentHeight - viewportHeight;
            if (scrollableHeight <= 0.0001f)
                yield break;

            float targetInViewportY;
            switch (align)
            {
                case ScrollAlign.Top:
                    targetInViewportY = viewport.rect.yMax;
                    break;
                case ScrollAlign.Bottom:
                    targetInViewportY = viewport.rect.yMin;
                    break;
                default:
                    targetInViewportY = viewport.rect.center.y;
                    break;
            }

            // Convert item center into viewport local space (y units match rect units).
            var itemWorldCenter = itemRect.TransformPoint(itemRect.rect.center);
            var itemInViewport = viewport.InverseTransformPoint(itemWorldCenter);
            float deltaY = itemInViewport.y - targetInViewportY;

            // verticalNormalizedPosition: 1 == top, 0 == bottom.
            float from = scrollRect.verticalNormalizedPosition;
            float currentOffset = (1f - from) * scrollableHeight; // pixels of scroll offset

            // When we increase offset (scroll down), items move up in viewport space => their Y increases.
            // To move the item from (itemInViewportY) to targetInViewportY, we need to reduce offset by deltaY.
            float desiredOffset = currentOffset - deltaY;
            float normalizedTarget = 1f - (desiredOffset / scrollableHeight);
            normalizedTarget = Mathf.Clamp01(normalizedTarget);

            if (duration <= 0f)
            {
                scrollRect.verticalNormalizedPosition = normalizedTarget;
                yield break;
            }

            if (Mathf.Abs(from - normalizedTarget) < 0.0001f)
                yield break;

            yield return StartCoroutine(ScrollToNormalizedRoutine(from, normalizedTarget, duration));
        }

        IEnumerator ScrollToNormalizedRoutine(float from, float to, float duration)
        {
            float t = 0f;
            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                float lerpT = Mathf.Clamp01(t / duration);
                float easedT = Mathf.SmoothStep(0f, 1f, lerpT);
                scrollRect.verticalNormalizedPosition = Mathf.Lerp(from, to, easedT);
                yield return null;
            }

            scrollRect.verticalNormalizedPosition = to;
        }

        private void SetCurrentPlayer(LeaderboardEntryData entry)
        {
            if (entry != null)
            {
                txtPlayerRank.SetTextLanguageKeepFont("key_leaderboard_current_rank",
                    ("%{value}", $"#{entry.rank}"));
            }
            else
            {
                txtPlayerRank.SetTextLanguageKeepFont("key_leaderboard_current_rank",
                    ("%{value}", "##"));   
            }
        }
    }
}
