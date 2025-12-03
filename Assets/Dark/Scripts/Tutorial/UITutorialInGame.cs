using InGame;

namespace Dark.Scripts.Tutorial
{
    public class UITutorialInGame : UITutorial
    {
        protected override void Start()
        {
            LevelManager.Instance.OnLevelLoaded += OnLevelLoaded;
        }

        private void OnLevelLoaded(LevelConfig level)
        {
            StartTutorial();
        }
    }
}