using System;
using InGame.Upgrade;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Home
{
    public class UpgradeTreeManager : MonoBehaviour
    {
        [SerializeField] private UpgradeTreeConfig upgradeTree;
        
        [Button]
        public void ActivateTree()
        {
            upgradeTree.Activate();   
        }
    }
}