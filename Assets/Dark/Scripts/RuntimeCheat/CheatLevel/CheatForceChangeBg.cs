using System;
using InGame;
using UnityEngine;
using UnityEngine.UI;

namespace Dark.Scripts.RuntimeCheat.CheatLevel
{
    public class CheatForceChangeBg : MonoBehaviour
    {
        [SerializeField] private Button btnChangeBgNormal;
        [SerializeField] private Button btnChangrBgBoss;
        
        private BackgroundInGame bgController;

        private void Awake()
        {
            bgController = FindAnyObjectByType<BackgroundInGame>();
        }

        private void OnEnable()
        {
            btnChangeBgNormal.onClick.RemoveAllListeners();
            btnChangeBgNormal.onClick.AddListener(ChangeBgNormal);
            btnChangrBgBoss.onClick.RemoveAllListeners();
            btnChangrBgBoss.onClick.AddListener(ChangeBgBoss);
        }

        private void ChangeBgNormal()
        {
            if (bgController) bgController.ForceChangeBg(false, true);
        }

        private void ChangeBgBoss()
        {
            if (bgController) bgController.ForceChangeBg(true, true);
        }
    }
}