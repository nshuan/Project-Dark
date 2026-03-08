using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Dark.Scripts.OutGame.Upgrade;
using Data;
using Economic;
using Cheat;
using Dark.Scripts.OutGame.SaveSlot;
using InGame.Upgrade.DynamicCost;
using Steamworks.NET;
using UnityEngine;

namespace InGame.Upgrade
{
    public class UpgradeManager : Singleton<UpgradeManager>
    {
        #region Actions

        public Action<UpgradeBonusInfoV2> OnActivated;

        #endregion
        
        #region Data

        private static string DataKey => GetDataKey(PlayerDataManager.DataKey);
        private Dictionary<string, UpgradeData> preloadedData = new Dictionary<string, UpgradeData>();
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
        private Dictionary<int, int> groupUnlockOrderMapById;
        private int currentGroupUnlockOrder;
        private int currentNodeUnlockOrder;
        
        public void PreloadAllData()
        {
            preloadedData = new Dictionary<string, UpgradeData>();
            foreach (var key in SaveSlotManager.SlotDataKeys)
            {
                var upgradeDataKey = GetDataKey(key);
                preloadedData[upgradeDataKey] = DataHandler.Load<UpgradeData>(upgradeDataKey, new UpgradeData());
            }
        }
        
        public void InitData()
        {
            PreloadAllData();
            if (preloadedData != null && preloadedData.ContainsKey(DataKey))
            {
                data = preloadedData[DataKey];
            }
            else
            {
                data = DataHandler.Load<UpgradeData>(DataKey, new UpgradeData());
                preloadedData ??= new Dictionary<string, UpgradeData>();
                preloadedData[DataKey] = data;
            }
#if UNITY_EDITOR
            // data = new UpgradeData(TreeConfig.nodeMapById);
#endif
            dataMapById = new Dictionary<int, UpgradeNodeData>();
            var index = 0;
            foreach (var node in data.nodes)
            {
                dataMapById.TryAdd(node.id, node);
                if (node.unlockOrder > currentNodeUnlockOrder) currentNodeUnlockOrder = node.unlockOrder;
            }
            RefreshGroupUnlockOrder();
        }

        private void Save()
        {
            DataHandler.Save(DataKey, Data);
            preloadedData[DataKey] = data;
        }

        public void ClearData(string dataKey)
        {
            data = null;
            if (DataHandler.Exist<UpgradeData>(dataKey))
                DataHandler.Clear(dataKey);
            preloadedData[dataKey] = new UpgradeData();
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
                    treeConfig = UpgradeTreeManifest.GetTreeConfig((CharacterClass.CharacterClass)PlayerDataManager.Instance.Data.characterClass);
                return treeConfig;
            }
        }
        
        #endregion
        
        public void ActivateTree(ref UpgradeBonusInfoV2 bonusInfo)
        {
            // Init bonus infor
            bonusInfo = new UpgradeBonusInfoV2();
           
            TreeConfig.ActivateTree(Data.nodes, ref bonusInfo);

#if UNITY_EDITOR
            var testBonusInfo = CheatBonusData.GetBonus();
            if (testBonusInfo.Item1) // enabled = true
            {
                bonusInfo = testBonusInfo.Item2;
            }
#endif
            
            OnActivated?.Invoke(bonusInfo);
        }
        
        /// <summary>
        /// nodeGroupLockOrder: unlock order comparing to other group lock node
        /// </summary>
        /// <param name="nodeId"></param>
        /// <param name="groupIds"></param>
        /// <returns></returns>
        public bool UpgradeNode(UpgradeNodeType nodeType, int nodeId, UpgradeGroupIdInfo[] groupIds)
        {
            if (TreeConfig.GetNodeById(nodeId) == null) return false;
            
            if (!dataMapById.ContainsKey(nodeId))
            {
                var newNodeData = new UpgradeNodeData() { id = nodeId, level = 0, unlockOrder = 999999 };
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
                var nodeGroupUnlockOrder = groupIds.Min((groupId) => GetGroupUnlockOrder(groupId.groupId, false));
                var costValue = 0;
                if (cost.costType == WealthType.Vestige)
                {
                    if (nodeConfig.dynamicVestige)
                    {
                        if (nodeConfig.MaxLevel == 1)
                            costValue = Mathf.RoundToInt(nodeConfig.vestigeCostRatio *
                                                         DynamicVestigeConfig.Instance.GetCost1Stage(nodeGroupUnlockOrder));
                        else
                        {
                            var listCostValue = DynamicVestigeConfig.Instance.GetCost5Stage(nodeGroupUnlockOrder);
                            currentLevel = Math.Min(currentLevel, listCostValue.Length - 1);
                            costValue = Mathf.RoundToInt(nodeConfig.vestigeCostRatio * listCostValue[currentLevel]);
                        }
                    }
                    else
                    {
                        currentLevel = Math.Min(currentLevel, cost.costValue.Length - 1);
                        costValue = Mathf.RoundToInt(nodeConfig.vestigeCostRatio * cost.costValue[currentLevel]);
                    }
                }
                else if (cost.costType == WealthType.Echoes)
                {
                    if (nodeConfig.dynamicEchoes)
                    {
                        if (nodeConfig.MaxLevel == 1)
                            costValue = Mathf.RoundToInt(1f * DynamicVestigeConfig.Instance.GetCost1Echoes(nodeGroupUnlockOrder));
                        else
                        {
                            var listCostValue = DynamicVestigeConfig.Instance.GetCost5Echoes(nodeGroupUnlockOrder);
                            currentLevel = Math.Min(currentLevel, listCostValue.Length - 1);
                            costValue = Mathf.RoundToInt(1f * listCostValue[currentLevel]);
                        }
                    }
                    else
                    {
                        currentLevel = Math.Min(currentLevel, cost.costValue.Length - 1);
                        costValue = Mathf.RoundToInt(1f * cost.costValue[currentLevel]);
                    }
                }
                else
                {
                    currentLevel = Math.Min(currentLevel, cost.costValue.Length - 1);
                    costValue = Mathf.RoundToInt(1f * cost.costValue[currentLevel]);
                }
                
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
            if (dataMapById[nodeId].level == 1)
            {
                foreach (var groupId in groupIds)
                {
                    if (!groupUnlockOrderMapById.ContainsKey(groupId.groupId) && groupId.isLockNode)
                    {
                        groupUnlockOrderMapById.Add(groupId.groupId, groupUnlockOrderMapById.Count);
                    }
                }
            }

            RefreshGroupUnlockOrder();
            dataMapById[nodeId].unlockOrder = currentNodeUnlockOrder;
            currentNodeUnlockOrder += 1;
                
            Save();
            
            SetSteamAchievementForSkill(nodeType, nodeId);
            
            return true;
        }

        public bool CanUpgrade(int nodeId, UpgradeGroupIdInfo[] groupIds)
        {
            if (TreeConfig.GetNodeById(nodeId) == null) return false;
            
            if (!dataMapById.ContainsKey(nodeId))
            {
                var newNodeData = new UpgradeNodeData() { id = nodeId, level = 0, unlockOrder = 999999 };
                Data.nodes.Add(newNodeData);
                dataMapById.Add(nodeId, newNodeData);
            }

            var nodeConfig = TreeConfig.GetNodeById(nodeId);
            
            // Nếu nodeConfig.MaxLevel = 0 thì override lại 
            if (nodeConfig.MaxLevel == 0)
            {
                var groupUnlockOrder =
                    nodeConfig.groupId.Min((info) => GetGroupUnlockOrder(info.groupId, true));
                foreach (var logicV2 in nodeConfig.nodeLogic)
                {
                    if (logicV2 is INodeDynamicBonusValueV2 { IsDynamic: true } dynamicLogic)
                    {
                        dynamicLogic.OverrideBonusValue(groupUnlockOrder);
                    }
                }
            }
            if (dataMapById[nodeId].level >= nodeConfig.MaxLevel) return false;

            var currentLevel = dataMapById[nodeId].level;
            var costInfo = nodeConfig.costInfo;
            foreach (var cost in costInfo)
            {
                var nodeGroupUnlockOrder = groupIds.Min((groupId) => GetGroupUnlockOrder(groupId.groupId, false));
                var costValue = 0;
                if (cost.costType == WealthType.Vestige)
                {
                    if (nodeConfig.dynamicVestige)
                    {
                        if (nodeConfig.MaxLevel == 1)
                            costValue = Mathf.RoundToInt(nodeConfig.vestigeCostRatio *
                                                         DynamicVestigeConfig.Instance.GetCost1Stage(nodeGroupUnlockOrder));
                        else
                        {
                            var listCostValue = DynamicVestigeConfig.Instance.GetCost5Stage(nodeGroupUnlockOrder);
                            currentLevel = Math.Min(currentLevel, listCostValue.Length - 1);
                            costValue = Mathf.RoundToInt(nodeConfig.vestigeCostRatio * listCostValue[currentLevel]);
                        }
                    }
                    else
                    {
                        currentLevel = Math.Min(currentLevel, cost.costValue.Length - 1);
                        costValue = Mathf.RoundToInt(nodeConfig.vestigeCostRatio * cost.costValue[currentLevel]);
                    }
                }
                else if (cost.costType == WealthType.Echoes)
                {
                    if (nodeConfig.dynamicEchoes)
                    {
                        if (nodeConfig.MaxLevel == 1)
                            costValue = Mathf.RoundToInt(1f * DynamicVestigeConfig.Instance.GetCost1Echoes(nodeGroupUnlockOrder));
                        else
                        {
                            var listCostValue = DynamicVestigeConfig.Instance.GetCost5Echoes(nodeGroupUnlockOrder);
                            currentLevel = Math.Min(currentLevel, listCostValue.Length - 1);
                            costValue = Mathf.RoundToInt(1f * listCostValue[currentLevel]);
                        }
                    }
                    else
                    {
                        currentLevel = Math.Min(currentLevel, cost.costValue.Length - 1);
                        costValue = Mathf.RoundToInt(1f * cost.costValue[currentLevel]);
                    }
                }
                else
                {
                    currentLevel = Math.Min(currentLevel, cost.costValue.Length - 1);
                    costValue = Mathf.RoundToInt(1f * cost.costValue[currentLevel]);
                }
                
                if (!WealthManager.Instance.CanSpend(cost.costType, costValue)) 
                {
                    return false;
                }
            }

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

        /// <summary>
        /// 999999 là chưa unlock
        /// </summary>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        public int GetNodeUnlockOrder(int nodeId)
        {
            if (!dataMapById.ContainsKey(nodeId)) return 999999;
            return dataMapById.GetValueOrDefault(nodeId).unlockOrder;
        }
        
        public int GetGroupUnlockOrder(int groupId, bool refresh)
        {
            if (!TreeConfig.nodeGroupsMapById.ContainsKey(groupId)) return 999999;
            if (refresh) RefreshGroupUnlockOrder();
            return groupUnlockOrderMapById[groupId];;
        }

        public bool IsGroupUnlocked(int groupId)
        {
            if (!TreeConfig.nodeGroupsMapById.ContainsKey(groupId)) return false;
            return groupUnlockOrderMapById[groupId] < 999999;
        }
        
        public void RefreshGroupUnlockOrder()
        {
            var groups = TreeConfig.nodeGroupsMapById.Values.ToList();
            var unlockedData = Data.nodes.Where(node => node.level >= 1).ToDictionary(d => d.id);
            var unlockedGroup = new List<UpgradeNodeGroup>();
            var lockedGroup = new List<UpgradeNodeGroup>();
            foreach (var group in groups)
            {
                if (unlockedData.ContainsKey(group.lockNode.nodeId)) unlockedGroup.Add(group);
                else lockedGroup.Add(group);
            }
            unlockedGroup.Sort((group1, group2) => unlockedData[group1.lockNode.nodeId].unlockOrder
                .CompareTo(unlockedData[group2.lockNode.nodeId].unlockOrder));
            groupUnlockOrderMapById ??= new Dictionary<int, int>();
            for (var i = 0; i < unlockedGroup.Count; i++)
            {
                groupUnlockOrderMapById[unlockedGroup[i].groupId] = i;
            }

            currentGroupUnlockOrder = unlockedGroup.Count;
            
            foreach (var group in lockedGroup)
            {
                groupUnlockOrderMapById[group.groupId] = currentGroupUnlockOrder;
            }
        }

        private void SetSteamAchievementForSkill(UpgradeNodeType nodeType, int nodeId)
        {
            if (nodeType != UpgradeNodeType.NodeSkill && nodeType != UpgradeNodeType.NodeEffect) return;
            
            // Echopiercer - Graven Edge
            if (nodeId == 2)
            {
                if (PlayerDataManager.Instance.Data.Class == CharacterClass.CharacterClass.Archer)
                    SteamStats.Instance.TryClaimAchievement(SteamAchievementsAPIName.UNLOCK_ECHOPIERCER);
                else if (PlayerDataManager.Instance.Data.Class == CharacterClass.CharacterClass.Knight)
                    SteamStats.Instance.TryClaimAchievement(SteamAchievementsAPIName.UNLOCK_GRAVEN_EDGE);
                return;
            }
            
            // Fracture Volley - Afterslash
            if (nodeId == 3)
            {
                if (PlayerDataManager.Instance.Data.Class == CharacterClass.CharacterClass.Archer)
                    SteamStats.Instance.TryClaimAchievement(SteamAchievementsAPIName.UNLOCK_FRACTURE_VOLLEY);
                else if (PlayerDataManager.Instance.Data.Class == CharacterClass.CharacterClass.Knight)
                    SteamStats.Instance.TryClaimAchievement(SteamAchievementsAPIName.UNLOCK_AFTERSLASH);
                return;
            }
            
            // Splitting Echo - Last Stand
            if (nodeId == 4)
            {
                if (PlayerDataManager.Instance.Data.Class == CharacterClass.CharacterClass.Archer)
                    SteamStats.Instance.TryClaimAchievement(SteamAchievementsAPIName.UNLOCK_SPLITTING_ECHO);
                else if (PlayerDataManager.Instance.Data.Class == CharacterClass.CharacterClass.Knight)
                    SteamStats.Instance.TryClaimAchievement(SteamAchievementsAPIName.UNLOCK_LAST_STAND);
                return;
            }
            
            // Wanderfang - Stormcoid
            if (nodeId == 5)
            {
                if (PlayerDataManager.Instance.Data.Class == CharacterClass.CharacterClass.Archer)
                    SteamStats.Instance.TryClaimAchievement(SteamAchievementsAPIName.UNLOCK_WANDERFANG);
                else if (PlayerDataManager.Instance.Data.Class == CharacterClass.CharacterClass.Knight)
                    SteamStats.Instance.TryClaimAchievement(SteamAchievementsAPIName.UNLOCK_STORMCOIL);
                return;
            }
            
            // Vanguard's Line
            if (nodeId == 6)
            {
                SteamStats.Instance.TryClaimAchievement(SteamAchievementsAPIName.UNLOCK_VANGUARD_LINE);
                return;
            }
            
            // Echofall
            if (nodeId == 7)
            {
                SteamStats.Instance.TryClaimAchievement(SteamAchievementsAPIName.UNLOCK_ECHOFALL);
                return;
            }
            
            // Vowpierce
            if (nodeId == 8)
            {
                SteamStats.Instance.TryClaimAchievement(SteamAchievementsAPIName.UNLOCK_VOWPIERCE);
                return;
            }
            
            // Trine Severance
            if (nodeId == 9)
            {
                SteamStats.Instance.TryClaimAchievement(SteamAchievementsAPIName.UNLOCK_TRINE_SEVERANCE);
                return;
            }
            
            // Passive Lightning
            if (nodeId is 10 or 14 or 18 or 22)
            {
                SteamStats.Instance.TryClaimAchievement(SteamAchievementsAPIName.UNLOCK_LIGHTNING_CHAIN);
                return;
            }
            
            // Passive Explosion
            if (nodeId is 11 or 15 or 19 or 23)
            {
                SteamStats.Instance.TryClaimAchievement(SteamAchievementsAPIName.UNLOCK_EXPLOSION);
                return;
            }
            
            // Passive Burning
            if (nodeId is 12 or 16 or 20 or 24)
            {
                SteamStats.Instance.TryClaimAchievement(SteamAchievementsAPIName.UNLOCK_THUNDER);
                return;
            }
            
            // Passive Lightning
            if (nodeId is 13 or 17 or 21 or 25)
            {
                SteamStats.Instance.TryClaimAchievement(SteamAchievementsAPIName.UNLOCK_BURNING);
                return;
            }
        }
        
#if HOT_CHEAT
        public void CheatUpdateBonusInfo(UpgradeBonusInfoV2 bonusInfo)
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
        public int unlockOrder = 999999;

        public void Upgrade()
        {
            level += 1;
        }
    }
    
    [Serializable]
    public class UpgradeNodeGroup
    {
        public int groupId;
        public UpgradeNodeConfig lockNode; // Node này đã unlock thì mới tính là group đã unlock
        public List<UpgradeNodeConfig> nodeList; // Tất cả node có trong group
    }
}