using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Dark.Scripts.OutGame.Upgrade;
using Data;
using Economic;
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

        private Dictionary<int, UpgradeNodeData> dataMapById;

        private void InitData()
        {
            data = DataHandler.Load<UpgradeData>(DataKey, new UpgradeData());
#if UNITY_EDITOR
            // data = new UpgradeData(TreeConfig.nodeMapById);
#endif
            dataMapById = new Dictionary<int, UpgradeNodeData>();
            foreach (var node in data.nodes)
            {
                dataMapById.TryAdd(node.id, node);
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
        
        private UpgradeTreeConfig treeConfig;

        public UpgradeTreeConfig TreeConfig
        {
            get
            {
                if (treeConfig == null) 
                    treeConfig = UpgradeTreeManifest.GetTreeConfig(CharacterClass.CharacterClass.Archer);
                return treeConfig;
            }
        }
        
        #endregion
        
        public void ActivateTree(ref UpgradeBonusInfo bonusInfo)
        {
            // Init bonus infor
            bonusInfo = new UpgradeBonusInfo();
            bonusInfo.skillBonus = new UpgradeBonusSkillInfo();

            bonusInfo.passiveMapByTriggerType = new Dictionary<PassiveTriggerType, List<PassiveType>>();
                
            TreeConfig.ActivateTree(Data.nodes, ref bonusInfo);

#if UNITY_EDITOR
            if (testBonusInfo != null)
            {
                bonusInfo = testBonusInfo;
                bonusInfo.skillBonus ??= new UpgradeBonusSkillInfo();
                bonusInfo.passiveMapByTriggerType ??= new Dictionary<PassiveTriggerType, List<PassiveType>>();
            }
#endif
            
            OnActivated?.Invoke(bonusInfo);
        }
        
        public bool UpgradeNode(int nodeId)
        {
            if (!dataMapById.ContainsKey(nodeId))
            {
                var newNodeData = new UpgradeNodeData() { id = nodeId, level = 0 };
                data.nodes.Add(newNodeData);
                dataMapById.Add(nodeId, newNodeData);
            }

            if (TreeConfig.GetNodeById(nodeId) == null) return false;
            var nodeConfig = TreeConfig.GetNodeById(nodeId);
            
            if (dataMapById[nodeId].level >= nodeConfig.levelNum) return false;

            var currentLevel = dataMapById[nodeId].level;
            var costInfo = nodeConfig.costInfo;
            if (costInfo.Any((cost) =>
                    WealthManager.Instance.CanSpend(cost.costType, cost.costValue[currentLevel]) == false))
            {
                DebugUtility.LogWarning($"Upgrade node {nodeConfig.nodeName} failed: Not enough resource!");
                return false;
            }
            
            foreach (var cost in nodeConfig.costInfo)
            {
                WealthManager.Instance.Spend(cost.costType, cost.costValue[currentLevel]);    
            }
            dataMapById[nodeId].Upgrade();
            Save();
            return true;
        }

        public UpgradeNodeData GetData(int nodeId)
        {
            return dataMapById.GetValueOrDefault(nodeId);
        }

#if UNITY_EDITOR
        private UpgradeBonusInfo testBonusInfo;
        public void ForceTestBonusInfo(UpgradeBonusInfo bonusInfo)
        {
            testBonusInfo = bonusInfo;
        }

        public void ForceReactivateTree()
        {
            if (testBonusInfo != null)
            {
                testBonusInfo.skillBonus ??= new UpgradeBonusSkillInfo();
                testBonusInfo.passiveMapByTriggerType ??= new Dictionary<PassiveTriggerType, List<PassiveType>>();
            }
            
            LevelUtility.BonusInfo = testBonusInfo;
            OnActivated?.Invoke(testBonusInfo);
        }
#endif

#if HOT_CHEAT
        public void CheatUpdateBonusInfo(UpgradeBonusInfo bonusInfo)
        {
            OnActivated?.Invoke(bonusInfo);
        }
#endif
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