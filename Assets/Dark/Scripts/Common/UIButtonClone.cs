using System;
using UnityEngine;
using UnityEngine.UI;

namespace Dark.Scripts.Common
{
    [RequireComponent(typeof(Button))]
    public class UIButtonClone : MonoBehaviour
    {
        [SerializeField] private Button target;

        private Button thisButton;
        
        private void Awake()
        {
            thisButton = GetComponent<Button>();
            thisButton.onClick.RemoveAllListeners();
            thisButton.onClick.AddListener(() => target.onClick?.Invoke());
        }
    }
}