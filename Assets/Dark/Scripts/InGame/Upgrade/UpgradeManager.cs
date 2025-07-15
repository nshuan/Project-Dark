using System;
using System.Collections.Generic;
using Core;
using Data;
using InGame.ConfigManager;
using InGame.Upgrade.UIDummy;
using UnityEngine;

namespace InGame.Upgrade
{
    public class UpgradeManager : Singleton<UpgradeManager>
    {
        #region Actions

        public Action<UpgradeBonusInfo> OnActivated;

        #endregion
        
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
            data = DataHandler.Load<UpgradeData>(DataKey, new UpgradeData(TreeConfig.nodeMapById));
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

        public UpgradeManager()
        {
            InitData();
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
            // Init bonus infor
            bonusInfo = new UpgradeBonusInfo();
            bonusInfo.skillBonus = new UpgradeBonusSkillInfo();

            bonusInfo.effectsMapByTriggerType = new Dictionary<EffectTriggerType, List<EffectType>>();
                
            TreeConfig.ActivateTree(Data.nodes, ref bonusInfo);

#if UNITY_EDITOR
            if (testBonusInfo != null)
            {
                bonusInfo = testBonusInfo;
                bonusInfo.skillBonus ??= new UpgradeBonusSkillInfo();
                bonusInfo.effectsMapByTriggerType ??= new Dictionary<EffectTriggerType, List<EffectType>>();
            }
#endif
            
            OnActivated?.Invoke(bonusInfo);
        }
        
        public bool UpgradeNode(int nodeId)
        {
            if (!nodeMapById.ContainsKey(nodeId)) return false;
            if (nodeMapById[nodeId].level >= treeConfig.nodeMapById[nodeId].levelNum) return false;
            
            nodeMapById[nodeId].Upgrade();
            Save();
            return true;
        }

        public UpgradeNodeData GetData(int nodeId)
        {
            return nodeMapById.GetValueOrDefault(nodeId);
        }

#if UNITY_EDITOR
        private UpgradeBonusInfo testBonusInfo;
        public void ForceTestBonusInfo(UpgradeBonusInfo bonusInfo)
        {
            testBonusInfo = bonusInfo;
        }
#endif
    }

    [Serializable]
    public class UpgradeData
    {
        public List<UpgradeNodeData> nodes;

        public UpgradeData(Dictionary<int, UpgradeNodeConfig> nodeMap)
        {
            nodes = new List<UpgradeNodeData>();
            foreach (var pair in nodeMap)
            {
                nodes.Add(new UpgradeNodeData()
                {
                    id = pair.Key,
                    level = 0
                });
            }
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