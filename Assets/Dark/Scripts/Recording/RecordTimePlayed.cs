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
        private List<string> recordScenes = new List<string>() { "Upgrade", "BaseLevel" };
        private List<string> ignoredScenes = new List<string>() { "Blank", "Level3Towers", "Level3TowersSquare", "Level4Towers", "Level4TowersTriangle" };
        
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
                currentScene = scene.name;
                return;
            }

            // back ra home hoặc init thì lưu lại thời gian đã chơi
            if (!recordScenes.Contains(scene.name))
            {
                if (!recording) return;
                
                Save();
                recording = false;
                currentScene = scene.name;
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
                
                currentScene = scene.name;
                return;
            }
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
            // Data chưa initialize thì ko lưu
            if (!data.initialized) return;
            data.timePlayedMilli += timePlayed.TotalMilliseconds;
            PlayerDataManager.Instance.Save(data);
        }
    }
}