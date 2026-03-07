using System;
using UnityEngine;

namespace Dark.Scripts.Settings.UI
{
    public class UISettingKeyListenerManager : MonoBehaviour
    {
        private static UISettingItemKeyListener[] keyListeners;
        
        private void Awake()
        {
            keyListeners = GetComponentsInChildren<UISettingItemKeyListener>();    
        }

        public static void OnKeyListenerSelected(ISettingItemKeyListener keyListenerLogic)
        {
            foreach (var listener in keyListeners)
            {
                if (listener._settingItemLogic == keyListenerLogic) continue;
                listener.Deselect();
            }
        }

        public static void OnKeyListenerUpdated(ISettingItemKeyListener keyListenerLogic, KeyCode updatedValue)
        {
            foreach (var listener in keyListeners)
            {
                if (listener._settingItemLogic == keyListenerLogic) continue;
                if (!listener._settingItemLogic.CompareKey(updatedValue)) continue;
                listener.ClearKey();
            }
        }
    }
}