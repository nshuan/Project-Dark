using Data;

namespace Dark.Scripts.ForDemo
{
    public class DemoBlockLevelButton : DemoBlockButton
    {
        protected override bool ShouldShowButton()
        {
            return PlayerDataManager.Instance.Data.level >= DemoConfig.MaxDemoLevel;
        }        
    }
}