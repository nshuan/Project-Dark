using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dark.Scripts.Settings.UI
{
    public class UISettingItemButton : UISettingItem
    {
        [OdinSerialize, NonSerialized] private ISettingItemButton _settingItemLogic;
        [SerializeField] private Button button;
        [SerializeField] private bool displayText = false;
        [SerializeField, ShowIf("displayText")] private TextMeshProUGUI txtDisplay;

        private void Start()
        {
            _settingItemLogic.DisplayText = txtDisplay;
            _settingItemLogic.Initialize(button); 
        }

        private void OnEnable()
        {
            _settingItemLogic.DisplayText = txtDisplay;
            _settingItemLogic.Initialize(button); 
        }

        public override void Save()
        {
            _settingItemLogic.Save();
        }
    }
}