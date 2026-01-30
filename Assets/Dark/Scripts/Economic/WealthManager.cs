using System;
using Core;
using Data;

namespace Economic
{
    public class WealthManager : Singleton<WealthManager>
    {
        #region Grade

        private int grade;
        public int Grade => grade;

        public void UpGrade()
        {
            grade += 1;
            OnUpGrade?.Invoke(grade);
            AddLevelPoint(1);
        }

        #endregion
        
        #region Exp

        private int exp;

        public int Exp => exp;

        public void AddExp(int value)
        {
            exp += value;
            Save();
            OnExpChanged?.Invoke(exp - value, exp);
        }
        
        #endregion

        #region LevelPoint

        private int levelPoint;
        public int LevelPoint => levelPoint;

        public void AddLevelPoint(int value)
        {
            levelPoint += value;
            Save();
            OnLevelPointChanged?.Invoke(levelPoint - value, levelPoint);
        }

        public bool UseLevelPoint(int value)
        {
            if (value > levelPoint) return false;
            levelPoint -= value;
            Save();
            OnLevelPointChanged?.Invoke(levelPoint + value, levelPoint);
            return true;
        }

        #endregion

        #region Vestige

        private int vestige;
        public int Vestige => vestige;

        public void AddVestige(int value)
        {
            vestige += value;
            Save();
            OnVestigeChanged?.Invoke(vestige - value, vestige);
        }

        public bool UseVestige(int value)
        {
            if (value > vestige) return false;
            vestige -= value;
            Save();
            OnVestigeChanged?.Invoke(vestige + value, vestige);
            return true;
        }

        #endregion

        #region BossPoint

        private int bossPoint;
        public int BossPoint => bossPoint;

        public void AddBossPoint(int value)
        {
            bossPoint += value;
            Save();
            OnBossPointChanged?.Invoke(bossPoint - value, bossPoint);
        }

        public bool UseBossPoint(int value)
        {
            if (value > bossPoint) return false;
            bossPoint -= value;
            Save();
            OnBossPointChanged?.Invoke(bossPoint + value, bossPoint);
            return true;
        }
    
        #endregion
        
        #region Actions

        public Action<int> OnUpGrade { get; set; } // <CurrentLevel>
        public Action<int, int> OnExpChanged { get; set; } // <BeforeChange, AfterChange>
        public Action<int, int> OnLevelPointChanged { get; set; }
        public Action<int, int> OnVestigeChanged { get; set; }
        public Action<int, int> OnBossPointChanged { get; set; }
        
        #endregion

        public WealthManager()
        {
            Initialize();    
        }
        
        public void Initialize()
        {
            var data = PlayerDataManager.Instance.Data;
            grade = data.grade;
            exp = data.exp;
            levelPoint = data.levelPoint;
            vestige = data.dark;
            bossPoint = data.bossPoint;
            hasShowInstructionVestige = data.hasShowInstructionVestige;
            hasShowInstructionEchoes = data.hasShowInstructionEchoes;
            hasShowInstructionSigils = data.hasShowInstructionSigils;
        }

        public void Save()
        {
            var data = PlayerDataManager.Instance.Data;
            var changedDark = vestige - data.dark;
            var changedBossPoint = bossPoint - data.bossPoint;
            var changedLevelPoint = levelPoint - data.levelPoint;
            data.grade = grade;
            data.exp = exp;
            data.levelPoint = levelPoint;
            data.dark = vestige;
            data.bossPoint = bossPoint;
            data.totalDarkClaimed += Math.Max(changedDark, 0);
            data.totalBossPointClaimed += Math.Max(changedBossPoint, 0);
            data.totalLevelPointClaimed += Math.Max(changedLevelPoint, 0);
            data.hasShowInstructionVestige = hasShowInstructionVestige;
            data.hasShowInstructionEchoes = hasShowInstructionEchoes;
            data.hasShowInstructionSigils = hasShowInstructionSigils;
            
            PlayerDataManager.Instance.Save(data);
        }

        public bool CanSpend(WealthType type, int amount)
        {
            switch (type)
            {
                case WealthType.Vestige:
                    return Vestige >= amount;
                case WealthType.Echoes:
                    return LevelPoint >= amount;
                case WealthType.Sigils:
                    return BossPoint >= amount;
                default:
                    return false;
            }    
        }
        
        public void Spend(WealthType type, int amount)
        {
            switch (type)
            {
                case WealthType.Vestige:
                    UseVestige(amount);
                    break;
                case WealthType.Echoes:
                    UseLevelPoint(amount);
                    break;
                case WealthType.Sigils:
                    UseBossPoint(amount);
                    break;
            }    
        }
        
        #region Resource Instruction

        public bool hasShowInstructionVestige;
        public bool hasShowInstructionEchoes;
        public bool hasShowInstructionSigils;
        
        public void SetShownInstruction(WealthType type)
        {
            switch (type)
            {
                case WealthType.Vestige:
                    hasShowInstructionVestige = true;
                    break;
                case WealthType.Echoes:
                    hasShowInstructionEchoes = true;
                    break;
                case WealthType.Sigils:
                    hasShowInstructionSigils = true;
                    break;
            }
            
            Save();
        }

        #endregion
    }

    public enum WealthType
    {
        Vestige, // Dark
        Echoes, // Level Point
        Sigils, // Boss Point
        Exp
    }
}