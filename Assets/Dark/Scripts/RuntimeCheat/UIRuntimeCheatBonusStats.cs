using System;
using System.Globalization;
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

        [Header("Bonus Stats")]
        public UIRuntimeCheatInfoInputField infoBonusDamePlus;
        public UIRuntimeCheatInfoInputField infoBonusDameMultiply;
        public UIRuntimeCheatInfoInputField infoBonusCritRatePlus;
        public UIRuntimeCheatInfoInputField infoBonusCritDame;
        public UIRuntimeCheatInfoInputField infoBonusCooldownPlus;
        public UIRuntimeCheatInfoInputField infoBonusHpPlus;
        public UIRuntimeCheatInfoInputField infoBonusHpMultiply;

        [Space] [Header("Bonus Skill")]
        public UIRuntimeCheatInfoInputField infoBonusSkillDamePlus;
        public UIRuntimeCheatInfoInputField infoBonusSkillDameMultiply;
        public UIRuntimeCheatInfoInputField infoBonusSkillCooldownPlus;
        public UIRuntimeCheatInfoInputField infoBonusSkillCooldownMultiply;
        public UIRuntimeCheatInfoInputField infoBonusSkillSizeMultiply;
        public UIRuntimeCheatInfoInputField infoBonusSkillRangeMultiply;
        public UIRuntimeCheatInfoInputField infoBonusSkillBulletPlus;
        public UIRuntimeCheatInfoInputField infoBonusSkillBulletMaxHitPlus;
        public UIRuntimeCheatInfoInputField infoBonusSkillStaggerMultiply;

        private void OnEnable()
        {
            var bonusInfo = LevelUtility.BonusInfo;
            
            infoBonusDamePlus.inputField.text = bonusInfo.damePlus.ToString();
            infoBonusDameMultiply.inputField.text = bonusInfo.dameMultiply.ToString(CultureInfo.InvariantCulture);
            infoBonusCritRatePlus.inputField.text = bonusInfo.criticalRatePlus.ToString(CultureInfo.InvariantCulture);
            infoBonusCritDame.inputField.text = bonusInfo.criticalDame.ToString();
            infoBonusCooldownPlus.inputField.text = bonusInfo.cooldownPlus.ToString(CultureInfo.InvariantCulture);
            infoBonusHpPlus.inputField.text = bonusInfo.hpPlus.ToString();
            infoBonusHpMultiply.inputField.text = bonusInfo.hpMultiply.ToString(CultureInfo.InvariantCulture);
            
            infoBonusSkillDamePlus.inputField.text = bonusInfo.skillBonus.skillDamePlus.ToString();
            infoBonusSkillDameMultiply.inputField.text = bonusInfo.skillBonus.skillDameMultiply.ToString(CultureInfo.InvariantCulture);
            infoBonusSkillCooldownPlus.inputField.text = bonusInfo.skillBonus.skillCooldownPlus.ToString(CultureInfo.InvariantCulture);
            infoBonusSkillCooldownMultiply.inputField.text = bonusInfo.skillBonus.skillCooldownMultiply.ToString(CultureInfo.InvariantCulture);
            infoBonusSkillSizeMultiply.inputField.text = bonusInfo.skillBonus.skillSizeMultiply.ToString(CultureInfo.InvariantCulture);
            infoBonusSkillRangeMultiply.inputField.text = bonusInfo.skillBonus.skillRangeMultiply.ToString(CultureInfo.InvariantCulture);
            infoBonusSkillBulletPlus.inputField.text = bonusInfo.skillBonus.bulletPlus.ToString();
            infoBonusSkillBulletMaxHitPlus.inputField.text = bonusInfo.skillBonus.bulletMaxHitPlus.ToString();
            infoBonusSkillStaggerMultiply.inputField.text = bonusInfo.skillBonus.staggerMultiply.ToString(CultureInfo.InvariantCulture);
        }

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
            
            if (int.TryParse(infoBonusSkillDamePlus.inputField.text, out valueInt))
                bonusInfo.skillBonus.skillDamePlus = valueInt;
            if (float.TryParse(infoBonusSkillDameMultiply.inputField.text, out valueFloat))
                bonusInfo.skillBonus.skillDameMultiply = valueFloat;
            if (float.TryParse(infoBonusSkillCooldownPlus.inputField.text, out valueFloat))
                bonusInfo.skillBonus.skillCooldownPlus = valueFloat;
            if (float.TryParse(infoBonusSkillCooldownMultiply.inputField.text, out valueFloat))
                bonusInfo.skillBonus.skillCooldownMultiply = valueFloat;
            if (float.TryParse(infoBonusSkillSizeMultiply.inputField.text, out valueFloat))
                bonusInfo.skillBonus.skillSizeMultiply = valueFloat;
            if (float.TryParse(infoBonusSkillRangeMultiply.inputField.text, out valueFloat))
                bonusInfo.skillBonus.skillRangeMultiply = valueFloat;
            if (int.TryParse(infoBonusSkillBulletPlus.inputField.text, out valueInt))
                bonusInfo.skillBonus.bulletPlus = valueInt;
            if (int.TryParse(infoBonusSkillBulletMaxHitPlus.inputField.text, out valueInt))
                bonusInfo.skillBonus.bulletMaxHitPlus = valueInt;
            if (float.TryParse(infoBonusSkillStaggerMultiply.inputField.text, out valueFloat))
                bonusInfo.skillBonus.staggerMultiply = valueFloat;
            
            LevelUtility.BonusInfo = bonusInfo;
        }
    }
}