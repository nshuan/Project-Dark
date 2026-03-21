using System;
using System.Collections.Generic;
using Dark.Scripts.Leaderboard;
using Data;
using InGame.CharacterClass;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Dark.Scripts.Leaderboard.UI
{
    public sealed class LeaderboardView : MonoBehaviour
    {
        [Header("Config")] 
        [SerializeField] private bool isClassLeaderboard;
        [SerializeField, ShowIf("isClassLeaderboard")] private CharacterClass leaderboardClass;
        
        [Header("UI")]
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private RectTransform content;
        [SerializeField] private LeaderboardItemView itemPrefab;
        
        private GameCompletionLeaderboardManager manager;

        readonly ComponentPool<LeaderboardItemView> _pool = new ComponentPool<LeaderboardItemView>();
        readonly List<LeaderboardItemView> _active = new List<LeaderboardItemView>(64);

        private void Awake()
        {
            if (!isClassLeaderboard)
                manager = LeaderboardManager.Instance.GetFullLeaderboard();
            else
                manager = LeaderboardManager.Instance.GetLeaderboard(leaderboardClass);
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
