using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace Data
{
    [Serializable]
    public class PlayerData
    {
        public bool initialized;
        public int level;
        
        // Class
        public int characterClass;
        
        // Resources
        public int grade;
        public int exp;
        public int levelPoint;
        public int dark;
        public int bossPoint;
        
        // Record
        public int passedDay;
        public double timePlayedMilli;
        
        public PlayerData()
        {
            level = 1;
            grade = 1;
        }
        
    }
}