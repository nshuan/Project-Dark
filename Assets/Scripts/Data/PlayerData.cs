using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace Data
{
    [Serializable]
    public class PlayerData
    {
        public int level;
        
        // Resources
        public int grade;
        public int exp;
        public int levelPoint;
        public int dark;
        public int bossPoint;

        public PlayerData()
        {
            level = 1;
            grade = 1;
        }
    }
}