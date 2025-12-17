using System;
using InGame;
using UnityEngine;

namespace Dark.Scripts.RuntimeCheat
{
    public class CheatOnOffOnTreeSpawn : MonoBehaviour
    {
#if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                GameConst.HideLaserWaveOnSpawnTree = !GameConst.HideLaserWaveOnSpawnTree;
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                GameConst.HideLockedNode = !GameConst.HideLockedNode;
            }
        }
#endif
    }
}