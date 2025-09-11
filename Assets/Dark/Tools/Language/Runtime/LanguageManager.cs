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
            CurrentLanguage = LanguageType.english;
        }

        #endregion
    }
}