using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dark.Scripts.Audio
{
    public class ButtonMute : MonoBehaviour
    {
        [SerializeField] private Button btnMute;
        [SerializeField] private TextMeshProUGUI txtMute;

        private void Awake()
        {
            btnMute.onClick.RemoveAllListeners();
            btnMute.onClick.AddListener(OnButtonClicked);
        }

        private void OnEnable()
        {
            txtMute.SetText(AudioManager.Instance.IsMuted ? "Unmute" : "Mute");
        }

        private void OnButtonClicked()
        {
            if (AudioManager.Instance.IsMuted)
            {
                AudioManager.Instance.Unmute();
                txtMute.SetText("Mute");
            }
            else
            {
                AudioManager.Instance.Mute();
                txtMute.SetText("Unmute");
            }
        }
    }
}