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
        [SerializeField] private TextMeshProUGUI txtLore;

        private void Awake()
        {
            loading.onStartLoading += OnStartLoading;
        }

        private void Start()
        {
            txtLore.SetText(LoreManifest.GetRandom());
        }

        private void OnStartLoading()
        {
            txtLore.SetText(LoreManifest.GetRandom());
        }
    }
}