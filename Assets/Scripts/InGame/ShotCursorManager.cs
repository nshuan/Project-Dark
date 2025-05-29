using System;
using System.Collections.Generic;
using InGame.MouseInput;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace InGame
{
    [CreateAssetMenu(menuName = "Scriptable Objects/ShotCursorManager", fileName = "ShotCursorManager", order = 1)]
    public class ShotCursorManager : SerializedScriptableObject
    {
        [NonSerialized, OdinSerialize] private Dictionary<ShotCursorType, MonoCursor> cursorPrefabs = new();

        public MonoCursor GetPrefab(ShotCursorType cursorType)
        {
            return cursorPrefabs.GetValueOrDefault(cursorType);
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