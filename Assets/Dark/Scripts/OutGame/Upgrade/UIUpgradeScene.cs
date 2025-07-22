using System;
using InGame.CharacterClass;
using InGame.Upgrade;
using InGame.Upgrade.UI;
using UnityEngine;

namespace Dark.Scripts.OutGame.Upgrade
{
    public class UIUpgradeScene : MonoBehaviour
    {
        [SerializeField] private Transform treeParent;
        
        private UIUpgradeTree cacheTree;
        
        private void Start()
        {
            cacheTree = Instantiate(UpgradeTreeConfig.Instance.GetTree(ClassManager.Instance.CurrentClass), treeParent);
        }
    }
}