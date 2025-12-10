using System;
using Core;
using Data;
using Economic;
using InGame.CharacterClass;
using InGame.Upgrade;

namespace Dark.Scripts.OutGame.SaveSlot
{
    public class SaveSlotManager : Singleton<SaveSlotManager>
    {
        #region Data

        private const string TotalDataCreatedKey = "totalDataSlotsCreated";
        
        private readonly string[] SlotDataKeys = new[]
        {
            "playerDataSlot0",
            "playerDataSlot1",
            "playerDataSlot2",
            "playerDataSlot3",
        };

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
            

            return ((CharacterClass)(GetSlotData(slotIndex).characterClass)).ToString();
        }

        public string GetDisplayPassedDays(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= SlotDataKeys.Length) return "days:";
            if (IsEmptySlot(slotIndex)) return "days:";
 
            return $"days: {GetSlotData(slotIndex).passedDay}";
        }

        public string GetDisplayLevel(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= SlotDataKeys.Length) return "level:";
            if (IsEmptySlot(slotIndex)) return "level:";
            
            return $"level: {GetSlotData(slotIndex).level + 1}";
        }

        public string GetDisplayTimePlayed(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= SlotDataKeys.Length) return "0 hours 0 min";
            if (IsEmptySlot(slotIndex)) return "0 hours 0 min";
     
            var totalTime = TimeSpan.FromMilliseconds(GetSlotData(slotIndex).timePlayedMilli);
            return $"{(int)totalTime.TotalHours} hours {totalTime.Minutes} min";
        }

        #endregion
    }
}