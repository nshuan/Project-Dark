using System;
using System.Collections.Generic;
using Dark.Scripts.SceneNavigation;
using Data;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dark.Scripts.Recording
{
    public class RecordTimePlayed : MonoBehaviour
    {
        private List<string> recordScenes = new List<string>() { "Upgrade", "InGame" };
        private List<string> ignoredScenes = new List<string>() { "Blank "};
        
        private DateTime currentStartTime;
        
        private string currentScene;
        private bool recording = false;
        
        private void Awake()
        {
            currentScene = SceneManager.GetActiveScene().name;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (ignoredScenes.Contains(scene.name)) return;
            
            // Chuyển sang upgrade từ home hoặc init nghĩa là bắt đầu chơi
            if (recordScenes.Contains(scene.name) && !recordScenes.Contains(currentScene))
            {
                // Set start time to count
                currentStartTime = DateTime.Now;
                recording = true;
                return;
            }

            // back ra home hoặc init thì lưu lại thời gian đã chơi
            if (!recordScenes.Contains(scene.name))
            {
                if (!recording) return;
                
                Save();
                recording = false;
                return;
            }
            
            // Đổi giữa upgrade với ingame thì cũng lưu lại luôn
            if (recordScenes.Contains(scene.name) && recordScenes.Contains(currentScene))
            {
                if (!recording)
                {
                    currentStartTime = DateTime.Now;
                    recording = true;
                }
                else
                {
                   Save();
                }
                
                return;
            }
      
            currentScene = scene.name;
        }

        // Đang chơi dở bị thoát khỏi game cũng lưu luôn
        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus == true)
            {
                if (recording)
                    Save();
                recording = false;
            }
            else
            {
                if (recordScenes.Contains(currentScene))
                {
                    currentStartTime = DateTime.Now;
                    recording = true;
                }
            }
        }

        private void Save()
        {
            var timePlayed = DateTime.Now - currentStartTime;
            currentStartTime = DateTime.Now;
            var data = PlayerDataManager.Instance.Data;
            data.timePlayedMilli += timePlayed.TotalMilliseconds;
            PlayerDataManager.Instance.Save(data);
        }
    }
}