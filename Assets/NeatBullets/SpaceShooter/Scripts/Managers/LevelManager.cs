using UnityEngine;
using UnityEngine.SceneManagement;

namespace NeatBullets.SpaceShooter.Scripts.Managers {
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance;
        private void Awake()
        {
            if (Instance == null) {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else {
                Destroy(gameObject);
            }
        }
    
        private void Start() {
            Application.targetFrameRate = 180;
        }
        
        public void LoadScene(string sceneName) {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
        


    }
}
