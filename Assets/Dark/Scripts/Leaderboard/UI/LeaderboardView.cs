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
        [SerializeField, HideIf("isClassLeaderboard")] private bool isEndlessLeaderboard;
        [SerializeField] private bool isNavigateToPlayerOnFirstLoad;
        [SerializeField] private bool isKeepingTop1;
        [SerializeField] private bool enableDebugLogs = true;
        
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
        readonly List<LeaderboardEntryData> _visibleEntries = new List<LeaderboardEntryData>(64);
        Coroutine _scrollRoutine;

        private int playerRankIndex = -1;
        private bool shouldNavigatedPlayer;
        private LeaderboardEntryData _top1Entry;

        private void Awake()
        {
            if (!isClassLeaderboard)
            {
                if (isEndlessLeaderboard) manager = LeaderboardManager.Instance.GetEndlessLeaderboard();
                else manager = LeaderboardManager.Instance.GetFullLeaderboard();
            }
            else
            {
                isEndlessLeaderboard = false;
                manager = LeaderboardManager.Instance.GetLeaderboard(leaderboardClass);
            }

            Log($"Awake. manager={(manager == null ? "null" : manager.name)}, isClassLeaderboard={isClassLeaderboard}, class={leaderboardClass}, isEndlessLeaderboard={isEndlessLeaderboard}.");
            
            btnNavigatePlayerRank.onClick.RemoveAllListeners();
            btnNavigatePlayerRank.onClick.AddListener(NavigatePlayer);
            shouldNavigatedPlayer = isNavigateToPlayerOnFirstLoad;
        }

        void OnEnable()
        {
            playerRankIndex = -1;
            if (top1Item) top1Item.IsEndlessLeaderboard = isEndlessLeaderboard;
            top1Item?.SetData(null);
            foreach (Transform child in content.transform)
            {
                child.gameObject.SetActive(false);
            }
            
            if (manager != null)
            {
                Log($"OnEnable. Subscribing and downloading top ranks.");
                manager.OnTopScoresDownloaded += OnTopScoresDownloaded;
                manager.OnPlayerScoresDownloaded += OnPlayerScoresDownloaded;
                manager.DownloadTop(100);
            }
            else
            {
                Log("OnEnable skipped because manager is null.");
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
            Log($"Top scores downloaded. count={(entries == null ? 0 : entries.Count)}.");

            if (entries is { Count: > 0 })
            {
                // Show full score text
                foreach (var entry in entries)
                {
                    // Keep top 1 thì đây là preview leader board nên sẽ show full score text
                    if (isKeepingTop1)
                        entry.isShowFullScoreText = true;
                    else
                        entry.isShowFullScoreText = entry.rank > 1;
                }
                
                // Get current player rank
                ApplyCurrentPlayerFlag(entries);
                manager.DownloadAroundPlayer(0);
                
                _top1Entry = entries[0];
                if (top1Item) top1Item.IsEndlessLeaderboard = isEndlessLeaderboard;
                top1Item?.SetData(_top1Entry);
                if (!isKeepingTop1)
                    entries.RemoveAt(0);
            }
            else
            {
                Log("Top scores were empty. Requesting current player rank anyway for debugging.");
                manager?.DownloadAroundPlayer(0);
                shouldNavigatedPlayer = false;
                _top1Entry = null;
            }
            SetEntries(entries);
        }
        
        private void OnPlayerScoresDownloaded(List<LeaderboardEntryData> entries)
        {
            Log($"Player score downloaded. count={(entries == null ? 0 : entries.Count)}.");

            if (entries == null)
            {
                SetCurrentPlayer(null);
                shouldNavigatedPlayer = false;
            }
            else
            {
                LeaderboardEntryData result = entries.Count > 0 ? entries[0] : null;
                SetCurrentPlayer(result);
                if (result != null) playerRankIndex = result.rank;
                else playerRankIndex = -1;
                Log(result == null
                    ? "Current player rank not returned by STOVE."
                    : $"Current player rank returned. rank={result.rank}, score={result.score}, name='{result.playerName}'.");

                RefreshCurrentPlayerHighlight();
                
                if (shouldNavigatedPlayer) NavigatePlayer();
                shouldNavigatedPlayer = false;
            }
        }

        public void SetEntries(IReadOnlyList<LeaderboardEntryData> entries)
        {
            if (content == null || itemPrefab == null)
            {
                Log($"SetEntries skipped. contentNull={content == null}, itemPrefabNull={itemPrefab == null}.");
                return;
            }

            for (int i = 0; i < _active.Count; i++)
                _pool.Release(_active[i]);
            _active.Clear();
            _visibleEntries.Clear();

            if (entries != null)
            {
                for (int i = 0; i < entries.Count; i++)
                {
                    entries[i].isCurrentPlayer = playerRankIndex > 0 && entries[i].rank == playerRankIndex;
                    var item = _pool.Get(itemPrefab, content);
                    item.IsEndlessLeaderboard = isEndlessLeaderboard;
                    item.SetData(entries[i]);
                    _active.Add(item);
                    _visibleEntries.Add(entries[i]);
                }
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(content);

            if (scrollRect != null)
                scrollRect.verticalNormalizedPosition = 1f;

            Log($"SetEntries complete. activeItems={_active.Count}, playerRankIndex={playerRankIndex}, top1='{(_top1Entry == null ? "" : _top1Entry.playerName)}'.");
        }

        private void NavigatePlayer()
        {
            Log($"NavigatePlayer requested. playerRankIndex={playerRankIndex}.");
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
                Log($"SetCurrentPlayer. rank={entry.rank}, score={entry.score}, name='{entry.playerName}'.");
                txtPlayerRank.SetTextLanguageKeepFont("key_leaderboard_current_rank",
                    ("%{value}", $"#{entry.rank}"));
            }
            else
            {
                Log("SetCurrentPlayer null.");
                txtPlayerRank.SetTextLanguageKeepFont("key_leaderboard_current_rank",
                    ("%{value}", "##"));   
            }
        }

        private void ApplyCurrentPlayerFlag(IEnumerable<LeaderboardEntryData> entries)
        {
            if (entries == null || playerRankIndex <= 0)
                return;

            foreach (var entry in entries)
            {
                if (entry == null)
                    continue;

                entry.isCurrentPlayer = entry.rank == playerRankIndex;
            }
        }

        private void RefreshCurrentPlayerHighlight()
        {
            if (_top1Entry != null)
            {
                _top1Entry.isCurrentPlayer = playerRankIndex > 0 && _top1Entry.rank == playerRankIndex;
                top1Item?.SetCurrentPlayerHighlight(_top1Entry.isCurrentPlayer);
            }

            for (var i = 0; i < _active.Count; i++)
            {
                var entry = i < _visibleEntries.Count ? _visibleEntries[i] : null;
                _active[i].SetCurrentPlayerHighlight(entry != null && playerRankIndex > 0 && entry.rank == playerRankIndex);
            }

            Log($"RefreshCurrentPlayerHighlight. playerRankIndex={playerRankIndex}, top1Current={(_top1Entry != null && _top1Entry.isCurrentPlayer)}, visibleCount={_visibleEntries.Count}.");
        }

        private void Log(string message)
        {
            if (!enableDebugLogs)
                return;

            Debug.Log($"[LeaderboardView:{name}] {message}");
        }
    }
}
