using System;
using Dark.Scripts.Common.UIWarning;
using InGame.Upgrade;
using TMPro;
using UnityEngine;

namespace Dark.Scripts.OutGame.Upgrade
{
    public class UIPopupConfirmExchange : UIPopupWarning
    {
        [SerializeField] private TextMeshProUGUI txtVestige;
        [SerializeField] private TextMeshProUGUI txtEchoes;
        [SerializeField] private TextMeshProUGUI txtSigils;
        [SerializeField] private TextMeshProUGUI txtNodeName;
        
        public void Setup(int vestige, int echoes, int sigils, string title, string content, string nodeNameToDisplay, Action callbackYes, Action callbackNo = null)
        {
            txtVestige.SetText($"{vestige}");
            txtEchoes.SetText($"{echoes}");
            txtSigils.SetText($"{sigils}");
            txtNodeName.SetText($"\"{nodeNameToDisplay}\"");

            callbackYes += () =>
            {
                this.DoCloseFadeOut();
            };
            Setup(title, content, callbackYes, callbackNo);
        }
    }
}