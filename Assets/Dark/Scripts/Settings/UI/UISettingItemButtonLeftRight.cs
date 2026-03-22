using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dark.Scripts.Settings.UI
{
    public class UISettingItemButtonLeftRight : UISettingItem
    {
        [OdinSerialize, NonSerialized] private ISettingItemButtonLeftRight _settingItemLogic;
        [SerializeField] private Button buttonLeft;
        [SerializeField] private Button buttonRight;
        [SerializeField] private bool displayText = false;
        [SerializeField, ShowIf("displayText")] private TextMeshProUGUI txtDisplay;

        private void Start()
        {
            _settingItemLogic.DisplayText = txtDisplay;
            _settingItemLogic.Initialize(buttonLeft); 
            _settingItemLogic.InitializeButtonRight(buttonRight);
        }

        private void OnEnable()
        {
            _settingItemLogic.DisplayText = txtDisplay;
            _settingItemLogic.Initialize(buttonLeft);
            _settingItemLogic.InitializeButtonRight(buttonRight);
        }

        public override void Save()
        {
            _settingItemLogic.Save();
        }
    }
}