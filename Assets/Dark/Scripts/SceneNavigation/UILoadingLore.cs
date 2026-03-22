using System;
using System.Collections.Generic;
using Dark.Scripts.Common.Lore;
using Dark.Tools.Language.Runtime;
using TMPro;
using UnityEngine;

namespace Dark.Scripts.SceneNavigation
{
    public class UILoadingLore : MonoBehaviour
    {
        [SerializeField] private Loading loading;
        [SerializeField] private TextMeshProUGUI txtTitle;
        [SerializeField] private TextMeshProUGUI txtLore;

        private void Awake()
        {
            // loading.onStartLoading += OnStartLoading;
        }

        private void OnEnable()
        {
            OnStartLoading();
        }

        private void OnStartLoading()
        {
            var info = LoreManifest.GetRandom();
            txtTitle.SetTextLanguage(info.nameKey);
            txtLore.SetTextLanguage(info.loreKey);
        }
    }
}