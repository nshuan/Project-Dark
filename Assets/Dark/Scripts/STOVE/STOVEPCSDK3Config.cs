namespace Dark.Scripts.STOVE
{
    using System;
    using UnityEngine;

    [Serializable]
    public sealed class STOVEPCSDK3Config
    {
        private const string ResourcePath = "STOVEPCSDK3Config";

        public bool autoInitialize = true;
        public bool disableInEditor = true;
        public bool enforceLauncher = true;
        public string environment = "LIVE";
        public string gameId = "GM-2A6F-6A54B6C1_IND";
        public string applicationKey = "d5f8912403522362f29cd17427b57da47cb810b5309df82fd37b4a2f269d3c74";
        public int launcherCheckTimeoutMilliseconds = 60000;
        public float callbackIntervalSeconds = 0.1f;
        public bool initializeOwnership = true;
        public bool queryOwnershipOnInitialize = true;
        public bool initializeGameSupport = true;

        public static STOVEPCSDK3Config Load()
        {
            var textAsset = Resources.Load<TextAsset>(ResourcePath);

            if (textAsset == null || string.IsNullOrWhiteSpace(textAsset.text))
                return Default();

            try
            {
                return JsonUtility.FromJson<STOVEPCSDK3Config>(textAsset.text) ?? Default();
            }
            catch (Exception exception)
            {
                Debug.LogError($"Failed to load STOVE PC SDK config from Resources/{ResourcePath}.json: {exception}");
                return Default();
            }
        }

        public static STOVEPCSDK3Config Default()
        {
            return new STOVEPCSDK3Config();
        }
    }
}
