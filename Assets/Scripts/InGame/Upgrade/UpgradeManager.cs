using System;
using System.Collections.Generic;
using Core;
using Data;
using InGame.Upgrade.UIDummy;
using UnityEngine;

namespace InGame.Upgrade
{
    public class UpgradeManager : Singleton<UpgradeManager>
    {
        #region Data

        private const string DataKey = "UpgradeData";
        private UpgradeData data;

        public UpgradeData Data
        {
            get
            {
                if (data == null) InitData();
                return data;
            }
        }

        private Dictionary<int, UpgradeNodeData> nodeMapById;

        private void InitData()
        {
            data = DataHandler.Load<UpgradeData>(DataKey, new UpgradeData());
            nodeMapById = new Dictionary<int, UpgradeNodeData>();
            foreach (var node in data.nodes)
            {
                nodeMapById.TryAdd(node.id, node);
            }
        }

        private void Save()
        {
            DataHandler.Save(DataKey, data);
        }

        #endregion

        #region Config

        private const string UpgradeTreeConfigPath = "DummyUpgradeTreeConfig";

        private DummyUpgradeTreeConfig treeConfig;

        public DummyUpgradeTreeConfig TreeConfig
        {
            get
            {
                if (treeConfig == null) 
                    treeConfig = Resources.Load<DummyUpgradeTreeConfig>(UpgradeTreeConfigPath);
                return treeConfig;
            }
        }
        
        #endregion
        
        public void ActivateTree(ref UpgradeBonusInfo bonusInfo)
        {
            TreeConfig.ActivateTree(Data.nodes, ref bonusInfo);
        }
        
        public void UpgradeNode(int nodeId)
        {
            if (!nodeMapById.ContainsKey(nodeId)) return;
            nodeMapById[nodeId].Upgrade();
            Save();
        }
    }

    [Serializable]
    public class UpgradeData
    {
        public List<UpgradeNodeData> nodes;

        public UpgradeData()
        {
            nodes = new List<UpgradeNodeData>();
        }
    }

    [Serializable]
    public class UpgradeNodeData
    {
        public int id;
        public int level;

        public void Upgrade()
        {
            level += 1;
        }
    }
}