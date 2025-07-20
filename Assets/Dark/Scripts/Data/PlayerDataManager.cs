using System.Collections.Generic;
using System.IO;
using Core;
using UnityEngine;

namespace Data
{
    public class PlayerDataManager : Singleton<PlayerDataManager>
    {
        private const string DataKey = "playerData";
        
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