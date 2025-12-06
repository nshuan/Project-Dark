using System;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.UI;

namespace Dark.Scripts.Settings.UI
{
    public class UISettingItemSlider : MonoBehaviour
    {
        [OdinSerialize, NonSerialized] private ISettingItemSlider _settingItemLogic;
        [SerializeField] private Slider slider;

        private void Start()
        {
            _settingItemLogic.Initialize(slider); 
        }
    }
}