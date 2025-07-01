using System.IO;
using UnityEngine;

namespace Database
{
    public class SaveLoadData : MonoBehaviour
    {
        public string savePath = "/playerData.json";

        public void Save(PlayerData data)
        {
            var jsonData = JsonUtility.ToJson(data);
            File.WriteAllText(Application.persistentDataPath + savePath, jsonData);
        }

        public PlayerData Load()
        {
            string filePath = Application.persistentDataPath + savePath;
            if (File.Exists(filePath))
            {
                string jsonData = File.ReadAllText(filePath);
                return JsonUtility.FromJson<PlayerData>(jsonData);
            }
            return null;
        }
    }
}