using System;
using UnityEngine;

namespace InGame.EndlessLevel
{
    public class UIEndlessMapChange : MonoBehaviour
    {
        [SerializeField] private UICloudyTransition uiCloudyTransition;

        private void Start()
        {
            LevelEndlessManager.OnStartHideMap += OnHideMap;
            LevelEndlessManager.OnStartShowMap += OnShowMap;
        }

        private void OnDestroy()
        {
            LevelEndlessManager.OnStartHideMap -= OnHideMap;
            LevelEndlessManager.OnStartShowMap -= OnShowMap;
        }

        private void OnHideMap(float duration)
        {
            uiCloudyTransition.PlayTransitionOut();
        }

        private void OnShowMap(float duration)
        {
            uiCloudyTransition.PlayTransitionIn();
        }
    }
}