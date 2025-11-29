using System;
using InGame;
using UnityEngine;

namespace Dark.Scripts.RuntimeCheat
{
    public class CheatOnOffLaserWaveOnTreeSpawn : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                GameConst.HideLaserWaveOnSpawnTree = !GameConst.HideLaserWaveOnSpawnTree;
            }
        }
    }
}