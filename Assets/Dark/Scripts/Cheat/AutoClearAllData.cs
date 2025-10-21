using System;
using Dark.Scripts.OutGame.SaveSlot;
using Data;
using UnityEngine;

namespace Cheat
{
    public class AutoClearAllData : MonoBehaviour
    {
        private const int DataVersion = 1;
        private const string Key = "DataVersion";

        private void Awake()
        {
            var cachedVersion = 0;
            if (DataHandler.Exist<int>(Key))
            {
                cachedVersion = DataHandler.Load<int>(Key);
            }

            if (cachedVersion < DataVersion)
            {
                SaveSlotManager.Instance.ClearSlot(0);
                SaveSlotManager.Instance.ClearSlot(1);
                SaveSlotManager.Instance.ClearSlot(2);
                SaveSlotManager.Instance.ClearSlot(3);
                
                DataHandler.Save(Key, DataVersion);
            }
        }
    }
}