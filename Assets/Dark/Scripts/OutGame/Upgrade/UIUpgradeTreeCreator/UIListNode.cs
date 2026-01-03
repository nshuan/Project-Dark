using System;
using System.Collections.Generic;
using System.Linq;
using InGame.Upgrade;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dark.Scripts.OutGame.Upgrade.UIUpgradeTreeCreator
{
    public class UIListNode : SerializedMonoBehaviour
    {
        // [SerializeField] private UpgradeTreeConfig upgradeTreeConfig;
        [SerializeField] private ScrollRect scrollView;
        [SerializeField] private RectTransform contentParent;
        [SerializeField] private UIListNodeItem itemPrefab;
        [SerializeField] private TMP_InputField searchInputField;
        [OdinSerialize, NonSerialized] private Dictionary<NodeType, Button> btnInfo;
        [SerializeField] private GameObject groupCreatorButton;
        
        private UICreatorManager manager;
        private UICreatorConfigLoader configLoader;
        private Dictionary<UpgradeNodeConfig, UIListNodeItem> itemPool = new Dictionary<UpgradeNodeConfig, UIListNodeItem>();
        private List<UpgradeNodeConfig> allNodes = new List<UpgradeNodeConfig>();
        private bool isInitialized = false;

        private int selectingNodeId = -1;

        private void Awake()
        {
            manager = FindAnyObjectByType<UICreatorManager>();
            configLoader = FindAnyObjectByType<UICreatorConfigLoader>();
        }

        private void Start()
        {
            // Store all nodes for filtering
            allNodes = configLoader.GetAllConfigs();
            InitializePool();

            // Setup search input field
            if (searchInputField != null)
            {
                searchInputField.onValueChanged.RemoveAllListeners();
                searchInputField.onValueChanged.AddListener(OnSearchValueChanged);
            }

            if (manager)
            {
                foreach (var info in btnInfo)
                {
                    info.Value.onClick.RemoveAllListeners();
                    info.Value.onClick.AddListener(() =>
                    {
                        if (selectingNodeId == -1)
                        {
                            ActiveCreatorButton(false);
                            return;
                        }
                        
                        manager.CreateNewNode(info.Key, 0, selectingNodeId, Guid.Empty);
                    });
                }
            }
        }

        private void InitializePool()
        {
            if (isInitialized) return;

            if (itemPrefab == null)
            {
                Debug.LogError("ItemPrefab is not assigned!");
                return;
            }

            if (contentParent == null)
            {
                Debug.LogError("ContentParent is not assigned!");
                return;
            }

            // Create items for all nodes and add to pool
            foreach (var nodeConfig in allNodes)
            {
                var item = Instantiate(itemPrefab, contentParent);
                item.Setup(nodeConfig);
                item.SetSelected(false);
                item.btnSelect.onClick.RemoveAllListeners();
                item.btnSelect.onClick.AddListener(() =>
                {
                    selectingNodeId = nodeConfig.nodeId;
                    ActiveCreatorButton(true);
                    foreach (var pair in itemPool)
                    {
                        pair.Value.SetSelected(false);
                    }
                    item.SetSelected(true);
                });
                item.gameObject.SetActive(true);
                itemPool[nodeConfig] = item;
            }

            isInitialized = true;
        }

        private void OnSearchValueChanged(string searchText)
        {
            FilterItems(searchText);
        }

        public void FilterItems(string searchText)
        {
            if (!isInitialized)
            {
                InitializePool();
            }

            // Get filtered nodes
            var nodesToShow = string.IsNullOrEmpty(searchText)
                ? allNodes
                : allNodes.Where(node => 
                    node.nodeName.Contains(searchText, System.StringComparison.OrdinalIgnoreCase) ||
                    node.description.Contains(searchText, System.StringComparison.OrdinalIgnoreCase) ||
                    node.nodeId.ToString().Contains(searchText, System.StringComparison.OrdinalIgnoreCase)
                ).ToList();

            // Enable items that match the filter, disable others
            foreach (var kvp in itemPool)
            {
                var nodeConfig = kvp.Key;
                var item = kvp.Value;
                
                bool shouldShow = nodesToShow.Contains(nodeConfig);
                item.gameObject.SetActive(shouldShow);
            }
        }

        private void ActiveCreatorButton(bool active)
        {
            groupCreatorButton.SetActive(active);
        }

        public void Open()
        {
            foreach (var pair in itemPool)
            {
                pair.Value.SetSelected(false);
            }
            selectingNodeId = -1;
            ActiveCreatorButton(false);
            gameObject.SetActive(true);
        }

        public void Close()
        {
            selectingNodeId = -1;
            ActiveCreatorButton(false);
            gameObject.SetActive(false);
        }
    }
}