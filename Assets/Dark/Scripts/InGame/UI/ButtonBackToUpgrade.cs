using Dark.Scripts.SceneNavigation;
using InGame.Pause;
using UnityEngine;
using UnityEngine.UI;

namespace InGame.UI
{
    [RequireComponent(typeof(Button))]
    public class ButtonBackToUpgrade : MonoBehaviour
    {
        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
        }

        private void Start()
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                // Loading.Instance.QuickLoadScene(SceneConstants.SceneUpgrade);
                PauseGame.Instance.Pause();
            });
        }
    }
}