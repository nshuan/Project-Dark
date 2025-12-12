using System;
using Core;
using Dark.Scripts.Analytics;
using Dark.Scripts.OutGame.SaveSlot;
using Dark.Scripts.SceneNavigation;
using Dark.Scripts.Utils;
using Data;
using InGame;
using InGame.CharacterClass;
using InGame.Upgrade;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Dark.Scripts.OutGame.Upgrade
{
    public class UIUpgradeScene : MonoSingleton<UIUpgradeScene>
    {
        [Header("Common")] 
        [SerializeField] private Button btnBack;
        
        [Space] [Header("Upgrade Tree")]
        [SerializeField] private UIPanelUpgradeTree panelUpgradeTree;

        [Space] [Header("Select class")] 
        [SerializeField] private GameObject panelSelectClass;

        protected override void Awake()
        {
            if (PlayerDataManager.Instance.Data.initialized == false)
            {
                panelUpgradeTree.gameObject.SetActive(false);
                panelSelectClass.SetActive(true);
            }
            else
            {
                panelUpgradeTree.gameObject.SetActive(true);
                panelSelectClass.SetActive(false);
                panelUpgradeTree.SpawnTree();
            }
            
            btnBack.onClick.RemoveAllListeners();
            btnBack.onClick.AddListener(() =>
            {
                btnBack.interactable = false;
                Loading.Instance.LoadScene(SceneConstants.SceneMenu);
            });
        }

        public void SelectClass(CharacterClass classType)
        {
            // Save selected class
            if (PlayerDataManager.Instance.Data.initialized == false)
            {
                var data = PlayerDataManager.Instance.Data;
                data.characterClass = (int)classType;
                data.initialized = true;
                
                PlayerDataManager.Instance.Save(data);
                
                var totalDataCreated = SaveSlotManager.Instance.GetTotalDataCreated();
                totalDataCreated += 1;
                SaveSlotManager.Instance.SaveTotalDataCreated(totalDataCreated);
                LogManager.Log(LogConst.EventLogTotalDataSlotsCreated, totalDataCreated.ToString());
            }
            
            // // Load Upgrade tree
            // panelUpgradeTree.SetActive(true);
            // panelSelectClass.SetActive(false);
            // Instantiate(UpgradeTreeManifest.GetTreePrefab(classType), treeParent);
            
            // Load level
#if UNITY_EDITOR
            LevelManager.isLoadFromInit = true;
#endif
            this.DelayCall(0.5f, () =>
            {
                LogManager.Log(LogConst.EventLogStartLevel, $"level_{PlayerDataManager.Instance.Data.level + 1}", "from class select");
                
                Loading.Instance.QuickLoadScene(SceneConstants.SceneInGame, () =>
                {
                    LevelManager.Instance.LoadLevel(PlayerDataManager.Instance.Data.level + 1);
                });
            });
        }
    }
}