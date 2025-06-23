using System;
using InGame.Effects;
using UnityEngine;

namespace InGame
{
    public class TowerEntity : MonoBehaviour, IDamageable
    {
        [SerializeField] private TowerConfig config;
        [SerializeField] private GameObject towerRange;

        private int MaxHp { get; set; }
        private int CurrentHp { get; set; }
        public bool IsDestroyed { get; set; }
        
        public Action<TowerEntity> OnDestroyed;
        
        public void Initialize()
        {
            MaxHp = config.hp;
            CurrentHp = MaxHp;
            IsDestroyed = false;

            OnDestroyed = null;
        }
        
        public void EnterTower()
        {
            towerRange.SetActive(true);    
        }

        public void LeaveTower()
        {
            towerRange.SetActive(false);
        }

        public void Damage(int damage)
        {
            if (IsDestroyed) return;
            CurrentHp -= damage;
            if (CurrentHp <= 0)
            {
                IsDestroyed = true;
                OnDestroyed?.Invoke(this);
            }
            
            // Do damage effect
            var camShakeEffect = new CameraShake() { Cam = EffectHelper.Instance.DefaultCamera };
            EffectHelper.Instance.PlayEffect(camShakeEffect);
        }
    }
}