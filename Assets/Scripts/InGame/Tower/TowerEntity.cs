using System;
using UnityEngine;

namespace InGame
{
    public class TowerEntity : MonoBehaviour
    {
        [SerializeField] private GameObject towerRange;
        
        public void EnterTower()
        {
            towerRange.SetActive(true);    
        }

        public void LeaveTower()
        {
            towerRange.SetActive(false);
        }
    }
}