using Sirenix.OdinInspector;
using UnityEngine;

namespace Core
{
    public class SerializedMonoSingleton<T> : SerializedMonoBehaviour where T : Component
    {
        protected static T _instance;

        /// <summary>
        /// Singleton design pattern
        /// </summary>
        /// <value>The instance.</value>
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T> ();
                    if (_instance == null)
                    {
                        GameObject obj = new GameObject ();
                        _instance = obj.AddComponent<T> ();
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// On awake, we initialize our instance. Make sure to call base.Awake() in override if you need awake.
        /// </summary>
        protected virtual void Awake ()
        {
            _instance = this as T;			
        }
        protected virtual void OnDestroy()
        {
            _instance = null;
        }
    }
}