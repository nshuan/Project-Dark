using System;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

namespace Data
{
    public class DataHandler
    {
        public static string DataPath
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
            var jsonData = "";
            if (typeof(T) == typeof(int))
                jsonData = JsonConvert.SerializeObject(new WrappedData<T>(data));
            else
                jsonData = JsonConvert.SerializeObject(data);
            File.WriteAllText(filePath, jsonData);
        }

        public static T Load<T>(string key, T defaultValue = default(T))
        {
            string filePath = DataPath + "/" + key + ".json";
            if (File.Exists(filePath))
            {
                string jsonData = File.ReadAllText(filePath);
                if (typeof(T) == typeof(int))
                    return JsonConvert.DeserializeObject<WrappedData<T>>(jsonData).value;
                return JsonConvert.DeserializeObject<T>(jsonData);
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

    [Serializable]
    public class WrappedData<T>
    {
        public T value;

        public WrappedData(T data)
        {
            this.value = data;
        }
    }
}