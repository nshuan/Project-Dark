using System;
using UnityEngine;

namespace Dark.Scripts.ForDemo
{
    public class DemoConfigAutoLoader : MonoBehaviour
    {
        private void Awake()
        {
            if (DemoConfig.IsDemo)
                DemoConfig.Instance.InitPublicProperties();
        }
    }
}