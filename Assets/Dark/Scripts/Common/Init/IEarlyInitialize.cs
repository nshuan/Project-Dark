using System;
using System.Collections.Generic;
using Dark.Scripts.ForDemo;
using Dark.Scripts.Leaderboard;
using Dark.Scripts.OutGame.SaveSlot;
using Dark.Scripts.Settings;
using Dark.Scripts.Settings.Resolution;
using Data;
using InGame.Upgrade;
using Dark.Scripts.STOVE;
using UnityEngine;

namespace Dark.Scripts.Common.Init
{
    public interface IEarlyInitialize
    {
        public void Initialize();
    }
    
    [Serializable]
    public class DemoConfigAutoLoader : IEarlyInitialize
    {
        public void Initialize()
        {
            DemoConfig.Instance.InitPublicProperties();
        }
    }
    
    [Serializable]
    public class GameSettingsAutoLoader : IEarlyInitialize
    {
        [SerializeField] private List<ResolutionEntry> availableResolutions = new List<ResolutionEntry>()
            { new ResolutionEntry(1920, 1080) };
        
        public void Initialize()
        {
            GameSettings.Initialize();
            ResolutionSettings.Initialize(availableResolutions, applyNow: true);
        }
    }
    
    [Serializable]
    public class DataInitializer : IEarlyInitialize
    {
        public void Initialize()
        {
            _ = PlayerDataManager.Instance;
            _ = UpgradeManager.Instance;
        }
    }

    [Serializable]
    public class GameSupportStatsInitializer : IEarlyInitialize
    {
        public void Initialize()
        {
            GameSupportStats.Instance.Initialize();
        }
    }
    
    [Serializable]
    public class LeaderboardSpeedrunInitializer : IEarlyInitialize
    {
        public void Initialize()
        {
            LeaderboardManager.Instance.Initialize();
        }
    }
}