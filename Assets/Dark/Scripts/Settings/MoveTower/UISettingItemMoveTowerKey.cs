using Dark.Scripts.Settings.UI;
using Dark.Tools.Language.Runtime;
using TMPro;
using UnityEngine;

namespace Dark.Scripts.Settings.MoveTower
{
    public class UISettingItemMoveTowerKey : UISettingItemKeyListener
    {
        [SerializeField] private TextMeshProUGUI txtTitle;

        private void OnDestroy()
        {
            LanguageManager.Instance.UnregisterForceUpdate(UpdateTitle);
        }
        
        protected override void Start()
        {
            base.Start();

            UpdateTitle();
            
            LanguageManager.Instance.RegisterForceUpdate(UpdateTitle);
        }

        private void UpdateTitle()
        {
            if (_settingItemLogic is SettingMoveTowerKeyListener moveTowerKeyLogic)
            {
                txtTitle.SetTextLanguage("key_settings_move_tower", ("%{value}", (moveTowerKeyLogic.keyIndex + 1).ToString()));
            }
        }
    }
}