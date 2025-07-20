using System;
using Dark.Scripts.CoreUI;
using Dark.Scripts.SceneNavigation;
using Economic;
using UnityEngine;
using UnityEngine.UI;

namespace InGame.UI
{
    public class PopupWin : MonoBehaviour
    {
        [SerializeField] private UIPopup ui;
        [SerializeField] private GameObject imgBlockRaycast;
        
        [Space]
        [SerializeField] private Button btnBackToTree;
        [SerializeField] private Button btnNextLevel;

        private void Start()
        {
            LevelManager.Instance.OnWin += OnWin;
        }

        private void OnWin()
        {
            UpdateUI();
            imgBlockRaycast.SetActive(true);
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