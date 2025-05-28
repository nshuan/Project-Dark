using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace InGame
{
    [CreateAssetMenu(menuName = "Scriptable Objects/ShotCursorManager", fileName = "ShotCursorManager", order = 1)]
    public class ShotCursorManager : SerializedScriptableObject
    {
        [SerializeField] private Dictionary<ShotCursorType, GameObject> cursorPrefs = new();

        public GameObject GetPrefab(ShotCursorType cursorType)
        {
            return cursorPrefs.GetValueOrDefault(cursorType);
        }
        
        #region Singleton

        private const string Path = "ScriptableObjects/ShotCursorManager";

        private static ShotCursorManager instance;
        public static ShotCursorManager Instance
        {
            get
            {
                if (instance == null)
                    instance = Resources.Load<ShotCursorManager>(Path);

                return instance;
            }
        }

        #endregion
    }

    public enum ShotCursorType
    {
        SINGLE,
        LINE,
        BOX
    }
}