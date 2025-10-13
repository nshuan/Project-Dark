using System;
using UnityEngine;
using UnityEngine.UI;

namespace Dark.Scripts.RuntimeCheat
{
    public class UIRuntimeCheatChangeBg : MonoBehaviour
    {
        public Button btnChange;
        public GameObject[] bgs;

        private int currentBg;

        private void Awake()
        {
            btnChange.onClick.RemoveAllListeners();
            btnChange.onClick.AddListener(() =>
            {
                if (bgs == null) return;
                if (bgs.Length == 0) return;
                
                currentBg = (currentBg + 1) % bgs.Length;
                for (var i = 0; i < bgs.Length; i++)
                {
                    bgs[i].SetActive(i == currentBg);
                }
            });
        }
    }
}