using System;
using System.Collections;
using Dark.Scripts.Audio;
using InGame.Effects;
using InGame.UI;
using UnityEngine;
using UnityEngine.UI;

namespace InGame
{
    public class TowerEntity : MonoBehaviour, IDamageable
    {
        [SerializeField] private Vector3[] standOffset;
        [SerializeField] private TowerAnim towerAnim;
        [SerializeField] private SpriteRenderer towerVisualUILayer;
        [SerializeField] private SpriteRenderer towerOutline;
        [SerializeField] private Sprite[] spriteStates;
        [SerializeField] private float[] thresholdState = new[] { 0f, 0.3f, 0.7f };
        [SerializeField] private TowerAutoRegenerate autoRegenerate;
        [SerializeField] private TowerRegenerateOnKill regenerateOnKill;
        [SerializeField] private AudioComponent sfxHit;

        private int currentState;
        
        public int Id { get; private set; }
        public int MaxHp { get; private set; }
        public int CurrentHp { get; private set; }
        public bool IsDestroyed { get; set; }
        
        public Action<int, DamageType> OnHit { get; set; }
        public Action<int> OnRegenerate { get; set; }
        public Action<Vector2> OnHitAttackerPos { get; set; }
        public Action<TowerEntity> OnDestroyed;
        
        public void Initialize(int id, int hp)
        {
            Id = id;
            MaxHp = hp;
            CurrentHp = MaxHp;
            IsDestroyed = false;

            OnDestroyed = null;
            currentState = 3; // 3 trạng thái máu và 1 trạng thái vỡ
            towerAnim.PlayIdle(currentState);
            towerVisualUILayer.sprite = spriteStates[currentState];
            towerOutline.sprite = spriteStates[currentState];
            autoRegenerate.Initialize(this, LevelUtility.GetTowerAutoRegen(MaxHp));
            regenerateOnKill.Initialize(this, LevelUtility.GetTowerRegenOnKill(MaxHp));
        }

        public void EnterTower()
        {
            selected.SetActive(true);
        }

        public void LeaveTower()
        {
            selected.SetActive(false);
        }

        public float HitDirectionX { get; set; }
        public float HitDirectionY { get; set; }

        public void Damage(int damage, Vector2 dealerPosition, float stagger, DamageType dmgType)
        {
            if (IsDestroyed) return;
            
            stagger = 0;
            CurrentHp -= damage;
            
            OnHit?.Invoke(damage, dmgType);
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
                    towerAnim.TransitionToIdle(currentState, true);
                    towerVisualUILayer.sprite = spriteStates[currentState];
                    towerOutline.sprite = spriteStates[currentState];
                }
                
                if (currentState == 1) UIWarningManager.Instance.WarnOnce(false);
            }
            
            autoRegenerate.Activate();
            
            // Do damage effect
            sfxHit.Play();
            towerAnim.PlayHit();
        }

        public void Regenerate(int value)
        {
            if (IsDestroyed) return;
            if (value <= 0) return;

            CurrentHp += value;
            OnRegenerate?.Invoke(value);
            
            if (currentState < thresholdState.Length - 1 && (float)CurrentHp / MaxHp >= thresholdState[currentState + 1])
            {
                currentState += 1;
                towerAnim.TransitionToIdle(currentState, false);
                towerVisualUILayer.sprite = spriteStates[currentState];
                towerOutline.sprite = spriteStates[currentState];
            }
        }
        
        [SerializeField] private GameObject selected;
        [SerializeField] private GameObject hover;
        public void Hover(bool hovering)
        {
            hover.SetActive(hovering);
            towerOutline.gameObject.SetActive(hovering);
        }
        
        public void OnMotionBlur()
        {
            towerVisualUILayer.gameObject.SetActive(true);
        }

        public void OnEndMotionBlur()
        {
            towerVisualUILayer.gameObject.SetActive(false);
            towerOutline.gameObject.SetActive(false);
        }

        /// <summary>
        /// Get the true center of the base of the tower
        /// the "towerVisual" should have the pivot on that center, then we only need to get the position of that
        /// </summary>
        /// <returns></returns>
        public Vector3 GetBaseCenter()
        {
            return towerAnim.transform.position;
        }

        public Vector3 GetTowerHeight()
        {
            return standOffset[currentState];
        }
    }
}