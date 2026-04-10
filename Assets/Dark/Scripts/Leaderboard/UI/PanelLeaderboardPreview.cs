using System;
using System.Collections.Generic;
using System.Linq;
using Dark.Scripts.OutGame.SaveSlot;
using InGame.EndlessLevel;
using UnityEngine;

namespace Dark.Scripts.Leaderboard.UI
{
    public class PanelLeaderboardPreview : MonoBehaviour
    {
        [SerializeField] private GameObject panelEndless;

        private void Start()
        {
            if (LevelEndlessData.IsUnlockedFeature) return;
            
            var listAllSlot = new List<int>() { 0, 1, 2, 3 };
            if (listAllSlot.Any((s) => SaveSlotManager.Instance.IsSlotCompleted(s)))
            {
                LevelEndlessData.UnlockEndlessFeature();
            }
        }

        private void OnEnable()
        {
            panelEndless.SetActive(LevelEndlessData.IsUnlockedFeature);
        }
    }
}