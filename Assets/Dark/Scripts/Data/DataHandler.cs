using System.IO;
using UnityEngine;

namespace Data
{
    public class DataHandler
    {
        private static string DataPath
        {
            get
            {
#if UNITY_EDITOR
                return Application.dataPath + "/_DataTest";
#endif
                return Application.persistentDataPath;
            }
        }
        public static bool Exist<T>(string key)
        {
            string filePath = DataPath + "/" + key + ".json";
            return File.Exists(filePath);
        }
        
        public static void Save<T>(string key, T data)
        {
            string filePath = DataPath + "/" + key + ".json";
            var jsonData = JsonUtility.ToJson(data);
            File.WriteAllText(filePath, jsonData);
        }

        public static T Load<T>(string key, T defaultValue = default(T))
        {
            string filePath = DataPath + "/" + key + ".json";
            if (File.Exists(filePath))
            {
                string jsonData = File.ReadAllText(filePath);
                return JsonUtility.FromJson<T>(jsonData);
            }
            return defaultValue;
        }
        
        public static void Clear(string key)
        {
            string filePath = DataPath + "/" + key + ".json";
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}