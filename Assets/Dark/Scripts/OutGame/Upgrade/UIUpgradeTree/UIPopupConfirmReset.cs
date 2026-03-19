using System;
using Dark.Scripts.Common.UIWarning;
using Dark.Tools.Language.Runtime;
using TMPro;
using UnityEngine;

namespace Dark.Scripts.OutGame.Upgrade
{
    public class UIPopupConfirmReset : UIPopupWarning
    {
        [SerializeField] private TextMeshProUGUI txtVestige;
        [SerializeField] private TextMeshProUGUI txtEchoes;
        [SerializeField] private TextMeshProUGUI txtSigils;
        [SerializeField] private TextMeshProUGUI txtNodeName;
        [SerializeField] private TextMeshProUGUI txtDescription1;
        
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
        
        public void SetupLocalize(int vestige, int echoes, int sigils, string keyTitle, string keyContent, string keyNameDisplay, string keyContent1, Action callbackYes, Action callbackNo = null)
        {
            txtVestige.SetText($"{vestige}");
            txtEchoes.SetText($"{echoes}");
            txtSigils.SetText($"{sigils}");
            txtNodeName.SetTextValueLanguage(
                $"\"{LanguageData.Instance.GetLocalizedString(keyNameDisplay, LanguageManager.Instance.CurrentLanguage)}\"");
            
            callbackYes += () =>
            {
                this.DoCloseFadeOut();
            };
            LocalizedSetup(keyTitle, keyContent, callbackYes, callbackNo);
            txtDescription1.SetTextLanguage(keyContent1);
        }
    }
}