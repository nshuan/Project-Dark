using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InGame.EndlessEditor
{
    public class EndlessLevelRenameEditor : MonoBehaviour
    {
        [SerializeField] private TMP_InputField inpName;
        [SerializeField] private Button btnSubmit;
        [SerializeField] private Button btnCancel;
        [SerializeField] private TextMeshProUGUI txtInvalid;

        private Action onCancel;
        
        private void Awake()
        {
            btnSubmit.onClick.RemoveAllListeners();
            btnSubmit.onClick.AddListener(() => inpName?.onSubmit?.Invoke(inpName.text));
            
            btnCancel.onClick.RemoveAllListeners();
            btnCancel.onClick.AddListener(() => onCancel?.Invoke());
        }

        public void GetName(string defaultName, Func<string, bool> nameValidator, Action<string> callbackSubmit, Action callbackCancel)
        {
            onCancel = callbackCancel;
            
            gameObject.SetActive(true);
            txtInvalid.gameObject.SetActive(false);
            inpName.text = defaultName;
            inpName.onSubmit.RemoveAllListeners();
            inpName.onSubmit.AddListener((t) =>
            {
                if (!nameValidator(t))
                {
                    DOTween.Kill(txtInvalid);
                    txtInvalid.gameObject.SetActive(true);
                    DOVirtual.DelayedCall(2f, () => txtInvalid.gameObject.SetActive(false)).SetTarget(txtInvalid);
                    return;
                }
                
                DOTween.Kill(txtInvalid);
                txtInvalid.gameObject.SetActive(false);
                callbackSubmit?.Invoke(t);
                gameObject.SetActive(false);
            });
        }
    }
}