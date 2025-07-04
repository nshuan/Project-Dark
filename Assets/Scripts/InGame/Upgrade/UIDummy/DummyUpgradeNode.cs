using System;
using InGame.Upgrade.UI;
using UnityEngine;
using UnityEngine.UI;

namespace InGame.Upgrade.UIDummy
{
    public class DummyUpgradeNode : MonoBehaviour
    {
        public UpgradeNodeConfig nodeConfig;
        public UIUpgradeNode nodeUI;

        [SerializeField] private Image nodePlaceHolder;

        private void OnEnable()
        {
            nodeUI.UpdateUI(new UpgradeNodeData());
        }

        public void HidePlaceHolder()
        {
            nodePlaceHolder.color -= new Color(0f, 0f, 0f, 1f);
            nodePlaceHolder.enabled = false;
        }
    }
}