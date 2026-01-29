using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dark.Scripts.Settings.Resolution
{
    /// <summary>
    /// Optional helper to drive resolution + fullscreen from UI.
    /// Drop this on a settings panel and wire a Toggle + TMP_Dropdown.
    /// </summary>
    public class ResolutionSettingsUI : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Toggle fullscreenToggle;
        [SerializeField] private TMP_Dropdown resolutionDropdown;

        [Header("Resolution list (optional override)")]
        [Tooltip("If empty, system resolutions (Screen.resolutions) are used.")]
        [SerializeField] private List<ResolutionEntry> supportedResolutions = new();

        [Header("Behavior")]
        [SerializeField] private bool initializeOnStart = true;
        [SerializeField] private bool applyOnStart = true;

        private bool _suppressCallbacks;

        private void Start()
        {
            if (initializeOnStart && !ResolutionSettings.IsInitialized)
            {
                ResolutionSettings.Initialize(
                    supportedResolutions != null && supportedResolutions.Count > 0 ? supportedResolutions : null,
                    applyNow: applyOnStart);
            }

            HookUI();
            RefreshUI();
        }

        private void HookUI()
        {
            if (fullscreenToggle)
            {
                fullscreenToggle.onValueChanged.RemoveListener(OnFullscreenToggleChanged);
                fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggleChanged);
            }

            if (resolutionDropdown)
            {
                resolutionDropdown.onValueChanged.RemoveListener(OnResolutionDropdownChanged);
                resolutionDropdown.onValueChanged.AddListener(OnResolutionDropdownChanged);
            }
        }

        public void RefreshUI()
        {
            _suppressCallbacks = true;

            try
            {
                if (fullscreenToggle)
                {
                    fullscreenToggle.isOn = ResolutionSettings.Fullscreen;
                }

                if (resolutionDropdown)
                {
                    var labels = ResolutionSettings.GetResolutionLabels();
                    resolutionDropdown.ClearOptions();
                    resolutionDropdown.AddOptions(new List<string>(labels));

                    var selectedIndex = ResolutionSettings.GetSelectedResolutionIndex();
                    if (selectedIndex >= 0 && selectedIndex < resolutionDropdown.options.Count)
                    {
                        resolutionDropdown.value = selectedIndex;
                    }
                    resolutionDropdown.RefreshShownValue();
                }
            }
            finally
            {
                _suppressCallbacks = false;
            }
        }

        public void OnFullscreenToggleChanged(bool isFullscreen)
        {
            if (_suppressCallbacks) return;
            ResolutionSettings.SetFullscreen(isFullscreen, apply: true);
        }

        public void OnResolutionDropdownChanged(int index)
        {
            if (_suppressCallbacks) return;
            ResolutionSettings.SetResolutionByIndex(index, apply: true);
        }
    }
}

