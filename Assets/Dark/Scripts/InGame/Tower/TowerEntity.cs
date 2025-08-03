using System;
using InGame.Effects;
using UnityEngine;
using UnityEngine.UI;

namespace InGame
{
    public class TowerEntity : MonoBehaviour, IDamageable
    {
        [SerializeField] public Vector3 standOffset;
        [SerializeField] private SpriteRenderer towerVisual;
        [SerializeField] private Sprite[] spriteStates;
        [SerializeField] private float[] thresholdState = new[] { 0f, 0.3f, 0.7f };

        private int currentState;
        
        public int Id { get; private set; }
        public int MaxHp { get; private set; }
        public int CurrentHp { get; private set; }
        public bool IsDestroyed { get; set; }
        
        public Action<int> OnHit { get; set; }
        public Action<Vector2> OnHitAttackerPos { get; set; }
        public Action<TowerEntity> OnDestroyed;
        
        private FlashColor damageEffect;
        
        public void Initialize(int id, int hp)
        {
            Id = id;
            MaxHp = hp;
            CurrentHp = MaxHp;
            IsDestroyed = false;

            OnDestroyed = null;
            damageEffect = new FlashColor() 
            {
                SpriteRendererTarget = towerVisual,
                FlashDuration = 0.1f,
                Color = new Color(1f, 0.6f, 0.6f, 1f)
            };
            currentState = spriteStates.Length - 1;
            towerVisual.sprite = spriteStates[currentState];
        }

        public void EnterTower()
        {
            selected.gameObject.SetActive(true);
            fxselected.Play();
        }

        public void LeaveTower()
        {
            selected.gameObject.SetActive(false);
            fxselected.Stop();
        }

        public float HitDirectionX { get; set; }
        public float HitDirectionY { get; set; }

        public void Damage(int damage, Vector2 dealerPosition, float stagger)
        {
            if (IsDestroyed) return;
            
            stagger = 0;
            CurrentHp -= damage;
            
            OnHit?.Invoke(damage);
            OnHitAttackerPos?.Invoke(dealerPosition);
            if (CurrentHp <= 0)
            {
                IsDestroyed = true;
                OnDestroyed?.Invoke(this);
            }
            else
            {
                if ((float)CurrentHp / MaxHp < thresholdState[currentState])
                {
                    currentState -= 1;
                    towerVisual.sprite = spriteStates[currentState];
                }
            }
            
            // Do damage effect
            VisualEffectHelper.Instance.PlayEffect(damageEffect);
        }

        [SerializeField] private SpriteRenderer selected;
        [SerializeField] private ParticleSystem fxselected;
        [SerializeField] private SpriteRenderer hover;
        [SerializeField] private ParticleSystem fxhover;
        public void Hover(bool hovering)
        {
            hover.gameObject.SetActive(hovering);
            fxhover.Play();
        }

        [Space]
        [SerializeField] private string highestSortingLayer;
        [SerializeField] private string defaultSortingLayer;
        public void SetHighestSortingLayer()
        {
            towerVisual.sortingLayerName = highestSortingLayer;
            selected.sortingLayerName = highestSortingLayer;
            hover.sortingLayerName = highestSortingLayer;
        }

        public void ResetSortingLayer()
        {
            towerVisual.sortingLayerName = defaultSortingLayer;
            selected.sortingLayerName = defaultSortingLayer;
            hover.sortingLayerName = defaultSortingLayer;
        }
    }
}