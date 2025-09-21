using System;
using InGame;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dark.Scripts.RuntimeCheat
{
    public class UIRuntimeCheatBonusStats : MonoBehaviour
    {
        [Serializable]
        public class UIRuntimeCheatInfoInputField
        {
            public TextMeshProUGUI title;
            public TMP_InputField inputField;
        }

        public UIRuntimeCheatInfoInputField infoBonusDamePlus;
        public UIRuntimeCheatInfoInputField infoBonusDameMultiply;
        public UIRuntimeCheatInfoInputField infoBonusCritRatePlus;
        public UIRuntimeCheatInfoInputField infoBonusCritDame;
        public UIRuntimeCheatInfoInputField infoBonusCooldownPlus;
        public UIRuntimeCheatInfoInputField infoBonusHpPlus;
        public UIRuntimeCheatInfoInputField infoBonusHpMultiply;

        public void Save()
        {
            var bonusInfo = LevelUtility.BonusInfo;
            if (int.TryParse(infoBonusDamePlus.inputField.text, out var valueInt))
                bonusInfo.damePlus = valueInt;
            if (float.TryParse(infoBonusDameMultiply.inputField.text, out var valueFloat))
                bonusInfo.dameMultiply = valueFloat;
            if (float.TryParse(infoBonusCritRatePlus.inputField.text, out valueFloat))
                bonusInfo.criticalRatePlus = valueFloat;
            if (int.TryParse(infoBonusCritDame.inputField.text, out valueInt))
                bonusInfo.criticalDame = valueInt;
            if (float.TryParse(infoBonusCooldownPlus.inputField.text, out valueFloat))
                bonusInfo.cooldownPlus = valueFloat;
            if (int.TryParse(infoBonusHpPlus.inputField.text, out valueInt))
                bonusInfo.hpPlus = valueInt;
            if (float.TryParse(infoBonusHpMultiply.inputField.text, out valueFloat))
                bonusInfo.hpMultiply = valueFloat;
            
            LevelUtility.BonusInfo = bonusInfo;
        }
    }
}