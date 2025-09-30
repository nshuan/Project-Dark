using System;
using Core;
using Data;
using InGame.CharacterClass;

namespace Dark.Scripts.OutGame.SaveSlot
{
    public class SaveSlotManager : Singleton<SaveSlotManager>
    {
        #region Data

        private readonly string[] SlotDataKeys = new[]
        {
            "playerDataSlot0",
            "playerDataSlot1",
            "playerDataSlot2",
            "playerDataSlot3",
        };
        
        private PlayerData[] cacheData = new PlayerData[4];

        public void SelectSlot(int index)
        {
            index = Math.Clamp(index, 0, SlotDataKeys.Length - 1);
            PlayerDataManager.CurrentDataKey = SlotDataKeys[index];
            PlayerDataManager.Instance.Initialize();
        }

        private PlayerData GetSlotData(int slotIndex)
        {
            return DataHandler.Load<PlayerData>(SlotDataKeys[slotIndex]);
        }

        public void ClearSlot(int index)
        {
            if (index < 0 || index >= SlotDataKeys.Length) return;
            PlayerDataManager.Instance.ClearData(SlotDataKeys[index]);
        }

        #endregion

        #region Display Data

        public bool IsEmptySlot(int slotIndex)
        {
            return slotIndex < 0 || slotIndex >= SlotDataKeys.Length || !DataHandler.Exist<PlayerData>(SlotDataKeys[slotIndex]);
        }

        public int GetClassTypeIndex(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= SlotDataKeys.Length) return -1;
            if (IsEmptySlot(slotIndex)) return -1;
            
            cacheData[slotIndex] ??= GetSlotData(slotIndex);

            return cacheData[slotIndex].characterClass;
        }
        
        public string GetDisplayClassName(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= SlotDataKeys.Length) return "";
            if (IsEmptySlot(slotIndex)) return "";
            
            cacheData[slotIndex] ??= GetSlotData(slotIndex);

            return ((CharacterClass)(cacheData[slotIndex].characterClass)).ToString();
        }

        public string GetDisplayPassedDays(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= SlotDataKeys.Length) return "days:";
            if (IsEmptySlot(slotIndex)) return "days:";
            
            cacheData[slotIndex] ??= GetSlotData(slotIndex);
            
            return $"days: {cacheData[slotIndex].passedDay}";
        }

        public string GetDisplayLevel(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= SlotDataKeys.Length) return "level:";
            if (IsEmptySlot(slotIndex)) return "level:";
            
            cacheData[slotIndex] ??= GetSlotData(slotIndex);
            
            return $"level: {cacheData[slotIndex].level}";
        }

        public string GetDisplayTimePlayed(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= SlotDataKeys.Length) return "0 hours 0 min";
            if (IsEmptySlot(slotIndex)) return "0 hours 0 min";
            
            cacheData[slotIndex] ??= GetSlotData(slotIndex);

            var totalTime = TimeSpan.FromMilliseconds(cacheData[slotIndex].timePlayedMilli);
            return $"{(int)totalTime.TotalHours} hours {totalTime.Minutes} min";
        }

        #endregion
    }
}