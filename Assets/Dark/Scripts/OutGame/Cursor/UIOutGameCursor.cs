using System;
using UnityEngine;

namespace Dark.Scripts.OutGame
{
    public class UIOutGameCursor : MonoBehaviour
    {
        private void Start()
        {
            
        }

        private void Update()
        {
            transform.position = Input.mousePosition;
        }
    }
}