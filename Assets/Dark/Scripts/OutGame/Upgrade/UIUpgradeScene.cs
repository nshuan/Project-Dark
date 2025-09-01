using System;
using Core;
using Dark.Scripts.SceneNavigation;
using Data;
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
            if (PlayerDataManager.Instance.IsNewData)
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
                Loading.Instance.LoadScene(SceneConstants.SceneMenu);
            });
        }

        public void SelectClass(CharacterClass classType)
        {
            // Save selected class
            if (PlayerDataManager.Instance.IsNewData)
            {
                var data = PlayerDataManager.Instance.Data;
                data.characterClass = (int)classType;
                
                PlayerDataManager.Instance.Save(data);
            }
            
            // Load Upgrade tree
            panelUpgradeTree.SetActive(true);
            panelSelectClass.SetActive(false);
            Instantiate(UpgradeTreeManifest.GetTreePrefab(classType), treeParent);
        }
    }
}