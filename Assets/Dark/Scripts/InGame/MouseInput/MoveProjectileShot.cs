using System;
using System.Collections.Generic;
using System.Linq;
using Dark.Scripts.Utils;
using Data;
using DG.Tweening;
using UnityEngine;
namespace InGame
{
    [Serializable]
    public class MoveProjectileShot : IMouseInput
    {
        private PlayerCharacter Character { get; set; }

        protected Camera Cam { get; set; }
        protected MonoCursor cursor;
        protected RectTransform cursorRect;
        protected Vector3 mousePosition;
        protected Vector3 worldMousePosition;
        
        public MoveChargeController ChargeController { get; set; }
        public bool CanShootNormal { get; set; }
        public bool CanShootCharge { get; set; }
        protected float CooldownNormal { get; set; }
        protected float CooldownCharge { get; set; }
        protected float cdCounterNormal;
        protected float cdCounterCharge;

        private CharacterClass.CharacterClass classType;
        
        #region Charge

        private bool canChargeBullet;
        private int bulletChargeAdded;
        private float bulletPerStep;
        private int bulletChargeMaxStep;

        private bool canChargeDame;
        private float dameChargeAdded;
        private float damePerStep;
        private int dameChargeMaxStep;

        private bool canChargeSize;
        private float sizeChargeAdded;
        private float sizePerStep;
        private int sizeChargeMaxStep;

        private bool canChargeRange;
        private float rangeChargeAdded;
        private float rangePerStep;
        private int rangeChargeMaxStep;

        private bool isCharging;
        private float chargeTimer;
        private float chargeStepTime;
        private int chargeStep;
        private int chargeMaxStep;
        #endregion

        private bool hasSetupCharge;

        public MoveProjectileShot()
        {

        }

        public MoveProjectileShot(Camera cam, MonoCursor cursor)
        {
            Cam = cam;
            this.cursor = cursor;
            cursorRect = cursor.GetComponent<RectTransform>();
        }

        public void Initialize(PlayerCharacter character, MoveChargeController chargeController)
        {
            classType = PlayerDataManager.Instance.Data.Class;
            
            Character = character;
            ChargeController = chargeController;
            CooldownNormal = LevelUtilityV2.GetNormalAttackCooldown();
            CooldownCharge = LevelUtilityV2.GetChargeAttackCooldown();
            
            canChargeBullet = LevelUtilityV2.BonusInfo.bonusUnlockSkill.unlockChargeAttackBullet;
            canChargeSize = LevelUtilityV2.BonusInfo.bonusUnlockSkill.unlockChargeAttackSize;
            canChargeDame = canChargeSize || canChargeBullet;
            canChargeRange = canChargeSize || canChargeBullet;
            
            ChargeController.SetProjectile(LevelUtilityV2.StatsNormalAttack.projectiles[PlayerProjectileType.ChargeBullet]);
            ChargeController.Cam = Cam;
            
            // Setup shot radius
            Character.ShowShotRadius(
                LevelManager.Instance.CurrentTower.GetBaseCenter(),
                LevelUtilityV2.GetNormalAttackRange(Vector2.right));
        }
        
        public virtual void OnMouseClick()
        {
            var isCharge = (canChargeBullet && bulletChargeAdded > 0) || (canChargeDame && dameChargeAdded > 0) ||
                           (canChargeSize && sizeChargeAdded > 0) || (canChargeRange && rangeChargeAdded > 0);
            
            if (!isCharge && !CanShootNormal) return;
            if (isCharge && !CanShootCharge) return;

            if (isCharge) CanShootCharge = false;
            else CanShootNormal = false;
            
            var tempMousePos = Cam.ScreenToWorldPoint(mousePosition);
            var (damage, criticalDamage) = LevelUtilityV2.GetChargeAttackDamage(
                canChargeDame && dameChargeAdded > 0 ? 1 + dameChargeAdded : 1f);
            var critRate = LevelUtilityV2.GetBaseCriticalRate();
            var bulletNum = 1;
            var skillSize = (canChargeSize && sizeChargeAdded > 0 ? 1 + sizeChargeAdded : 1f) * LevelUtilityV2.GetNormalAttackSize();
            var skillRange = (canChargeRange && rangeChargeAdded > 0 ? 1 + rangeChargeAdded : 1f) * LevelUtilityV2.GetNormalAttackRange(Vector2.right);
            var maxHit = 1;
            if (LevelUtilityV2.BonusInfo.bonusUnlockSkill.unlockNormalAttackPiercing) 
                maxHit += LevelUtilityV2.GetNormalPiercingAmount();
            var stagger = LevelUtilityV2.GetBaseStagger();

            InputInGame.BlockTeleport = true;
            var delayShot = 0f;
            if (isCharge)
            {
                // Nếu bắn charge thì maxHit = 1 thôi
                maxHit = 1;
                delayShot = 0f;
                Character.EndChargeAndShoot();
            }
            else
            {
                delayShot = Character.PlayShoot(worldMousePosition);
            }
            
            Character.DelayCall(delayShot, () =>
            {
                var isChargeBullet = canChargeBullet && bulletChargeAdded > 0;
                var isChargeSize = canChargeSize && sizeChargeAdded > 0;

                var projectileType = PlayerProjectileType.Normal;
                if (isChargeBullet)
                {
                    projectileType = isChargeSize ? PlayerProjectileType.ChargeBulletSize : PlayerProjectileType.ChargeBullet;
                }
                else
                {
                    if (isChargeSize) projectileType = PlayerProjectileType.ChargeSize;
                }

                if (!isChargeBullet)
                {
                    var blossomAmount = 0;
                    if (canChargeSize)
                        blossomAmount = LevelUtilityV2.GetChargeSizeAmount();
                    var blossomAction = blossomAmount == 0
                        ? null
                        : new List<IProjectileHit>()
                        {
                            new ProjectileHitBlossom()
                            {
                                projectile = LevelUtilityV2.StatsNormalAttack.projectiles[PlayerProjectileType.ChargeSizeSubBullet],
                                bulletAmount = blossomAmount,
                                blossomSize = classType == CharacterClass.CharacterClass.Knight ? skillRange * LevelUtilityV2.StatsChargeSize.range : LevelUtilityV2.StatsChargeSize.range
                            }
                        };
                    LevelUtilityV2.StatsNormalAttack.Shoot(
                        LevelUtilityV2.StatsNormalAttack.projectiles[projectileType],
                        Character.transform.position,
                        LevelManager.Instance.CurrentTower.GetBaseCenter(),
                        tempMousePos,
                        damage,
                        isCharge ? 1 : bulletNum,
                        skillSize,
                        skillRange,
                        criticalDamage,
                        critRate,
                        stagger,
                        maxHit,
                        isCharge,
                        null,
                        blossomAction
                        );
                }

                if (isCharge)
                {
                    var chargeRange = (canChargeRange && rangeChargeAdded > 0 ? 1 + rangeChargeAdded : 1f) *
                                      LevelUtilityV2.GetNormalAttackRange(Vector2.right);
                    // Không check trong range charge nữa, check trên toàn map luôn
                    Character.Weapon.GetAllEnemiesInRange(15f); 
                    
                    // Check enemy in range
                    var nearestDistance = float.MaxValue;
                    EnemyEntity tempNearestEnemy = null;
                 
                    foreach (var enemy in EnemyManager.Instance.Enemies)
                    {
                        if (enemy.Value.gameObject.activeInHierarchy && enemy.Value.Activated && enemy.Value.IsDestroyed == false)
                        {
                            var direction = enemy.Value.transform.position - LevelManager.Instance.CurrentTower.GetBaseCenter();
                            var distance = direction.magnitude;
                            if (distance > skillRange)
                                continue;
                    
                            if (distance < nearestDistance)
                            {
                                nearestDistance = distance;
                                tempNearestEnemy = enemy.Value;
                            }
                        }
                    }

                    if (tempNearestEnemy)
                    {
                        Character.SetDirection(tempNearestEnemy.transform.position);
                        ChargeController.ForceDirection =
                            tempNearestEnemy.transform.position - Character.transform.position;
                        ChargeController.UseForceDirection = true;
                    }
                    
                    ChargeController.Attack((projectile, direction, delay) =>
                    {
                        var blossomAmount = 0;
                        if (canChargeSize)
                            blossomAmount = LevelUtilityV2.GetChargeSizeAmount();
                        var blossomAction = blossomAmount == 0
                            ? null
                            : new List<IProjectileHit>()
                            {
                                new ProjectileHitBlossom()
                                {
                                    projectile = LevelUtilityV2.StatsNormalAttack.projectiles[PlayerProjectileType.ChargeSizeSubBullet],
                                    bulletAmount = blossomAmount,
                                    blossomSize = classType == CharacterClass.CharacterClass.Knight ? skillRange * LevelUtilityV2.StatsChargeSize.range : LevelUtilityV2.StatsChargeSize.range
                                }
                            };
                        
                        projectile.Init(
                            LevelManager.Instance.CurrentTower.GetBaseCenter(), 
                            direction.normalized, 
                            chargeRange,
                            skillSize, 
                            LevelUtilityV2.StatsNormalAttack.speedScale,
                            damage,
                            criticalDamage, 
                            critRate, 
                            stagger, 
                            true, 
                            maxHit, 
                            null, 
                            blossomAction,
                            ProjectileType.PlayerProjectile);
                        
                        projectile.Activate(delay);
                    });

                    ChargeController.UseForceDirection = false;
                }

                InputInGame.BlockTeleport = false;
            });

            CooldownNormal = LevelUtilityV2.GetNormalAttackCooldown();
            CooldownCharge = LevelUtilityV2.GetChargeAttackCooldown();

            if (isCharge)
            {
                CombatActions.OnAttackCharge?.Invoke(CooldownCharge);
                cdCounterCharge = CooldownCharge;
                cdCounterCharge += delayShot;
            }
            else
            {
                CombatActions.OnAttackNormal?.Invoke(CooldownNormal);
                cdCounterNormal = CooldownNormal;
                cdCounterNormal += delayShot;
            }
            

            // Reset range
            Character.UpdateShotRadius(
                LevelUtilityV2.GetNormalAttackRange(Vector2.right), false);
            
            // Do cursor effect
            cursor.UpdateScale(0f);
            cursor.UpdateChargeUnitAdd(false);
            cursor.UpdateCooldown(false, 0f);
            DOTween.Complete(this);
            var seq = DOTween.Sequence(this);
            seq.Append(cursor.transform.DOPunchScale(0.3f * Vector3.one, 0.13f).SetEase(Ease.InQuad))
                .Join(cursor.visual.DOFade(0.3f, 0.13f).SetEase(Ease.InQuad).SetLoops(2, LoopType.Yoyo))
                .Join(DOTween.To(() => cursor.content.transform.localScale.x - 1f, x =>
                {
                    cursor.UpdateScale(x);
                }, 0f, 0.13f));
            seq.Play().OnComplete(() => cursor.UpdateCooldown(false, 0f));
        }

        public void OnHoldStarted()
        {
            // if (!CanShootCharge) return;
            if (isCharging) return; 
            
            ResetChargeVariable();

            if (canChargeBullet && bulletChargeMaxStep > 0) isCharging = true;
            else if (canChargeDame && dameChargeMaxStep > 0) isCharging = true;
            else if (canChargeSize && sizeChargeMaxStep > 0) isCharging = true;
            else if (canChargeRange && rangeChargeMaxStep > 0) isCharging = true;

            if (isCharging)
            {
                CombatActions.OnChargeStarted?.Invoke();
                hasSetupCharge = false;
            }
        }

        public void OnHoldReleased()
        {
            if (isCharging)
                CombatActions.OnChargeEnded?.Invoke();
            isCharging = false;
            Character.EndChargeAndShoot();
        }

        public void ResetChargeVariable()
        {
            chargeStepTime = LevelUtilityV2.GetChargeStepTime();
            
            bulletChargeAdded = 0;
            bulletPerStep = LevelUtilityV2.StatsNormalAttack.chargeBulletStep;
            bulletChargeMaxStep = Math.Max(LevelUtilityV2.StatsNormalAttack.chargeBulletMaxStep, Mathf.RoundToInt(LevelUtilityV2.GetChargeBulletAmount() / bulletPerStep));

            dameChargeAdded = 0f;
            damePerStep = LevelUtilityV2.StatsNormalAttack.chargeDameStep;
            dameChargeMaxStep = LevelUtilityV2.StatsNormalAttack.chargeDameMaxStep;

            sizeChargeAdded = 0f;
            sizePerStep = LevelUtilityV2.StatsNormalAttack.chargeSizeStep;
            sizeChargeMaxStep = LevelUtilityV2.StatsNormalAttack.chargeSizeMaxStep;
            
            rangeChargeAdded = 0f;
            rangePerStep = LevelUtilityV2.StatsNormalAttack.chargeRangeStep;
            rangeChargeMaxStep = LevelUtilityV2.StatsNormalAttack.chargeRangeMaxStep;

            isCharging = false;
            chargeStep = 0;
            chargeTimer = 0f;
            chargeMaxStep = Mathf.Max(bulletChargeMaxStep, dameChargeMaxStep, sizeChargeMaxStep, rangeChargeMaxStep);
        }

        public bool CanCharge => CanShootCharge && !isCharging && (
            (canChargeBullet && bulletChargeMaxStep > 0) ||
            (canChargeDame && dameChargeMaxStep > 0) ||
            (canChargeRange && rangeChargeMaxStep > 0) || 
            canChargeSize && sizeChargeMaxStep > 0
            );

        public bool CanMove => true;

        public virtual void OnUpdate(Vector2 worldMousePosition)
        {
            this.worldMousePosition.x = worldMousePosition.x;
            this.worldMousePosition.y = worldMousePosition.y;
            
            mousePosition = Input.mousePosition;
            mousePosition.z = 0; // Set z to 0 for 2D
            cursorRect.position = mousePosition;    
            
            // Cooldown if player can not shoot
            if (!CanShootNormal)
            {
                cdCounterNormal -= Time.deltaTime;
                if (cdCounterNormal <= 0)
                    CanShootNormal = !GameConst.DefaultAutoAttack;
            }
            
            if (!CanShootCharge)
            {
                cdCounterCharge -= Time.deltaTime;
                if (cdCounterCharge <= 0)
                {
                    CanShootCharge = true;
                    if (CanCharge) cursor.SetReadyToCharge();
                    CombatActions.OnChargeCooldownComplete?.Invoke();
                }
            }
            else
            {
                if (isCharging && chargeMaxStep > 0 && chargeStep < chargeMaxStep)
                {
                    if (hasSetupCharge == false)
                    {
                        hasSetupCharge = true;
                        InputInGame.OnChargeSetup?.Invoke();
                        Character.UpdateChargeScale(1f);
                        Character.PlayCharge();
                        cursor.UpdateScale(0f);
                        cursor.UpdateCooldown(false, 0f);
                    }
                    
                    if (chargeTimer < chargeStepTime)
                    {
                        chargeTimer += Time.deltaTime;
                        cursor.UpdateCooldown(true, 1 - Mathf.Clamp(chargeTimer / chargeStepTime, 0f, 1f));
                    }
                    else
                    {
                        chargeTimer -= chargeStepTime;
                        chargeStep += 1;
                        
                        cursor.UpdateScale(0f);
                        cursor.UpdateChargeUnitAdd(true, chargeStep);
                        cursor.transform.DOPunchScale(0.2f * Vector3.one, 0.13f).SetEase(Ease.InQuad)
                            .OnComplete(() => cursor.UpdateCooldown(true, 0f));

                        if (chargeStep >= chargeMaxStep)
                        {
                            cursor.UpdateMax();
                        }
                        
                        if (canChargeBullet && bulletChargeMaxStep > 0)
                        {
                            bulletChargeAdded = (int)(bulletPerStep * Math.Min(chargeStep, bulletChargeMaxStep));
                            bulletChargeAdded = Math.Min(bulletChargeAdded, LevelUtilityV2.GetChargeBulletAmount());
                            for (int i = 0; i < bulletChargeAdded - ChargeController.TotalBulletAdded; i++)
                            {
                                ChargeController.AddBullet(Character.transform.position,
                                    this.worldMousePosition - Character.transform.position);
                            }
                        }

                        if (canChargeDame && dameChargeMaxStep > 0)
                        {
                            dameChargeAdded = damePerStep * Math.Min(chargeStep, dameChargeMaxStep);
                        }

                        if (canChargeSize && sizeChargeMaxStep > 0)
                        {
                            sizeChargeAdded = sizePerStep * Math.Min(chargeStep, sizeChargeMaxStep);
                            ChargeController.AddSize(sizeChargeAdded > 0 ? 1 + sizeChargeAdded : 1f);
                            Character.UpdateChargeScale(1f + Math.Min(chargeStep, sizeChargeMaxStep) * 0.15f);
                        }

                        if (canChargeRange && rangeChargeMaxStep > 0)
                        {
                            rangeChargeAdded = rangePerStep * Math.Min(chargeStep, rangeChargeMaxStep);
                            Character.UpdateShotRadius(
                                (rangeChargeAdded > 0 ? 1 + rangeChargeAdded : 1f) * LevelUtilityV2.GetNormalAttackRange(Vector2.right));
                        }
                    }
                }
            }
            
#if UNITY_EDITOR
            var corners = new Vector3[4];
            cursorRect.GetWorldCorners(corners);
            corners = corners.Select((corner) => Cam.ScreenToWorldPoint(corner)).ToArray();
            
            // Draw lines between corners to visualize the box
            Debug.DrawLine(corners[0], corners[1], Color.red); // Bottom Left -> Top Left
            Debug.DrawLine(corners[1], corners[2], Color.red); // Top Left -> Top Right
            Debug.DrawLine(corners[2], corners[3], Color.red); // Top Right -> Bottom Right
            Debug.DrawLine(corners[3], corners[0], Color.red); // Bottom Right -> Bottom Left
            
            var ray = Cam.ScreenPointToRay(mousePosition);

            // Default: draw a ray forward
            // Raydistance = 100f
            var rayEnd = ray.origin + ray.direction * 100f;

            // Draw the ray in Scene view
            Debug.DrawLine(ray.origin, rayEnd, Color.green);
#endif
        }

        public void Deactivate()
        {
            
        }

        public virtual void OnDrawGizmos()
        {
            
        }

        public void Dispose()
        {
            cursor.gameObject.SetActive(false);
        }
    }
}