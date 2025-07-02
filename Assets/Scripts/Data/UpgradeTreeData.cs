using System;
using System.Collections.Generic;
using InGame.Upgrade;
using Sirenix.Serialization;

namespace Data
{
    [Serializable]
    public class UpgradeTreeData
    {
        public UpgradeNodeData treeRoot;
    }

    [Serializable]
    public class UpgradeNodeData
    {
        public List<UpgradeNodeData> children;
        public UpgradeNodeConfig nodeConfig;
        public bool unlocked;

        public UpgradeNodeData(UpgradeNodeConfig config)
        {
            children = new List<UpgradeNodeData>();
            nodeConfig = config;
        }
    }
}