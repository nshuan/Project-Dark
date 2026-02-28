using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dark.Scripts.Settings.UI
{
    public interface ISettingItemKeyListener : ISettingItemLogic<Button>
    {
        Button Button { get; set; }
        TextMeshProUGUI DisplayText { get; set; }
        new void Initialize(Button button)
        {
            Button = button;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClick);
        }

        void OnClick();
        void UpdateKey(KeyCode keyCode);
        bool IsSelecting { get; }
        bool CompareKey(KeyCode keyCode);
    }
    
    [Serializable]
    public class SettingMoveTowerKeyListener : ISettingItemKeyListener
    {
        [Range(0, 3), SerializeField] public int keyIndex;
        
        private KeyCode currentKey;
        private bool selecting;

        public void Initialize(Button button)
        {
            currentKey = keyIndex switch
            {
                0 => GameSettings.KeyMoveTower0,
                1 => GameSettings.KeyMoveTower1,
                2 => GameSettings.KeyMoveTower2,
                3 => GameSettings.KeyMoveTower3,
                _ => KeyCode.None
            };
            
            UpdateValue(true);
            selecting = false;
            
            Button = button;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClick);
        }

        public void Save()
        {
            switch (keyIndex)
            {
                case 0:
                    GameSettings.KeyMoveTower0 = currentKey;
                    break;
                case 1:
                    GameSettings.KeyMoveTower1 = currentKey;
                    break;
                case 2:
                    GameSettings.KeyMoveTower2 = currentKey;
                    break;
                case 3:
                    GameSettings.KeyMoveTower3 = currentKey;
                    break;
            }
            
            GameSettings.Save();
        }

        public void UpdateValue(bool onEnable)
        {
            if (selecting)
            {
                DisplayText.SetText("...");
                return;
            }

            if (currentKey is >= KeyCode.Alpha0 and <= KeyCode.Alpha9)
            {
                DisplayText.SetText(((int)currentKey - KeyCode.Alpha0).ToString());
                return;
            }

            DisplayText.SetText(currentKey.ToString());
        }

        public Button Button { get; set; }
        public TextMeshProUGUI DisplayText { get; set; }
        public void OnClick()
        {
            selecting = !selecting;
            UpdateValue(false);
            UISettingKeyListenerManager.OnKeyListenerSelected(this);
        }

        public bool IsSelecting => selecting;
        public bool CompareKey(KeyCode keyCode)
        {
            return currentKey == keyCode;
        }

        public void UpdateKey(KeyCode keyCode)
        {
            // == None thì luôn update
            if (keyCode == KeyCode.None)
            {
                currentKey = KeyCode.None;
                UpdateValue(false);
                return;
            }
            
            if (!selecting) return;
            if (!IsValidKey(keyCode)) return;
            if (keyCode == KeyCode.Escape)
            {
                selecting = false;
                UpdateValue(false);
                return;
            }

            selecting = false;
            currentKey = keyCode;
            UISettingKeyListenerManager.OnKeyListenerUpdated(this, currentKey);
            UpdateValue(false);
        }

        public bool IsValidKey(KeyCode keyCode)
        {
            if (keyCode == KeyCode.None) return true;
            if (keyCode is KeyCode.Escape) return true;
            if (keyCode is >= KeyCode.Alpha0 and <= KeyCode.Alpha9) return true;
            if (keyCode is >= KeyCode.A and <= KeyCode.Z) return true;
            if (keyCode is >= KeyCode.Keypad0 and <= KeyCode.Keypad9) return true;
            if (keyCode is KeyCode.UpArrow or KeyCode.DownArrow or KeyCode.RightArrow or KeyCode.LeftArrow) return true;
            if (keyCode is KeyCode.LeftShift or KeyCode.RightShift) return true;
            if (keyCode is KeyCode.LeftControl or KeyCode.RightControl) return true;
            
            return false;
        }
    }
}