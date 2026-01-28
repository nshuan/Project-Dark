using System;
using TMPro;
using UnityEngine;

namespace Dark.Scripts.OutGame.Home
{
    public class UIDisplayGameVersion : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI txtVersion;

        private void Start()
        {
            txtVersion.SetText($"v{Application.version}");
        }
    }
}