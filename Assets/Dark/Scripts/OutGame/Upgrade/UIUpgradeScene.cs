using System;
using Core;
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
        [SerializeField] private GameObject panelUpgradeTree;
        [SerializeField] private Transform treeParent;

        [Space] [Header("Select class")] 
        [SerializeField] private GameObject panelSelectClass;

        protected override void Awake()
        {
            if (PlayerDataManager.Instance.Data.initialized == false)
            {
                panelUpgradeTree.SetActive(false);
                panelSelectClass.SetActive(true);
            }
            else
            {
                panelUpgradeTree.SetActive(true);
                panelSelectClass.SetActive(false);
                Instantiate(UpgradeTreeManifest.GetTreePrefab((CharacterClass)PlayerDataManager.Instance.Data.characterClass), treeParent);
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
                Loading.Instance.LoadScene(SceneConstants.SceneInGame, () =>
                {
                    LevelManager.Instance.LoadLevel(PlayerDataManager.Instance.Data.level + 1);
                });
            });
        }
    }
}