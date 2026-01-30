using System;
using UnityEngine;

namespace InGame
{
    public class EnemyHealthBar : MonoBehaviour
    {
        [SerializeField] private Transform hpBar;
        
        public int MaxHp { get; set; }

        public void UpdateHp(int hp)
        {
            if (MaxHp <= 0)
            {
                hpBar.gameObject.SetActive(false);
                return;
            }
            
            hp = Math.Clamp(hp, 0, MaxHp);
            hpBar.localScale = new Vector3((float)hp / MaxHp, 1, 1);
            hpBar.gameObject.SetActive(true);
        }
    }
}