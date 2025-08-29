using System;
using Core;
using Data;
using InGame.CharacterClass;
using InGame.Upgrade;
using UnityEngine;
using UnityEngine.Serialization;

namespace Dark.Scripts.OutGame.Upgrade
{
    public class UIUpgradeScene : MonoSingleton<UIUpgradeScene>
    {
        [Header("Upgrade Tree")]
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