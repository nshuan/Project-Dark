using System;
using Dark.Scripts.ForDemo;
using Dark.Scripts.Settings;

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
}