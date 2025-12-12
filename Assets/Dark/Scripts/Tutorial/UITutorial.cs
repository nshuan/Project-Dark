using UnityEngine;
using UnityEngine.UI;

namespace Dark.Scripts.Tutorial
{
    public class UITutorial : MonoBehaviour
    {
        [SerializeField] private string keyTutorial;
        [SerializeField] private Image imgCover;
        [SerializeField] private UIAbstractTutorialStep[] tutorialSteps;

        private int currentStepIndex = -1;

        protected virtual void Start()
        {
            if (TutorialManager.Instance.GetCurrentTutorialStep(keyTutorial) >= tutorialSteps.Length) return;
            StartTutorial();
        }

        protected void StartTutorial()
        {
            currentStepIndex = TutorialManager.Instance.GetCurrentTutorialStep(keyTutorial) - 1;
            NextStep();
        }

        protected void NextStep()
        {
            while (true)
            {
                currentStepIndex += 1;
                if (currentStepIndex >= tutorialSteps.Length)
                {
                    gameObject.SetActive(false);
                    return;
                }

                if (!tutorialSteps[currentStepIndex].IsValid())
                {
                    continue;
                }

                var currentStep = tutorialSteps[currentStepIndex];
                var tempStepIndex = currentStepIndex;
                currentStep.OnComplete += () =>
                {
                    TutorialManager.Instance.CompleteTutorialStep(keyTutorial, tempStepIndex);
                    currentStep.Hide();
                    NextStep();
                };
                currentStep.Setup();
                break;
            }
        }
    }
}