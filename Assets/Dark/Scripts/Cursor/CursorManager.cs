using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dark.Scripts.Cursor
{
    public class CursorManager : MonoBehaviour
    {
        [SerializeField] private CursorInfo cursorOutGame;
        [SerializeField] private CursorInfo cursorInGame;
        
        private void Awake()
        {
            SceneManager.sceneLoaded += OnSceneLoaded; 
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "BaseLevel")
            {
                UnityEngine.Cursor.SetCursor(cursorInGame.tex, cursorInGame.hotSpot, cursorInGame.mode);
                return;
            }

            if (scene.name is "Home" or "Upgrade")
            {
                UnityEngine.Cursor.SetCursor(cursorOutGame.tex, cursorOutGame.hotSpot, cursorOutGame.mode);
                return;
            }
        }
        
        [Serializable]
        public struct CursorInfo
        {
            public Texture2D tex;
            public Vector2 hotSpot;
            public CursorMode mode;
        }
    }
}