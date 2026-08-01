using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using UnityEngine;
using Newtonsoft.Json;
using UnityEditor;

namespace Data
{
    public class DataHandler
    {
        private static readonly string keyEncrypt = "ash_warden_key_2026";

        public static string DataPath
        {
            get
            {
                var path = string.Empty;
#if UNITY_EDITOR
                path = Application.dataPath + "/_DataTest";
#else
                path = Application.persistentDataPath;
#endif
                EnsureDataDirectory(path);
                return path;
            }
        }

        private static void EnsureDataDirectory(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        public static bool Exist<T>(string key)
        {
            string filePath = DataPath + "/" + key + ".json";
            return File.Exists(filePath);
        }

        // =========================
        // SAVE (NEW FORMAT ONLY)
        // =========================
        public static void Save<T>(string key, T data)
        {
            string filePath = DataPath + "/" + key + ".json";

            // Old wrapping logic preserved
            object finalData = data;
            if (typeof(T) == typeof(int) || typeof(T) == typeof(bool))
                finalData = new WrappedData<T>(data);

            string json = JsonConvert.SerializeObject(finalData);

            // Create wrapper with hash
            SaveWrapper wrapper = new SaveWrapper
            {
                data = json,
                hash = GenerateHash(json)
            };

            string wrappedJson = JsonConvert.SerializeObject(wrapper);

            // Encrypt
            string encrypted = Encrypt(wrappedJson, keyEncrypt);

            File.WriteAllText(filePath, encrypted);
        }

        // =========================
        // LOAD (WITH MIGRATION)
        // =========================
        public static T Load<T>(string key, T defaultValue = default(T))
        {
            string filePath = DataPath + "/" + key + ".json";

            if (!File.Exists(filePath))
                return defaultValue;

            string fileContent = File.ReadAllText(filePath);

            // -------------------------
            // 1. Try NEW encrypted format
            // -------------------------
            try
            {
                string wrappedJson = Decrypt(fileContent, keyEncrypt);
                SaveWrapper wrapper = JsonConvert.DeserializeObject<SaveWrapper>(wrappedJson);

                if (wrapper == null)
                    throw new Exception("Invalid wrapper");

                // Verify hash
                if (wrapper.hash != GenerateHash(wrapper.data))
                    throw new Exception("Hash mismatch");

                return DeserializeData<T>(wrapper.data, key, filePath);
            }
            catch
            {
                // -------------------------
                // 2. OLD format fallback
                // -------------------------
                Debug.Log($"[DataHandler] Old save detected for key: {key}, migrating...");

                try
                {
                    T oldData = DeserializeData<T>(fileContent, key, filePath);

                    // 🔥 Re-save in new format
                    Save(key, oldData);

                    return oldData;
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"[DataHandler] Failed to load old data: {e}");
                    return defaultValue;
                }
            }
        }

        private static T DeserializeData<T>(string jsonData, string key, string filePath)
        {
            if (typeof(T) == typeof(int) || typeof(T) == typeof(bool))
            {
                try
                {
                    return JsonConvert.DeserializeObject<WrappedData<T>>(jsonData).value;
                }
                catch
                {
                    // Old unwrapped int/bool
                    var data = JsonConvert.DeserializeObject<T>(jsonData);

                    // Re-save fixed format
                    Save<T>(key, data);

                    return data;
                }
            }

            return JsonConvert.DeserializeObject<T>(jsonData);
        }

        public static void Clear(string key)
        {
            string filePath = DataPath + "/" + key + ".json";

            if (File.Exists(filePath))
            {
#if UNITY_EDITOR
                filePath = "Assets/_DataTest/" + key + ".json";
                AssetDatabase.DeleteAsset(filePath);
                AssetDatabase.Refresh();
                return;
#endif
                File.Delete(filePath);
            }
        }

        // =========================
        // ENCRYPTION
        // =========================
        private static string Encrypt(string data, string key)
        {
            string xored = Xor(data, key);
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(xored));
        }

        private static string Decrypt(string encryptedData, string key)
        {
            string decoded = Encoding.UTF8.GetString(Convert.FromBase64String(encryptedData));
            return Xor(decoded, key);
        }

        private static string Xor(string input, string key)
        {
            char[] result = new char[input.Length];

            for (int i = 0; i < input.Length; i++)
                result[i] = (char)(input[i] ^ key[i % key.Length]);

            return new string(result);
        }

        // =========================
        // HASH
        // =========================
        private static string GenerateHash(string data)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(data + keyEncrypt);
                byte[] hash = sha.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        [Serializable]
        private class SaveWrapper
        {
            public string data;
            public string hash;
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
