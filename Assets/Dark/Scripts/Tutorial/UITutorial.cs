using UnityEngine;

namespace Dark.Scripts.Tutorial
{
    public class UITutorial : MonoBehaviour
    {
        [SerializeField] private UIAbstractTutorialStep[] tutorialSteps;

        private int currentStepIndex = -1;

        protected virtual void Start()
        {
            StartTutorial();
        }

        protected void StartTutorial()
        {
            currentStepIndex = -1;
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

                tutorialSteps[currentStepIndex].OnComplete += NextStep;
                tutorialSteps[currentStepIndex].Setup();
                break;
            }
        }
    }
}