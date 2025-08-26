using System.Collections.Generic;
using System.IO;
using Core;
using UnityEngine;

namespace Data
{
    public class PlayerDataManager : Singleton<PlayerDataManager>
    {
        private const string DefaultDataKey = "playerData";
        public static string CurrentDataKey = "";
        private static string DataKey => string.IsNullOrEmpty(CurrentDataKey) ? DefaultDataKey : CurrentDataKey;

        private PlayerData data;
        public PlayerData Data => data;
        
        public PlayerDataManager()
        {
            Initialize();
        }
        
        public void Initialize()
        {
            data = DataHandler.Load<PlayerData>(DataKey) ?? new PlayerData();
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
        
        #endregion
    }
}