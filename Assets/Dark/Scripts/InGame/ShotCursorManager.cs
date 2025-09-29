using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace InGame
{
    [CreateAssetMenu(menuName = "Scriptable Objects/ShotCursorManager", fileName = "ShotCursorManager", order = 1)]
    public class ShotCursorManager : SerializedScriptableObject
    {
        [NonSerialized, OdinSerialize] private Dictionary<ShotCursorType, MonoCursor> cursorPrefabs = new();
        [NonSerialized, OdinSerialize] private Dictionary<ShotCursorType, IMouseInput> moveCursorMap = new();
        
        public MonoCursor GetPrefab(ShotCursorType cursorType, Transform parent = null)
        {
            return Instantiate(cursorPrefabs.GetValueOrDefault(cursorType), parent);
        }

        public IMouseInput GetCursorMoveLogic(ShotCursorType cursorType, Camera cam, MonoCursor cursor)
        {
            if (!moveCursorMap.ContainsKey(cursorType)) return null;
            return Activator.CreateInstance(moveCursorMap[cursorType].GetType(), cam, cursor) as IMouseInput;
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