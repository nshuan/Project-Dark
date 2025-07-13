using System;
using InGame.Effects;
using UnityEngine;

namespace InGame
{
    public class TowerEntity : MonoBehaviour, IDamageable
    {
        [SerializeField] public Vector3 standOffset;
        [SerializeField] private SpriteRenderer towerVisual;
        
        public int Id { get; private set; }
        public int MaxHp { get; private set; }
        public int CurrentHp { get; private set; }
        public bool IsDestroyed { get; set; }
        
        public Action<int> OnHit { get; set; }
        public Action<TowerEntity> OnDestroyed;
        
        public void Initialize(int id, int hp)
        {
            Id = id;
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
            OnHit?.Invoke(damage);
            if (CurrentHp <= 0)
            {
                IsDestroyed = true;
                OnDestroyed?.Invoke(this);
            }
            
            // Do damage effect
            var flashRed = new FlashColor()
            {
                SpriteRendererTarget = towerVisual,
                FlashDuration = 0.1f,
                Color = Color.red
            };
            EffectHelper.Instance.PlayEffect(flashRed);
        }

        [SerializeField] private GameObject objHighlight;
        public void Highlight(bool highlighted)
        {
            objHighlight.SetActive(highlighted);
        }

        [SerializeField] private GameObject objHover;
        public void Hover(bool hovering)
        {
            objHover.SetActive(hovering);
        }
    }
}