using System;
using Dark.Scripts.OutGame.SaveSlot;
using Dark.Scripts.SceneNavigation;
using UnityEngine;
using UnityEngine.UI;

namespace Dark.Scripts.OutGame.Upgrade
{
    public class UISelectClassBackToHomeButton : MonoBehaviour
    {
        [SerializeField] private Button button;

        private void Awake()
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                button.interactable = false;
                SaveSlotManager.Instance.ClearSlot(SaveSlotManager.Instance.CurrentSlotIndex);
                Loading.Instance.LoadScene(SceneConstants.SceneMenu);
            });
        }
    }
}