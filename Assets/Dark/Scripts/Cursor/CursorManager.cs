using System;
using Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dark.Scripts.Cursor
{
    public class CursorManager : MonoSingleton<CursorManager>
    {
        [SerializeField] private CursorInfo cursorOutGame;
        [SerializeField] private CursorInfo cursorInGame;
        
        protected override void Awake()
        {
            base.Awake();
            SceneManager.sceneLoaded += OnSceneLoaded; 
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "BaseLevel")
            {
                SetCursorInGame();
            }
            else
            {
                SetCursorOutGame();
            }
        }

        public void SetCursorOutGame()
        {
            UnityEngine.Cursor.SetCursor(cursorOutGame.tex, cursorOutGame.hotSpot, cursorOutGame.mode);
            UnityEngine.Cursor.visible = true;
        }
        
        public void SetCursorInGame()
        {
            UnityEngine.Cursor.visible = false;
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