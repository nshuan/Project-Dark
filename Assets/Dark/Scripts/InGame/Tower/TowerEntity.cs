using System;
using Dark.Scripts.AudioV2;
using DG.Tweening;
using InGame.Shield;
using InGame.UI;
using UnityEngine;

namespace InGame
{
    public class TowerEntity : MonoBehaviour, IDamageable
    {
        [SerializeField] private Vector3[] standOffset;
        [SerializeField] private TowerAnim towerAnim;
        [SerializeField] private TowerAnim towerBaseAnim;
        [SerializeField] private MeshRenderer towerMesh;
        [SerializeField] private MeshRenderer towerBaseMesh;
        // [SerializeField] private TowerAnim towerVisualUILayer;
        [SerializeField] private Sprite[] spriteStates;
        [SerializeField] private float[] thresholdState = new[] { 0f, 0.3f, 0.7f };
        [SerializeField] private ParticleSystem vfxExplode;
        [SerializeField] private TowerAutoRegenerate autoRegenerate;
        [SerializeField] private TowerRegenerateOnKill regenerateOnKill;
        public Transform[] itemCollectorPositions;
        [SerializeField] private AudioPlayComponentV2 sfxHit;
        [SerializeField] private Transform flashSize;

        [Space] [Header("Shield")] 
        [SerializeField] public TowerShield shield;
        
        [Space] [Header("Config")]
        [SerializeField] private string normalSortingLayerName;
        [SerializeField] private int normalSortingOrder;
        [SerializeField] private string hoverSortingLayerName;
        [SerializeField] private int hoverSortingOrder;
        [SerializeField] private string normalBaseSortingLayerName;
        [SerializeField] private int normalBaseSortingOrder;

        private int currentState;
        public int CurrentState => currentState;
        
        public int Id { get; private set; }
        public int MaxHp { get; private set; }
        public int CurrentHp { get; private set; }
        public bool IsDestroyed { get; set; }
        
        public Action<int, DamageType> OnHit { get; set; }
        public Action<int, DamageType> OnHitShield { get; set; }
        public Action<int> OnRegenerate { get; set; }
        public Action<Vector2> OnHitAttackerPos { get; set; }
        public Action<TowerEntity> OnDestroyed;
        private bool isSelecting;
        
        public void Initialize(int id, int hp)
        {
            Id = id;
            MaxHp = hp;
            CurrentHp = MaxHp;
            shield.Initialize();
            IsDestroyed = false;

            OnDestroyed = null;
            currentState = 3; // 3 trạng thái máu và 1 trạng thái vỡ
            towerAnim.PlayIdle(currentState);
            towerBaseAnim.PlayIdle(currentState);
            towerAnim.SetActiveOutline(false);
            towerBaseAnim.SetActiveOutline(false);
            autoRegenerate.Initialize(this, LevelUtilityV2.GetBaseRegen());
            regenerateOnKill.Initialize(this, LevelUtilityV2.GetBaseLifeLeech());
        }
        
        public void EnterTower()
        {
            isSelecting = true;
            selected.SetActive(true);
        }

        public void LeaveTower()
        {
            isSelecting = false;
            selected.SetActive(false);
        }

        public float HitDirectionX { get; set; }
        public float HitDirectionY { get; set; }

        public void Damage(int damage, Vector2 dealerPosition, float stagger, DamageType dmgType)
        {
            if (IsDestroyed) return;
            
            // Reduce shield before applying to tower health
            var totalDamage = damage;
            damage = shield.Damage(damage);
            OnHitShield?.Invoke(totalDamage - damage, dmgType);
            
            stagger = 0;
            var lastHealth = CurrentHp;
            CurrentHp -= damage;
            CombatActions.OnDamageReceived?.Invoke(lastHealth - CurrentHp);
            
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
                    vfxExplode?.Play(true);
                    towerAnim.TransitionToIdle(currentState, true);
                    towerBaseAnim.TransitionToIdle(currentState, true);
                }
                
                if (currentState == 1) UIWarningManager.Instance.WarnOnce(false);
            }
            
            autoRegenerate.Activate();
            
            // Do damage effect
            sfxHit.Play();
            towerAnim.PlayHit();
            towerBaseAnim.PlayHit();
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
                towerBaseAnim.TransitionToIdle(currentState, false);
            }
        }
        
        [SerializeField] private GameObject selected;
        // [SerializeField] private GameObject hover;
        public void Hover(bool hovering, bool showUILayerOnHovering = true)
        {
            if (hovering && showUILayerOnHovering)
            {
                towerMesh.sortingLayerName = hoverSortingLayerName;
                towerMesh.sortingOrder = hoverSortingOrder;
                towerBaseMesh.sortingLayerName = hoverSortingLayerName;
                towerBaseMesh.sortingOrder = hoverSortingOrder - 1;
            }
            else
            {
                towerMesh.sortingLayerName = normalSortingLayerName;
                towerMesh.sortingOrder = normalSortingOrder;
                towerBaseMesh.sortingLayerName = normalBaseSortingLayerName;
                towerBaseMesh.sortingOrder = normalBaseSortingOrder;
            }

            if (hovering)
            {
                towerAnim.PlayHover(currentState);
                towerBaseAnim.PlayHover(currentState);
            }
            else
            {
                towerAnim.PlayIdle(currentState);
                towerBaseAnim.PlayIdle(currentState);
            }
            towerAnim.SetActiveOutline(hovering);
            towerBaseAnim.SetActiveOutline(hovering);
        }

        public void ShowFlashSize(bool show, float size)
        {
            if (!show)
            {
                DOTween.Kill(flashSize.transform);
                flashSize.gameObject.SetActive(false);
            }
            else
            {
                DOTween.Kill(flashSize.transform);
                flashSize.transform.localScale = Vector3.zero;
                flashSize.gameObject.SetActive(true);
                flashSize.transform.DOScale(size, 0.25f).SetEase(Ease.OutQuad)
                    .SetUpdate(true).SetTarget(flashSize.transform);
            }
        }
        
        public void OnMotionBlur()
        {
            // towerVisualUILayer.PlayIdle(currentState);
            // towerVisualUILayer.gameObject.SetActive(true);
        }

        public void OnEndMotionBlur()
        {
            // towerVisualUILayer.gameObject.SetActive(false);
            // towerVisualUILayer.SetActiveOutline(false);
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