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

        [SerializeField] private string keyTutorial;
        [SerializeField] private Image imgCover;
        [SerializeField] private UIAbstractTutorialStep[] tutorialSteps;

        private int currentStepIndex = -1;
        private Material matCover;

        private void Awake()
        {
            matCover = imgCover.material;
        }

        protected virtual void Start()
        {
            // if (TutorialManager.Instance.GetCurrentTutorialStep(keyTutorial) >= tutorialSteps.Length) return;
            StartTutorial();
        }

        protected void StartTutorial()
        {
            // currentStepIndex = TutorialManager.Instance.GetCurrentTutorialStep(keyTutorial) - 1;
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

                var currentStep = tutorialSteps[currentStepIndex];
                var tempStepIndex = currentStepIndex;
                currentStep.OnComplete += () =>
                {
                    TutorialManager.Instance.CompleteTutorialStep(keyTutorial, tempStepIndex);
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
                        });
                    }
                    imgCover.gameObject.SetActive(true);
                    Time.timeScale = 0.2f;
                });
                
                break;
            }
        }
    }
}