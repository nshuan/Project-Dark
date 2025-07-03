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

#if UNITY_EDITOR
        public PlayerDataManager()
        {
            Initialize();
        }
#endif
        
        public void Initialize()
        {
            data = DataHandler.Load<PlayerData>(SavePath) ?? new PlayerData();
        }

        #region Methods

        public List<int> GetUnlockedUpgradeNodes()
        {
            return data.unlockedUpgradeNodes;
        }

        #endregion
        
        #region SAVE LOAD

        public void Save()
        {
            DataHandler.Save(SavePath, data);    
        }
        
        #endregion
    }
}