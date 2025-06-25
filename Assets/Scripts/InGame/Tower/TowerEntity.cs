using System;
using InGame.Effects;
using UnityEngine;

namespace InGame
{
    public class TowerEntity : MonoBehaviour, IDamageable
    {
        [SerializeField] private GameObject towerRange;

        public int MaxHp { get; private set; }
        public int CurrentHp { get; private set; }
        public bool IsDestroyed { get; set; }
        
        public Action<TowerEntity> OnDestroyed;
        
        public void Initialize(int hp, float radius)
        {
            MaxHp = hp;
            CurrentHp = MaxHp;
            IsDestroyed = false;

            OnDestroyed = null;
            
            // Update UI
            towerRange.transform.localScale = 2 * radius * Vector3.one;
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