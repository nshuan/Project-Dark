using System;
using System.Collections;
using System.Collections.Generic;
using Dark.Scripts.SceneNavigation;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Dark.Scripts.Common.Init
{
    public class InitAndLoadHome : SerializedMonoBehaviour
    {
        [OdinSerialize, NonSerialized] private List<IEarlyInitialize> initializers;
        [SerializeField] private bool autoLoadHome = true;

        private IEnumerator Start()
        {
            foreach (var initializer in initializers)
            {
                initializer.Initialize();
                yield return new WaitForEndOfFrame();
            }
            
            if (autoLoadHome)
                Loading.Instance.LoadScene(SceneConstants.SceneMenu);    
        }
    }
}