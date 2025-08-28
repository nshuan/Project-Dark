using System;
using Data;
using InGame.CharacterClass;
using InGame.Upgrade;
using UnityEngine;
using UnityEngine.Serialization;

namespace Dark.Scripts.OutGame.Upgrade
{
    public class UIUpgradeScene : MonoBehaviour
    {
        [Header("Upgrade Tree")]
        [SerializeField] private GameObject panelUpgradeTree;
        [SerializeField] private Transform treeParent;

        [Space] [Header("Select class")] 
        [SerializeField] private GameObject panelSelectClass;

        private void Awake()
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
                Instantiate(UpgradeTreeManifest.GetTreePrefab(CharacterClass.Archer), treeParent);
            }
        }
    }
}