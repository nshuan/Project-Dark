using System;
using Dark.Scripts.Utils.Camera;
using UnityEngine;
using UnityEngine.UI;

namespace Dark.Scripts.Tutorial
{
    public class UITutorial : MonoBehaviour
    {
        private static readonly int MatCenterProperty = Shader.PropertyToID("_Center");
        private static readonly int MatRadiusXYProperty = Shader.PropertyToID("_RadiusXY");
        private static readonly int MatRoundnessProperty = Shader.PropertyToID("_Roundness");

        [SerializeField] private Image imgCover;
        [SerializeField] protected UIAbstractTutorialStep[] tutorialSteps;

        protected int currentStepIndex = -1;
        private Material matCover;

        private void Awake()
        {
            matCover = imgCover.material;
        }

        protected virtual void Start()
        {
            if (TutorialManager.Instance.GetCurrentTutorialStepTree() >= tutorialSteps.Length) return;
            currentStepIndex = TutorialManager.Instance.GetCurrentTutorialStepTree() - 1;
            StartTutorial();
        }

        protected void StartTutorial()
        {
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
                    CompleteStep(tempStepIndex);
                    imgCover.gameObject.SetActive(false);
                    Time.timeScale = 1f;
                    currentStep.Hide();
                    NextStep();
                };
                currentStep.Setup((focusPos, focusSize, focusRoundness, enableRaycast, enableTapToClose) =>
                {
                    matCover.SetVector(MatCenterProperty,
                        new Vector4((focusPos.x + SafeScaler.ScreenWidth / 2f) / SafeScaler.ScreenWidth,
                            (focusPos.y + SafeScaler.ScreenHeight / 2f) / SafeScaler.ScreenHeight, 0, 0));
                    matCover.SetVector(MatRadiusXYProperty,
                        new Vector4(focusSize.x, focusSize.y, 0f, 0f));
                    matCover.SetFloat(MatRoundnessProperty, focusRoundness);
                    imgCover.raycastTarget = enableRaycast;
                    if (imgCover.TryGetComponent<Button>(out var btnCover))
                    {
                        btnCover.interactable = enableTapToClose;
                        btnCover.onClick.RemoveAllListeners();
                        btnCover.onClick.AddListener(() =>
                        {
                            Time.timeScale = 1f;
                            imgCover.gameObject.SetActive(false);
                            currentStep.Hide();
                        });
                    }
                    imgCover.gameObject.SetActive(true);
                    Time.timeScale = 0.2f;
                });
                
                break;
            }
        }

        protected virtual void CompleteStep(int indexStep)
        {
            TutorialManager.Instance.CompleteTutorialStepTree(indexStep);
        }
    }
}