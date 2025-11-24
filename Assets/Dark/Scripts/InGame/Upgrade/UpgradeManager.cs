using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Dark.Scripts.InGame.Upgrade;
using Dark.Scripts.OutGame.Upgrade;
using Data;
using Economic;
using Cheat;
using UnityEngine;

namespace InGame.Upgrade
{
    public class UpgradeManager : Singleton<UpgradeManager>
    {
        #region Actions

        public Action<UpgradeBonusInfo> OnActivated;

        #endregion
        
        #region Data

        private static string DataKey => GetDataKey(PlayerDataManager.DataKey);
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

        public void InitData()
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
            DataHandler.Save(DataKey, Data);
        }

        public void ClearData(string dataKey)
        {
            data = null;
            if (DataHandler.Exist<UpgradeData>(dataKey))
                DataHandler.Clear(dataKey);
        }

        public static string GetDataKey(string playerDataKey)
        {
            return playerDataKey + "_UpgradeData";
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
           
            TreeConfig.ActivateTree(Data.nodes, ref bonusInfo);

#if UNITY_EDITOR
            var testBonusInfo = CheatBonusData.GetBonus();
            if (testBonusInfo.Item1) // enabled = true
            {
                bonusInfo = testBonusInfo.Item2;
                bonusInfo.skillBonus ??= new UpgradeBonusSkillInfo();
                bonusInfo.passiveMapByTriggerType ??= new Dictionary<PassiveTriggerType, List<PassiveType>>();
            }
#endif
            
            OnActivated?.Invoke(bonusInfo);
        }
        
        public bool UpgradeNode(int nodeId)
        {
            if (TreeConfig.GetNodeById(nodeId) == null) return false;
            
            if (!dataMapById.ContainsKey(nodeId))
            {
                var newNodeData = new UpgradeNodeData() { id = nodeId, level = 0 };
                Data.nodes.Add(newNodeData);
                dataMapById.Add(nodeId, newNodeData);
            }

            var nodeConfig = TreeConfig.GetNodeById(nodeId);
            
            if (dataMapById[nodeId].level >= nodeConfig.MaxLevel) return false;

            var currentLevel = dataMapById[nodeId].level;
            var costInfo = nodeConfig.costInfo;
            var costValueToSpend = new Dictionary<WealthType, int>();
            foreach (var cost in costInfo)
            {
                var costValueIndex = cost.costType switch
                {
                    WealthType.Vestige => Data.indexVestige,
                    WealthType.Echoes => Data.indexEchoes,
                    WealthType.Sigils => Data.indexSigils,
                    _ => 0
                };
                
                var costValue = UpgradeRequirementConfig.Instance.GetRequirement(cost.costType, costValueIndex);
                
                if (!WealthManager.Instance.CanSpend(cost.costType, costValue)) 
                {
                    DebugUtility.LogWarning($"Upgrade node {nodeConfig.nodeName} failed: Not enough resource!");
                    return false;
                }
                
                costValueToSpend[cost.costType] = costValue;
            }
            
            foreach (var cost in nodeConfig.costInfo)
            {
                WealthManager.Instance.Spend(cost.costType, costValueToSpend[cost.costType]);
                switch (cost.costType)
                {
                    case WealthType.Vestige:
                        Data.indexVestige += 1;
                        break;
                    case WealthType.Echoes:
                        Data.indexEchoes += 1;
                        break;
                    case WealthType.Sigils:
                        Data.indexSigils += 1;
                        break;
                }
            }
            dataMapById[nodeId].Upgrade();
            Save();
            return true;
        }

        public UpgradeNodeData GetData(int nodeId)
        {
            return dataMapById.GetValueOrDefault(nodeId);
        }

        public int GetRequirementIndex(WealthType costType)
        {
            return costType switch
            {
                WealthType.Vestige => Data.indexVestige,
                WealthType.Echoes => Data.indexEchoes,
                WealthType.Sigils => Data.indexSigils,
                _ => 0
            };
        }
        
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
        public int indexVestige;
        public int indexEchoes;
        public int indexSigils;

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