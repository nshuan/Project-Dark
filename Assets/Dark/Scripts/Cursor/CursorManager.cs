using System;
using Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dark.Scripts.Cursor
{
    public class CursorManager : MonoSingleton<CursorManager>
    {
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
            var cursor = CursorManifest.GetCursorInfo(CursorKind.OutGame);
            UnityEngine.Cursor.SetCursor(cursor.tex, cursor.hotSpot, cursor.mode);
            UnityEngine.Cursor.visible = true;
        }
        
        public void SetCursorInGame()
        {
            UnityEngine.Cursor.visible = false;
        }
    }
}