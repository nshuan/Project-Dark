using System.Collections.Generic;
using System.IO;
using Core;
using Dark.Scripts.OutGame.SaveSlot;
using InGame.EndlessLevel;
using UnityEngine;

namespace Data
{
    public class PlayerDataManager : Singleton<PlayerDataManager>
    {
        private const string DefaultDataKey = "game_playerDataSlot0";
        public static string CurrentDataKey = "";
        public static string DataKey => string.IsNullOrEmpty(CurrentDataKey) ? DefaultDataKey : CurrentDataKey;

        private Dictionary<string, PlayerData> preloadedData = new Dictionary<string, PlayerData>();
        
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
            PreloadAllData();
            Initialize();
        }
        
        public void Initialize()
        {
            if (preloadedData != null && preloadedData.ContainsKey(DataKey))
            {
                data = preloadedData[DataKey];
                return;
            }
            
            if (DataHandler.Exist<PlayerData>(DataKey))
            {
                data = DataHandler.Load<PlayerData>(DataKey);
            }
            else
            {
                data = new PlayerData();
            }
            
            preloadedData ??= new Dictionary<string, PlayerData>();
            preloadedData[DataKey] = data;
        }

        public void CompleteLevel()
        {
            data.level += 1;
            Save();
        }

        public void SetFlagCompletedAllLevel()
        {
            data.completed = true;
            LevelEndlessData.UnlockEndlessFeature();
            Save();
        }

        public void SetCompleteAllLevelTime(double timeCompletedMilli)
        {
            data.timeCompletedMilli = timeCompletedMilli;
            Save();
        }

        public void SetUploadedScoreBigLeaderboard(bool save)
        {
            data.uploadedScoreBigLeaderboard = true;
            if (save) Save();
        }

        public void SetUploadedScoreSmallLeaderboard(bool save)
        {
            data.uploadedScoreSmallLeaderboard = true;
            if (save) Save();
        }

        public void UpdateEndlessWave(int passedWave, bool save)
        {
            if (passedWave > data.endlessWavePassed)
            {
                data.endlessWavePassed = passedWave;
                if (save) Save();
            }
        }
        
        #region SAVE LOAD

        public void Save()
        {
            DataHandler.Save(DataKey, data);
            preloadedData[DataKey] = data;
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
            preloadedData[dataKey] = new PlayerData();
        }

        public void PreloadAllData()
        {
            preloadedData = new Dictionary<string, PlayerData>();
            foreach (var key in SaveSlotManager.SlotDataKeys)
            {
                preloadedData[key] = DataHandler.Load<PlayerData>(key, new PlayerData());
            }
        }
        
        #endregion
    }
}