using Dark.Scripts.Settings.UI;
using Dark.Tools.Language.Runtime;
using TMPro;
using UnityEngine;

namespace Dark.Scripts.Settings.MoveTower
{
    public class UISettingItemMoveTowerKey : UISettingItemKeyListener
    {
        [SerializeField] private TextMeshProUGUI txtTitle;

        protected override void Start()
        {
            base.Start();

            if (_settingItemLogic is SettingMoveTowerKeyListener moveTowerKeyLogic)
            {
                txtTitle.SetTextLanguage("key_settings_move_tower", ("%{value}", (moveTowerKeyLogic.keyIndex + 1).ToString()));
            }
        }
    }
}