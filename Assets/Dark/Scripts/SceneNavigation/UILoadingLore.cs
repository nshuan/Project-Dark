using System;
using System.Collections.Generic;
using Dark.Scripts.Common.Lore;
using TMPro;
using UnityEngine;

namespace Dark.Scripts.SceneNavigation
{
    public class UILoadingLore : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI txtLore;

        private void Start()
        {
            txtLore.SetText(LoreManifest.GetRandom());
        }
    }
}