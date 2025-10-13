using System.Collections.Generic;
using InGame;
using TMPro;
using UnityEngine;

namespace Dark.Scripts.RuntimeCheat
{
    public class UIRuntimeCheatBonusPassive : MonoBehaviour
    {
        public TMP_InputField[] inpBonusPassiveSize;
        public TMP_InputField[] inpBonusPassiveValue;
        public TMP_InputField[] inpBonusPassiveCooldown;
        public TMP_InputField[] inpBonusPassiveChance;
        public TMP_InputField[] inpBonusPassiveStagger;

        private void OnEnable()
        {
            foreach (var inp in inpBonusPassiveSize) inp.text = "0";
            foreach (var inp in inpBonusPassiveValue) inp.text = "0";
            foreach (var inp in inpBonusPassiveCooldown) inp.text = "0";
            foreach (var inp in inpBonusPassiveChance) inp.text = "0";
            foreach (var inp in inpBonusPassiveStagger) inp.text = "0";
            
            var bonusInfo = LevelUtility.BonusInfo;

            if (bonusInfo.passiveBonusSizeMapByType is { Count: > 0 })
            {
                for (var i = 0; i < inpBonusPassiveSize.Length; i++)
                {
                    if (bonusInfo.passiveBonusSizeMapByType.ContainsKey((PassiveType)i))
                        inpBonusPassiveSize[i].text = bonusInfo.passiveBonusSizeMapByType[(PassiveType)i].ToString();
                }
            }
            
            if (bonusInfo.passiveBonusValueMapByType is { Count: > 0 })
            {
                for (var i = 0; i < inpBonusPassiveValue.Length; i++)
                {
                    if (bonusInfo.passiveBonusValueMapByType.ContainsKey((PassiveType)i))
                        inpBonusPassiveValue[i].text = bonusInfo.passiveBonusValueMapByType[(PassiveType)i].ToString();
                }
            }
            
            if (bonusInfo.passiveBonusCooldownMapByType is { Count: > 0 })
            {
                for (var i = 0; i < inpBonusPassiveCooldown.Length; i++)
                {
                    if (bonusInfo.passiveBonusCooldownMapByType.ContainsKey((PassiveType)i))
                        inpBonusPassiveCooldown[i].text = bonusInfo.passiveBonusCooldownMapByType[(PassiveType)i].ToString();
                }
            }
            
            if (bonusInfo.passiveBonusChanceMapByType is { Count: > 0 })
            {
                for (var i = 0; i < inpBonusPassiveChance.Length; i++)
                {
                    if (bonusInfo.passiveBonusChanceMapByType.ContainsKey((PassiveType)i))
                        inpBonusPassiveChance[i].text = bonusInfo.passiveBonusChanceMapByType[(PassiveType)i].ToString();
                }
            }
            
            if (bonusInfo.passiveBonusStaggerMapByType is { Count: > 0 })
            {
                for (var i = 0; i < inpBonusPassiveStagger.Length; i++)
                {
                    if (bonusInfo.passiveBonusStaggerMapByType.ContainsKey((PassiveType)i))
                        inpBonusPassiveStagger[i].text = bonusInfo.passiveBonusStaggerMapByType[(PassiveType)i].ToString();
                }
            }
        }

        public void Save()
        {
            var bonusInfo = LevelUtility.BonusInfo;

            bonusInfo.passiveBonusSizeMapByType ??= new Dictionary<PassiveType, float>();
            for (var i = 0; i < inpBonusPassiveSize.Length; i++)
            {
                if (float.TryParse(inpBonusPassiveSize[i].text, out var valueFloat))
                    bonusInfo.passiveBonusSizeMapByType[(PassiveType)i] = valueFloat;
            }
            
            bonusInfo.passiveBonusValueMapByType ??= new Dictionary<PassiveType, float>();
            for (var i = 0; i < inpBonusPassiveValue.Length; i++)
            {
                if (float.TryParse(inpBonusPassiveValue[i].text, out var valueFloat))
                    bonusInfo.passiveBonusValueMapByType[(PassiveType)i] = valueFloat;
            }
            
            bonusInfo.passiveBonusCooldownMapByType ??= new Dictionary<PassiveType, float>();
            for (var i = 0; i < inpBonusPassiveCooldown.Length; i++)
            {
                if (float.TryParse(inpBonusPassiveCooldown[i].text, out var valueFloat))
                    bonusInfo.passiveBonusCooldownMapByType[(PassiveType)i] = valueFloat;
            }
            
            bonusInfo.passiveBonusChanceMapByType ??= new Dictionary<PassiveType, float>();
            for (var i = 0; i < inpBonusPassiveChance.Length; i++)
            {
                if (float.TryParse(inpBonusPassiveChance[i].text, out var valueFloat))
                    bonusInfo.passiveBonusChanceMapByType[(PassiveType)i] = valueFloat;
            }
            
            bonusInfo.passiveBonusStaggerMapByType ??= new Dictionary<PassiveType, float>();
            for (var i = 0; i < inpBonusPassiveStagger.Length; i++)
            {
                if (float.TryParse(inpBonusPassiveStagger[i].text, out var valueFloat))
                    bonusInfo.passiveBonusStaggerMapByType[(PassiveType)i] = valueFloat;
            }
            
            LevelUtility.BonusInfo = bonusInfo;
        }
    }
}