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
            OnExpChanged?.Invoke(exp - value, exp);
        }
        
        #endregion

        #region LevelPoint

        private int levelPoint;
        public int LevelPoint => levelPoint;

        public void AddLevelPoint(int value)
        {
            levelPoint += value;
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

        #region Dark

        private int dark;
        public int Dark => dark;

        public void AddDark(int value)
        {
            dark += value;
            OnDarkChanged?.Invoke(dark - value, dark);
        }

        public bool UseDark(int value)
        {
            if (value > dark) return false;
            dark -= value;
            Save();
            OnDarkChanged?.Invoke(dark + value, dark);
            return true;
        }

        #endregion

        #region BossPoint

        private int bossPoint;
        public int BossPoint => bossPoint;

        public void AddBossPoint(int value)
        {
            bossPoint += value;
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
        public Action<int, int> OnDarkChanged { get; set; }
        public Action<int, int> OnBossPointChanged { get; set; }
        
        #endregion

        public WealthManager()
        {
            Initialize();    
        }
        
        private void Initialize()
        {
            var data = PlayerDataManager.Instance.Data;
            grade = data.grade;
            exp = data.exp;
            levelPoint = data.levelPoint;
            dark = data.dark;
            bossPoint = data.bossPoint;
        }

        private void Save()
        {
            var data = PlayerDataManager.Instance.Data;
            data.grade = grade;
            data.exp = exp;
            data.levelPoint = levelPoint;
            data.dark = dark;
            data.bossPoint = bossPoint;
            
            PlayerDataManager.Instance.Save(data);
        }
    }

    public enum WealthType
    {
        Vestige,
        Echoes,
        Sigils
    }
}