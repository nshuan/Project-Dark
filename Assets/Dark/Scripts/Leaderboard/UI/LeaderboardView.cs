using System;
using System.Collections.Generic;
using Dark.Scripts.Leaderboard;
using UnityEngine;
using UnityEngine.UI;

namespace Dark.Scripts.Leaderboard.UI
{
    public sealed class LeaderboardView : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] ScrollRect scrollRect;
        [SerializeField] RectTransform content;
        [SerializeField] LeaderboardItemView itemPrefab;
        
        private GameCompletionLeaderboardManager manager;

        readonly ComponentPool<LeaderboardItemView> _pool = new ComponentPool<LeaderboardItemView>();
        readonly List<LeaderboardItemView> _active = new List<LeaderboardItemView>(64);

        private void Awake()
        {
            manager = GameCompletionLeaderboardManager.Instance;
        }

        void OnEnable()
        {
            if (manager != null)
                manager.OnScoresDownloaded += SetEntries;
        }

        void OnDisable()
        {
            if (manager != null)
                manager.OnScoresDownloaded -= SetEntries;
        }

        public void SetEntries(List<LeaderboardEntryData> entries)
        {
            SetEntries((IReadOnlyList<LeaderboardEntryData>)entries);
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
    }
}
