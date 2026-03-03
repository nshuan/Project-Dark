using System;
using Dark.Scripts.CoreUI;
using Dark.Scripts.Utils;
using Dark.Tools.Language.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dark.Scripts.Common.UIWarning
{
    public class UIPopupWarning : UIPopup
    {
        [SerializeField] private TextMeshProUGUI txtTitle;
        [SerializeField] private TextMeshProUGUI txtContent;
        [SerializeField] private Button btnYes;
        [SerializeField] private Button btnNo;

        public virtual void Setup(string title, string content, Action callbackYes, Action callbackNo = null)
        {
            btnYes.interactable = true;
            btnNo.interactable = true;
            txtTitle.SetText(title);
            txtContent.SetText(content);
            btnYes.onClick.RemoveAllListeners();
            btnYes.onClick.AddListener(() =>
            {
                btnYes.interactable = false;
                btnNo.interactable = false;
                this.DelayCall(UIConst.BtnDelayOnClick, () => callbackYes?.Invoke());
            });
            btnNo.onClick.RemoveAllListeners();
            btnNo.onClick.AddListener(() =>
            {
                btnYes.interactable = false;
                btnNo.interactable = false;
                if (callbackNo != null)
                    callbackNo?.Invoke();
                else
                    this.DoCloseFadeOut();
            });
        }
        
        public virtual void LocalizedSetup(string keyTitle, string keyContent, Action callbackYes, Action callbackNo = null)
        {
            btnYes.interactable = true;
            btnNo.interactable = true;
            txtTitle.SetTextLanguage(keyTitle);
            txtContent.SetTextLanguage(keyContent);
            btnYes.onClick.RemoveAllListeners();
            btnYes.onClick.AddListener(() =>
            {
                btnYes.interactable = false;
                btnNo.interactable = false;
                this.DelayCall(UIConst.BtnDelayOnClick, () => callbackYes?.Invoke());
            });
            btnNo.onClick.RemoveAllListeners();
            btnNo.onClick.AddListener(() =>
            {
                btnYes.interactable = false;
                btnNo.interactable = false;
                if (callbackNo != null)
                    callbackNo?.Invoke();
                else
                    this.DoCloseFadeOut();
            });
        }
    }
}