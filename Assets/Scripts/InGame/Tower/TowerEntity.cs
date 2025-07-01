using System;
using InGame.Effects;
using UnityEngine;

namespace InGame
{
    public class TowerEntity : MonoBehaviour, IDamageable
    {
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
        }
        
        public void EnterTower()
        {
          
        }

        public void LeaveTower()
        {
            
        }

        public void Damage(int damage, Vector2 damageDirection, float stagger)
        {
            if (IsDestroyed) return;
            stagger = 0;
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