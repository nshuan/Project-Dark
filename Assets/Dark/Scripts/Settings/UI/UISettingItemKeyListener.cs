using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dark.Scripts.Settings.UI
{
    public class UISettingItemKeyListener : UISettingItem
    {
        [OdinSerialize, NonSerialized] public ISettingItemKeyListener _settingItemLogic;
        [SerializeField] public Button button;
        [SerializeField] private bool displayText = false;
        [SerializeField, ShowIf("displayText")] private TextMeshProUGUI txtDisplay;

        protected virtual void Start()
        {
            _settingItemLogic.DisplayText = txtDisplay;
            _settingItemLogic.Initialize(button); 
        }

        private void OnEnable()
        {
            _settingItemLogic.DisplayText = txtDisplay;
            _settingItemLogic.Initialize(button); 
        }

        private void OnDisable()
        {
            _settingItemLogic?.UpdateKey(KeyCode.Escape);
        }

        private void Update()
        {
            if (Input.anyKeyDown)
            {
                if (!_settingItemLogic.IsSelecting) return;
                foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(key))
                    {
                        _settingItemLogic.UpdateKey(key);
                        break;
                    }
                }
            }
        }

        public override void Save()
        {
            _settingItemLogic.Save();
        }

        public void ClearKey()
        {
            _settingItemLogic.UpdateKey(KeyCode.None);
        }

        public void Deselect()
        {
            _settingItemLogic.UpdateKey(KeyCode.Escape);
        }
    }
}