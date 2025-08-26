using System.IO;
using UnityEngine;

namespace Data
{
    public class DataHandler
    {
        public static bool Exist<T>(string key)
        {
            string filePath = Application.dataPath + "/" + key + ".json";
            return File.Exists(filePath);
        }
        
        public static void Save<T>(string key, T data)
        {
            string filePath = Application.dataPath + "/" + key + ".json";
            var jsonData = JsonUtility.ToJson(data);
            File.WriteAllText(filePath, jsonData);
        }

        public static T Load<T>(string key, T defaultValue = default(T))
        {
            string filePath = Application.dataPath + "/" + key + ".json";
            if (File.Exists(filePath))
            {
                string jsonData = File.ReadAllText(filePath);
                return JsonUtility.FromJson<T>(jsonData);
            }
            return defaultValue;
        }
    }
}