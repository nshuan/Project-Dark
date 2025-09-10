using System;
using Dark.Scripts.CoreUI;
using Dark.Scripts.Utils;
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

        public void Setup(string title, string content, Action callbackYes, Action callbackNo = null)
        {
            txtTitle.SetText(title);
            txtContent.SetText(content);
            btnYes.onClick.RemoveAllListeners();
            btnYes.onClick.AddListener(() =>
            {
                this.DelayCall(UIConst.BtnDelayOnClick, () => callbackYes?.Invoke());
            });
            btnNo.onClick.RemoveAllListeners();
            btnNo.onClick.AddListener(() =>
            {
                if (callbackNo != null)
                    callbackNo?.Invoke();
                else
                    this.DoClose();
            });
        }
    }
}