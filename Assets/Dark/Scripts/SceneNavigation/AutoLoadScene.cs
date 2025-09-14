using System;
using UnityEngine;

namespace Dark.Scripts.SceneNavigation
{
    public class AutoLoadScene : MonoBehaviour
    {
        public string sceneNameToLoad;
        
        private void Start()
        {
            Loading.Instance.LoadScene(sceneNameToLoad);    
        }
    }
}