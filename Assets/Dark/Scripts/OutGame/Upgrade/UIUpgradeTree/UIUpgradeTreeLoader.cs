using System;
using InGame.CharacterClass;
using UnityEngine;

namespace Dark.Scripts.OutGame.Upgrade
{
    public class UIUpgradeTreeLoader : MonoBehaviour
    {
        [SerializeField] private Transform parent;

        private void Awake()
        {
            Instantiate(UpgradeTreeManifest.Instance.GetTreePrefab(CharacterClass.Archer), parent);
        }
    }
}