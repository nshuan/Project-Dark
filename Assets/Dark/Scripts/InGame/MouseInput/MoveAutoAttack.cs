using System;
using System.Collections.Generic;
using System.Linq;
using Dark.Scripts.Utils;
using DG.Tweening;
using UnityEngine;

namespace InGame
{
    [Serializable]
    public class MoveAutoAttack : IMouseInput
    {
        private InputInGame InputManager { get; set; }

        protected Camera Cam { get; set; }
        protected MonoCursor cursor;
        protected RectTransform cursorRect;
        // protected Vector3 mousePosition;
        protected Vector3 worldMousePosition;
        
        public bool CanShoot { get; set; }
        protected float Cooldown { get; set; }
        protected float ActivateDuration { get; set; } = 1f;
        protected float cdCounter;

        private LevelManager levelManager;
        private EnemyManager manager;
        private EnemyEntity nearestEnemy;
        private EnemyEntity forceTargetEnemy;
        private EnemyEntity hoveringEnemy;
        private Collider2D[] mouseHoverEnemies;
        
        public MoveAutoAttack()
        {
            
        }

        public MoveAutoAttack(Camera cam, MonoCursor cursor)
        {
            Cam = cam;
            this.cursor = cursor;
            cursorRect = cursor.GetComponent<RectTransform>();
            levelManager = LevelManager.Instance;
            manager = EnemyManager.Instance;

            mouseHoverEnemies = new Collider2D[20];
        }

        public void Initialize(InputInGame manager, MoveChargeController chargeController)
        {
            CanShoot = GameConst.DefaultAutoAttack;
            cursor.SetAuto(GameConst.DefaultAutoAttack);

            InputManager = manager;
            Cooldown = LevelUtilityV2.GetNormalAttackCooldown();
            ActivateDuration = 1f;
        }

        public virtual void OnMouseClick()
        {
            forceTargetEnemy = hoveringEnemy;
            hoveringEnemy?.SetHover(false);
        }
        
        private void AutoAttack()
        {
            if (!CanShoot) return;
            
            var tempMousePos = new Vector2(worldMousePosition.x, worldMousePosition.y);
            var (damage, criticalDamage) = LevelUtilityV2.GetNormalAttackDamage();
            var critRate = LevelUtilityV2.GetBaseCriticalRate();
            var bulletNum = 1;
            var skillSize = 1f;
            var skillRange = LevelUtilityV2.GetNormalAttackRange(Vector2.right);
            var maxHit = 1;
            if (LevelUtilityV2.BonusInfo.bonusUnlockSkill.unlockNormalAttackPiercing) 
                maxHit += LevelUtilityV2.GetNormalPiercingAmount();
            var stagger = LevelUtilityV2.GetBaseStagger();
            
            var delayShot = InputManager.PlayerVisual.PlayShoot(worldMousePosition);
            var targetEnemy = nearestEnemy;
            var activateSplitBullets = 0;
            if (LevelUtilityV2.BonusInfo.bonusUnlockSkill.unlockNormalAttackBullet) 
                activateSplitBullets = LevelUtilityV2.GetNormalBulletAmount();
            var activateActions = activateSplitBullets == 0
                ? null
                : new List<IProjectileActivate>()
                {
                    new ProjectileActivateSplit()
                    {
                        projectile = LevelUtilityV2.StatsNormalAttack.projectiles[PlayerProjectileType.Normal],
                        amount = activateSplitBullets,
                        angle = LevelUtilityV2.StatsNormalBullet.GetNormalBulletSpanAngle(activateSplitBullets + 1)
                    }
                };
            InputManager.DelayCall(delayShot, () =>
            {
                InputManager.PlayerVisual.Weapon.GetAllEnemiesInRange(skillRange);
                
                LevelUtilityV2.StatsNormalAttack.ShootToTarget(
                    LevelUtilityV2.StatsNormalAttack.projectiles[PlayerProjectileType.Normal],
                    targetEnemy,
                    InputManager.ProjectileSpawnPos.position,
                    LevelManager.Instance.CurrentTower.GetBaseCenter(),
                    tempMousePos,
                    damage,
                    bulletNum,
                    skillSize,
                    skillRange,
                    criticalDamage,
                    critRate,
                    stagger,
                    maxHit,
                    false,
                    activateActions,
                    null);
            });

            CombatActions.OnAttackNormal?.Invoke(Cooldown);
            
            cdCounter = Cooldown;
            
            // Do cursor effect
            cursor.UpdateScale(0f);
            cursor.UpdateChargeUnitAdd(false);
            cursor.UpdateCooldown(false, 0f);
            DOTween.Complete(this);
            var seq = DOTween.Sequence(this);
            seq.Append(cursor.contentAimAndMove.transform.DOPunchScale(0.3f * Vector3.one, 0.13f).SetEase(Ease.InQuad))
                .Join(cursor.visual.DOFade(0.3f, 0.13f).SetEase(Ease.InQuad).SetLoops(2, LoopType.Yoyo))
                .Join(DOTween.To(() => cursor.content.transform.localScale.x - 1f, x =>
                {
                    cursor.UpdateScale(x);
                }, 0f, 0.13f));
            seq.Play().OnComplete(() => cursor.UpdateCooldown(false, 0f));
        }

        public void OnHoldStarted()
        {
            CanShoot = false;
            cursor.SetAuto(false);
        }

        public void OnHoldReleased()
        {
            if (!CanShoot)
            {
                CanShoot = true;
                cdCounter = Cooldown;
                cursor.SetAuto(true);
                cursor.UpdateCooldown(false, 0f);
            }
        }

        public void ResetChargeVariable()
        {
            CanShoot = GameConst.DefaultAutoAttack;
            cursor.SetAuto(GameConst.DefaultAutoAttack);
        }

        public bool CanCharge => false;

        public bool CanMove => true;

        public virtual void OnUpdate(Vector2 worldMousePosition)
        {
            if (cdCounter >= 0) cdCounter -= Time.deltaTime;

            if (!CanShoot)
            {
                InputManager.PlayerVisual.SetDirection(worldMousePosition);
                if (nearestEnemy) nearestEnemy.SetAimed(false);
                forceTargetEnemy?.SetAimed(false);
                return;
            }
            
            // Check bấm vào enemy để force attack vào đó
            var mouseOverCount = Physics2D.OverlapPointNonAlloc(worldMousePosition, mouseHoverEnemies, LayerMask.GetMask("EnemyAim"));
            if (mouseOverCount > 0)
            {
                hoveringEnemy?.SetHover(false);
                hoveringEnemy = null;
                for (var i = 0; i < mouseOverCount; i++)
                {
                    var entity = mouseHoverEnemies[i].GetComponentInParent<EnemyEntity>();
                    if (entity)
                    {
                        hoveringEnemy = entity;
                        hoveringEnemy.SetHover(true);
                        break;
                    }
                }
            }
            else
            {
                hoveringEnemy?.SetHover(false);
                hoveringEnemy = null;
            }
                

            // Nếu auto target thì check enemy gần nhất rồi target vào
            var canAutoTarget = GetNearestEnemy();
            if (canAutoTarget)
            {
                this.worldMousePosition.x = nearestEnemy.transform.position.x;
                this.worldMousePosition.y = nearestEnemy.transform.position.y;
            }
            else
            {
                this.worldMousePosition.x = worldMousePosition.x;
                this.worldMousePosition.y = worldMousePosition.y; 
            }
            
            InputManager.PlayerVisual.SetDirection(this.worldMousePosition);
            
            var mousePosition = Input.mousePosition;
            mousePosition.z = 0; // Set z to 0 for 2D
            cursorRect.position = mousePosition;    
            
            // cursor.UpdateCooldown(true, 1 - Mathf.Clamp(cdCounter / Cooldown, 0f, 1f));
            if (cdCounter <= 0 && canAutoTarget)
                AutoAttack();
        }

        public void Deactivate()
        {
            
        }

        public virtual void OnDrawGizmos()
        {
            
        }

        public void Dispose()
        {
            
        }
        
        private bool GetNearestEnemy()
        {
            if (forceTargetEnemy && !forceTargetEnemy.IsDestroyed)
            {
                nearestEnemy?.SetAimed(false);
                nearestEnemy = forceTargetEnemy;
                nearestEnemy.SetAimed(true);
                return nearestEnemy;
            }
            
            var nearestDistance = float.MaxValue;
            EnemyEntity tempNearestEnemy = null;
                 
            foreach (var enemy in manager.Enemies)
            {
                if (enemy.Value.gameObject.activeInHierarchy && enemy.Value.Activated && enemy.Value.IsDestroyed == false)
                {
                    var direction = enemy.Value.transform.position - levelManager.CurrentTower.GetBaseCenter();
                    var distance = direction.magnitude;
                    if (distance > LevelUtilityV2.GetNormalAttackRange(direction))
                        continue;
                    
                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        tempNearestEnemy = enemy.Value;
                    }
                }
            }

            if (nearestEnemy &&  tempNearestEnemy != nearestEnemy) nearestEnemy.SetAimed(false);
            if (!tempNearestEnemy) return false;
            nearestEnemy = tempNearestEnemy;
            nearestEnemy.SetAimed(true);
            
            return true;
        }
    }
}