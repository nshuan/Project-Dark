using System;
using Data;
using UnityEngine;

namespace Dark.Tools.Language.Runtime
{
    public class LanguageManager
    {
        public LanguageType CurrentLanguage { get; private set; }

        #region INIT

        private static LanguageManager instance;

        public static LanguageManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new LanguageManager();
                return instance;
            }
        }

        private LanguageManager()
        {
            Load();
            CurrentLanguage = settings.defaultLanguage;
        }

        #endregion

        #region Data

        private static string KeyLanguageSettings = "language_settings";

        private LanguageSettings settings;
        
        private void Load()
        {
            settings = DataHandler.Load<LanguageSettings>(KeyLanguageSettings, new LanguageSettings());
        }

        private void Save()
        {
            DataHandler.Save<LanguageSettings>(KeyLanguageSettings, settings);
        }

        #endregion
        
        public void UpdateDefaultLanguage(LanguageType language)
        {
            CurrentLanguage = language;
            settings.defaultLanguage = language;
            Save();
        }
    }

    [Serializable]
    public class LanguageSettings
    {
        public LanguageType defaultLanguage;
    }
}