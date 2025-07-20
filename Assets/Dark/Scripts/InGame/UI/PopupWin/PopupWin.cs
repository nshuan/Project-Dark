using System;
using Dark.Scripts.CoreUI;
using Dark.Scripts.SceneNavigation;
using UnityEngine;
using UnityEngine.UI;

namespace InGame.UI
{
    public class PopupWin : MonoBehaviour
    {
        [SerializeField] private UIPopup ui;
        
        [SerializeField] private Button btnBackToTree;
        [SerializeField] private Button btnNextLevel;

        private void Start()
        {
            LevelManager.Instance.OnWin += OnWin;
        }

        private void OnWin()
        {
            UpdateUI();
            ui.DoOpen();
        }

        private void UpdateUI()
        {
            btnBackToTree.onClick.RemoveAllListeners();
            btnBackToTree.onClick.AddListener(() =>
            {
                Loading.Instance.LoadScene(SceneConstants.SceneUpgrade);
            });
        }
    }
}