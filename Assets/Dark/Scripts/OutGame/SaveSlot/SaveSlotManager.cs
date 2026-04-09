using System;
using Core;
using Dark.Scripts.Tutorial;
using Dark.Tools.Language.Runtime;
using Data;
using Economic;
using InGame;
using InGame.CharacterClass;
using InGame.Upgrade;

namespace Dark.Scripts.OutGame.SaveSlot
{
    public class SaveSlotManager : Singleton<SaveSlotManager>
    {
        #region Data

        private const string TotalDataCreatedKey = "game_totalDataSlotsCreated";
        
        public static string[] SlotDataKeys = new[]
        {
            "game_playerDataSlot0",
            "game_playerDataSlot1",
            "game_playerDataSlot2",
            "game_playerDataSlot3",
        };

        public int CurrentSlotIndex = 0;
        public void SelectSlot(int index)
        {
            index = Math.Clamp(index, 0, SlotDataKeys.Length - 1);
            PlayerDataManager.CurrentDataKey = SlotDataKeys[index];
            PlayerDataManager.Instance.Initialize();
            WealthManager.Instance.Initialize();
            UpgradeManager.Instance.InitData();
        }

        private PlayerData GetSlotData(int slotIndex)
        {
            return DataHandler.Load<PlayerData>(SlotDataKeys[slotIndex]);
        }

        public void ClearSlot(int index)
        {
            if (index < 0 || index >= SlotDataKeys.Length) return;
            PlayerDataManager.Instance.ClearData(SlotDataKeys[index]);
            UpgradeManager.Instance.ClearData(UpgradeManager.GetDataKey(SlotDataKeys[index]));
            TutorialManager.Instance.ClearData(SlotDataKeys[index]);
        }
        
        public int GetTotalDataCreated()
        {
            return DataHandler.Load<int>(TotalDataCreatedKey, 0);
        }

        public void SaveTotalDataCreated(int count)
        {
            DataHandler.Save(TotalDataCreatedKey, count);
        }

        #endregion

        #region Display Data

        public bool IsEmptySlot(int slotIndex)
        {
            return slotIndex < 0 || slotIndex >= SlotDataKeys.Length || !DataHandler.Exist<PlayerData>(SlotDataKeys[slotIndex]);
        }

        public bool IsCompletedSlot(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= SlotDataKeys.Length ||
                !DataHandler.Exist<PlayerData>(SlotDataKeys[slotIndex])) return false;
            var data = DataHandler.Load<PlayerData>(SlotDataKeys[slotIndex]);
            return data.completed;
        }

        public int GetClassTypeIndex(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= SlotDataKeys.Length) return -1;
            if (IsEmptySlot(slotIndex)) return -1;

            return GetSlotData(slotIndex).characterClass;
        }
        
        public string GetDisplayClassName(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= SlotDataKeys.Length) return "";
            if (IsEmptySlot(slotIndex)) return "";

            var classType = (CharacterClass)(GetSlotData(slotIndex).characterClass);
            if (classType == CharacterClass.Archer)
            {
                var result = LanguageData.Instance.GetLocalizedString("key_node_the_sightsunder", LanguageManager.Instance.CurrentLanguage);
                if (result.Substring(0, 4).ToLower() == "the ")
                    result = result[4..];
                return result;
            }
            else if (classType == CharacterClass.Knight)
            {
                var result = LanguageData.Instance.GetLocalizedString("key_node_the_vergebrand", LanguageManager.Instance.CurrentLanguage);
                if (result.Substring(0, 4).ToLower() == "the ")
                    result = result[4..];
                return result;
            }
            
            return "";
        }

        public string GetDisplayPassedDays(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= SlotDataKeys.Length) return "days:";
            if (IsEmptySlot(slotIndex)) return "days:";

            return LanguageData.Instance
                .GetLocalizedString("key_save_slot_days", LanguageManager.Instance.CurrentLanguage)
                .Replace("%{value}", GetSlotData(slotIndex).passedDay.ToString());
        }

        public string GetDisplayLevel(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= SlotDataKeys.Length) return "level:";
            if (IsEmptySlot(slotIndex)) return "level:";

            var slotData = GetSlotData(slotIndex);
            var displayLevel = slotData.level + 1;
            if (displayLevel > LevelManifest.Instance.GetMaxLevel(slotData.Class)) displayLevel = slotData.level;
            return LanguageData.Instance
                .GetLocalizedString("key_save_slot_level", LanguageManager.Instance.CurrentLanguage)
                .Replace("%{value}", displayLevel.ToString());
        }

        public bool IsSlotCompleted(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= SlotDataKeys.Length) return false;
            if (IsEmptySlot(slotIndex)) return false;
            
            var slotData = GetSlotData(slotIndex);
            return slotData.level + 1 > LevelManifest.Instance.GetMaxLevel(slotData.Class);
        }

        public string GetDisplayTimePlayed(int slotIndex)
        {
            var result =
                LanguageData.Instance.GetLocalizedString("key_save_slot_time",
                    LanguageManager.Instance.CurrentLanguage);
            if (slotIndex < 0 || slotIndex >= SlotDataKeys.Length) return result.Replace("%{value1}", "0").Replace("%{value2}", "0");
            if (IsEmptySlot(slotIndex)) return result.Replace("%{value1}", "0").Replace("%{value2}", "0");
     
            var totalTime = TimeSpan.FromMilliseconds(GetSlotData(slotIndex).timePlayedMilli);
            return result.Replace("%{value1}", ((int)totalTime.TotalHours).ToString()).Replace("%{value2}", totalTime.Minutes.ToString());
        }
        
        public string GetDisplayTimeCompleted(int slotIndex)
        {
            var result =
                LanguageData.Instance.GetLocalizedString("key_save_slot_time",
                    LanguageManager.Instance.CurrentLanguage);
            if (slotIndex < 0 || slotIndex >= SlotDataKeys.Length) return result.Replace("%{value1}", "0").Replace("%{value2}", "0");
            if (IsEmptySlot(slotIndex)) return result.Replace("%{value1}", "0").Replace("%{value2}", "0");
     
            var totalTime = TimeSpan.FromMilliseconds(GetSlotData(slotIndex).timeCompletedMilli);
            return result.Replace("%{value1}", ((int)totalTime.TotalHours).ToString()).Replace("%{value2}", totalTime.Minutes.ToString());
        }

        #endregion
    }
}