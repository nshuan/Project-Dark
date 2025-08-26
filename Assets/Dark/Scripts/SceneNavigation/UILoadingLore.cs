using System;
using System.Collections.Generic;
using Dark.Scripts.Common.Lore;
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
            loading.onStartLoading += OnStartLoading;
        }

        private void Start()
        {
            OnStartLoading();
        }

        private void OnStartLoading()
        {
            var info = LoreManifest.GetRandom();
            txtTitle.SetText(info.name);
            txtLore.SetText(info.lore);
        }
    }
}