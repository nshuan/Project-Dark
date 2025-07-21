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
        public Action<Vector2> OnHitAttackerPos { get; set; }
        public Action<TowerEntity> OnDestroyed;

        private Vector3 hitDirection = new Vector3();
        
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
            selected.SetActive(true);
        }

        public void LeaveTower()
        {
            selected.SetActive(false);
        }

        public void Damage(int damage, Vector2 attackerPos, float stagger)
        {
            if (IsDestroyed) return;
            
            stagger = 0;
            CurrentHp -= damage;
            hitDirection.x = transform.position.x - attackerPos.x;
            hitDirection.y = transform.position.y - attackerPos.y;
            hitDirection.x /= hitDirection.magnitude;
            hitDirection.y /= hitDirection.magnitude;
            
            OnHit?.Invoke(damage);
            OnHitAttackerPos?.Invoke(attackerPos);
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
                Color = new Color(1f, 0.6f, 0.6f, 1f)
            };
            VisualEffectHelper.Instance.PlayEffect(flashRed);
        }

        [SerializeField] private GameObject selected;
        [SerializeField] private GameObject objHover;
        public void Hover(bool hovering)
        {
            objHover.SetActive(hovering);
        }
    }
}