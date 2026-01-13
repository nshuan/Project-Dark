using System;
using InGame;
using InGame.Upgrade;
using UnityEngine;

namespace Dark.Scripts.RuntimeCheat
{
    public class UIRuntimeCheatFakeInspector : MonoBehaviour
    {
        public RuntimeInspector inspector;

        private void Awake()
        {
            UpgradeManager.Instance.OnActivated += OnUpgradeActivated;
        }

        private void OnDestroy()
        {
            UpgradeManager.Instance.OnActivated -= OnUpgradeActivated;
        }

        private void OnUpgradeActivated(UpgradeBonusInfoV2 bonusInfo)
        {
            inspector.selected = bonusInfo;
        }
    }
}