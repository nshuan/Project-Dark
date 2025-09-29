using Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace InGame
{
    [CreateAssetMenu(menuName = "Scriptable Objects/ElementalEffectManager", fileName = "ElementalEffectManager", order = 1)]
    public class ElementalEffectManager : SerializedScriptableObject
    {
        #region Singleton

        private const string Path = "ScriptableObjects/ElementalEffectManager";

        private static ElementalEffectManager instance;
        public static ElementalEffectManager Instance
        {
            get
            {
                if (instance == null)
                    instance = Resources.Load<ElementalEffectManager>(Path);

                return instance;
            }
        }

        #endregion
    }
}