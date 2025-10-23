using System.Collections.Generic;
using System.IO;
using Core;
using UnityEngine;

namespace Data
{
    public class PlayerDataManager : Singleton<PlayerDataManager>
    {
        private const string DefaultDataKey = "playerDataSlot0";
        public static string CurrentDataKey = "";
        public static string DataKey => string.IsNullOrEmpty(CurrentDataKey) ? DefaultDataKey : CurrentDataKey;

        private PlayerData data;
        public PlayerData Data
        {
            get
            {
                if (data == null) Initialize();
                return data;
            }
        }
        
        public PlayerDataManager()
        {
            Initialize();
        }
        
        public void Initialize()
        {
            if (DataHandler.Exist<PlayerData>(DataKey))
            {
                data = DataHandler.Load<PlayerData>(DataKey);
            }
            else
            {
                data = new PlayerData();
            }
        }

        public void CompleteLevel()
        {
            data.level += 1;
            Save();
        }
        
        #region SAVE LOAD

        public void Save()
        {
            DataHandler.Save(DataKey, data);
        }

        public void Save(PlayerData newData)
        {
            data = newData;
            Save();
        }

        public void ClearData(string dataKey)
        {
            data = null;
            if (DataHandler.Exist<PlayerData>(dataKey))
                DataHandler.Clear(dataKey);
        }
        
        #endregion
    }
}