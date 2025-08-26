using System;
using Core;
using Data;
using InGame.CharacterClass;

namespace Dark.Scripts.OutGame.SaveSlot
{
    public class SaveSlotManager : MonoSingleton<SaveSlotManager>
    {
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
        }

        private PlayerData GetSlotData(int slotIndex)
        {
            return DataHandler.Load<PlayerData>(SlotDataKeys[slotIndex]);
        }

        #region Display Data

        public bool IsEmptySlot(int slotIndex)
        {
            return slotIndex < 0 || slotIndex >= SlotDataKeys.Length || !DataHandler.Exist<PlayerData>(SlotDataKeys[slotIndex]);
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
            if (slotIndex < 0 || slotIndex >= SlotDataKeys.Length) return "";
            if (IsEmptySlot(slotIndex)) return "";
            
            cacheData[slotIndex] ??= GetSlotData(slotIndex);
            
            return cacheData[slotIndex].passedDay.ToString();
        }

        public string GetDisplayLevel(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= SlotDataKeys.Length) return "";
            if (IsEmptySlot(slotIndex)) return "";
            
            cacheData[slotIndex] ??= GetSlotData(slotIndex);
            
            return cacheData[slotIndex].level.ToString();
        }

        public string GetDisplayTimePlayed(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= SlotDataKeys.Length) return "";
            if (IsEmptySlot(slotIndex)) return "";
            
            cacheData[slotIndex] ??= GetSlotData(slotIndex);
            
            return TimeSpan.FromMilliseconds(cacheData[slotIndex].timePlayedMilli).ToString(@"hh\:mm\:ss");
        }

        #endregion
    }
}