using UnityEngine;
using UnityEngine.UI;

namespace Dark.Scripts.Common
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Button))]
    public sealed class UIOpenUrlButton : MonoBehaviour
    {
        [SerializeField] private UrlType urlType;

        private UrlConfigSO _config;
        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _config = Resources.Load<UrlConfigSO>("UrlConfig");
        }

        private void OnEnable()
        {
            if (_button == null)
                _button = GetComponent<Button>();

            if (_button != null)
                _button.onClick.AddListener(OpenUrl);
        }

        private void OnDisable()
        {
            if (_button != null)
                _button.onClick.RemoveListener(OpenUrl);
        }

        public void OpenUrl()
        {
            var url = _config.GetUrl(urlType);
            
            if (!string.IsNullOrWhiteSpace(url))
            {
                Application.OpenURL(url.Trim());
                return;
            }

            Debug.LogWarning(
                $"{nameof(UIOpenUrlButton)} on '{name}' requires a URL.",
                this);
        }
    }
}