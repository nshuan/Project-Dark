using System;
using UnityEngine;

namespace InGame.UI.Waves
{
    public class UIWaveProcessItem : MonoBehaviour
    {
        [SerializeField] private RectTransform[] states;
        public RectTransform rectTransform;

        private void Awake()
        {
            rectTransform = (RectTransform)transform;
        }

        public void UpdateUI(int stateIndex)
        {
            stateIndex = Math.Clamp(stateIndex, 0, states.Length - 1);
            for (var i = 0; i < states.Length; i++)
            {
                states[i].gameObject.SetActive(i == stateIndex);
                if (i == stateIndex)
                {
                    rectTransform.sizeDelta = states[i].sizeDelta;
                }
            }
        }
    }
}