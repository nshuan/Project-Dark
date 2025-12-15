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
            if (TutorialManager.Instance.GetCurrentTutorialStep() >= tutorialSteps.Length) return;
            currentStepIndex = TutorialManager.Instance.GetCurrentTutorialStep() - 1;
            StartTutorial();
        }

        protected override void CompleteStep(int indexStep)
        {
            TutorialManager.Instance.CompleteTutorialStep(indexStep);
        }
    }
}