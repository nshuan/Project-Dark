using System;
using System.Collections.Generic;
using Dark.Scripts.Leaderboard;
using Dark.Tools.Language.Runtime;
using Data;
using InGame.CharacterClass;
using Sirenix.OdinInspector;
using TMPro;
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
        [SerializeField] private LeaderboardItemView top1Item;
        [SerializeField] private TextMeshProUGUI txtPlayerRank;
        
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
            {
                manager.OnTopScoresDownloaded += OnTopScoresDownloaded;
                manager.OnPlayerScoresDownloaded += OnPlayerScoresDownloaded;
                manager.DownloadTop(10);
                manager.DownloadAroundPlayer(5);
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
                top1Item.SetData(entries[0]);
                entries.RemoveAt(0);
            }
            SetEntries(entries);
        }
        
        private void OnPlayerScoresDownloaded(List<LeaderboardEntryData> entries)
        {
            if (entries is { Count: > 0 })
            {
                txtPlayerRank.SetTextLanguage("key_leaderboard_current_rank",
                    ("%{value}", $"#{entries[(entries.Count - 1) / 2].rank}"));
            }
            else
            {
                txtPlayerRank.SetTextLanguageKeepFont("key_leaderboard_current_rank",
                    ("%{value}", "##"));   
            }
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
