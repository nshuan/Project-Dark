using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using UnityEngine;

namespace Dark.Scripts.OutGame.Settings
{
    public class UISettingIconBoss : MonoBehaviour
    {
        private static string KeyBossUnlocked = "key_unlocked_bosses";
        public static void SetBossUnlocked(int bossId)
        {
            var unlockedData = DataHandler.Load<WrappedBossUnlockedData>(KeyBossUnlocked, new WrappedBossUnlockedData());
            if (!unlockedData.unlockedBoss.Contains(bossId))
            {
                unlockedData.unlockedBoss.Add(bossId);
                DataHandler.Save(KeyBossUnlocked, unlockedData);
            }
        }

        public static bool IsBossUnlocked(int bossId)
        {
            var unlockedData = DataHandler.Load<WrappedBossUnlockedData>(KeyBossUnlocked, new WrappedBossUnlockedData());
            return unlockedData.unlockedBoss.Contains(bossId);
        }
        
        [Serializable]
        public class WrappedBossUnlockedData
        {
            public List<int> unlockedBoss = new();
        }

        [SerializeField] private GameObject imgBossIcon;
        [SerializeField] private GameObject imgLock;
        [SerializeField] private int bossId;

        private void OnEnable()
        {
            var bossUnlocked = IsBossUnlocked(bossId);
            imgBossIcon.SetActive(bossUnlocked);
            imgLock.SetActive(!bossUnlocked);
        }
    }
}