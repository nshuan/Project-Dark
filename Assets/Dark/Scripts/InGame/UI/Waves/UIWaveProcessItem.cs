using System;
using UnityEngine;

namespace InGame.UI.Waves
{
    public class UIWaveProcessItem : MonoBehaviour
    {
        [SerializeField] private GameObject[] states;
        
        public void UpdateUI(int stateIndex)
        {
            stateIndex = Math.Clamp(stateIndex, 0, states.Length - 1);
            for (var i = 0; i < states.Length; i++)
            {
                states[i].SetActive(i == stateIndex);
            }
        }
    }
}