using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dark.Scripts.Settings.UI
{
    public class UISettingItemSlider : SerializedMonoBehaviour
    {
        [OdinSerialize, NonSerialized] private ISettingItemSlider _settingItemLogic;
        [SerializeField] private Slider slider;
        [SerializeField] private TextMeshProUGUI txtDisplayValue;

        private void Start()
        {
            _settingItemLogic.DisplayValue = txtDisplayValue;
            _settingItemLogic.Initialize(slider); 
        }
    }
}