using System;
using Dark.Scripts.ForDemo;
using Dark.Scripts.OutGame.SaveSlot;
using Dark.Scripts.Settings;
using Data;
using InGame.Upgrade;

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
            if (DemoConfig.IsDemo)
                DemoConfig.Instance.InitPublicProperties();
        }
    }
    
    [Serializable]
    public class GameSettingsAutoLoader : IEarlyInitialize
    {
        public void Initialize()
        {
            GameSettings.Initialize();
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
}