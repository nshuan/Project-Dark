using System;
using System.Linq;
using Economic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace InGame.Upgrade
{
    [CreateAssetMenu(menuName = "Dark/Upgrade/Upgrade Node Config", fileName = "UpgradeNodeConfig")]
    public class UpgradeNodeConfig : SerializedScriptableObject
    {
        public int nodeId;
        public string nodeName; // Name to display
        public UpgradeNodeConfig[] preRequire;
        public string description; // Description to display
        public UpgradeNodeCostInfo[] costInfo; 
        public int levelNum = 1;
        [NonSerialized, OdinSerialize] public INodeActivateLogic[] nodeLogic;
        [Space] [Header("Visual")] 
        public Sprite nodeSprite;
        public Sprite nodeSpriteLock;
        
        public UpgradeNodeState State { get; set; }
        public bool Activated { get; set; }

        public void ActivateNode(int level, ref UpgradeBonusInfo bonusInfo)
        {
            if (nodeLogic == null) return;
            if (level <= 0 || level > levelNum) return;
            for (var i = 1; i <= level; i++)
            {
                foreach (var logic in nodeLogic)
                {
                    logic.ActivateNode(level, ref bonusInfo);
                }   
            }
        }

#if UNITY_EDITOR
        
        private void OnValidate()
        {
            if (preRequire.Contains(this))
            {
                var tempRequire = preRequire.ToList();
                tempRequire.Remove(this);
                preRequire = tempRequire.ToArray();
            }
        }

#endif
    }

    [Serializable]
    public class UpgradeNodeCostInfo
    {
        public WealthType costType; // Type of resource needed to unlock this node
        public int[] costValue;
    }

    public enum UpgradeNodeState
    {
        Locked,
        CanUnlock,
        Unlocked
    }
}