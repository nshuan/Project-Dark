using System;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace InGame.Upgrade
{
    public abstract class UpgradeNodeConfig : SerializedScriptableObject
    {
        [ReadOnly] public int nodeIndex;
        public string nodeName; // Name to display
        public UpgradeNodeConfig[] preRequire;
        public string description; // Description to display
        public int costType; // Type of resource needed to unlock this node
        public int costValue; 
        
        public UpgradeNodeState State { get; set; }
        public bool Activated { get; set; }

        public abstract void ActivateNode();

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

    public enum UpgradeNodeState
    {
        Locked,
        CanUnlock,
        Unlocked
    }
}