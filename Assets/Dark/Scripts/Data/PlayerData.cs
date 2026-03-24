using System;
using System.Collections.Generic;
using InGame.CharacterClass;
using UnityEngine.Serialization;

namespace Data
{
    [Serializable]
    public class PlayerData
    {
        public bool initialized;
        public bool completed;
        public int level;
        
        // Class
        public int characterClass;
        
        // Resources
        public int grade;
        public int exp;
        public int levelPoint;
        public int dark;
        public int bossPoint;
        public int totalDarkClaimed;
        public int totalBossPointClaimed;
        public int totalLevelPointClaimed;
        public int resetPoint;
        
        // Record
        public int passedDay;
        public double timePlayedMilli;
        public double timeCompletedMilli;
        public bool uploadedScoreBigLeaderboard;
        public bool uploadedScoreSmallLeaderboard;
        
        // Instruction
        public bool hasShowInstructionVestige;
        public bool hasShowInstructionEchoes;
        public bool hasShowInstructionSigils;
        
        public PlayerData()
        {
            level = 0;
            grade = 1;
            passedDay = 1;
            timePlayedMilli = 0;
        }

        public CharacterClass Class => (CharacterClass)characterClass;
    }
}