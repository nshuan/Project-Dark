using System.Collections.Generic;
using System.IO;
using Core;
using UnityEngine;

namespace Data
{
    public class PlayerDataManager : Singleton<PlayerDataManager>
    {
        private const string SavePath = "/playerData.json";
        
        private PlayerData data;
        public PlayerData Data => data;
        
        public PlayerDataManager()
        {
            Initialize();
        }
        
        public void Initialize()
        {
            data = DataHandler.Load<PlayerData>(SavePath) ?? new PlayerData();
        }
        
        #region SAVE LOAD

        public void Save()
        {
            DataHandler.Save(SavePath, data);    
        }

        public void Save(PlayerData newData)
        {
            data = newData;
            Save();
        }
        
        #endregion
    }
}