using UnityEngine;
using UnityEngine.UI;

namespace Dark.Scripts.ForDemo
{
    public class UIButtonFeedback : MonoBehaviour
    {
        [SerializeField] private Button btnFeedback;
        
        private void Awake()
        {
            btnFeedback.onClick.RemoveAllListeners();
            btnFeedback.onClick.AddListener(() =>
            {
                Application.OpenURL(DemoConfig.FeedbackURL);
            });
        }
    }
}