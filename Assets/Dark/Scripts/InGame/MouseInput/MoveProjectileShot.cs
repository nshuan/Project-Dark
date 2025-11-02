using System;
using System.Linq;
using Dark.Scripts.Utils;
using DG.Tweening;
using UnityEngine;
namespace InGame
{
    [Serializable]
    public class MoveProjectileShot : IMouseInput
    {
        private InputInGame InputManager { get; set; }

        protected Camera Cam { get; set; }
        protected MonoCursor cursor;
        protected RectTransform cursorRect;
        protected Vector3 mousePosition;
        protected Vector3 worldMousePosition;
        
        public MoveChargeController ChargeController { get; set; }
        public bool CanShoot { get; set; }
        protected float Cooldown { get; set; }
        protected float cdCounter;

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

        public MoveProjectileShot()
        {

        }

        public MoveProjectileShot(Camera cam, MonoCursor cursor)
        {
            Cam = cam;
            this.cursor = cursor;
            cursorRect = cursor.GetComponent<RectTransform>();
        }

        public void Initialize(InputInGame manager, MoveChargeController chargeController)
        {
            InputManager = manager;
            ChargeController = chargeController;
            Cooldown = LevelUtility.GetSkillCooldown(false);

            var skillBonusInfo = LevelUtility.BonusInfo.skillBonus;
            canChargeBullet = skillBonusInfo.unlockedChargeBullet;
            canChargeSize = skillBonusInfo.unlockedChargeSize;
            canChargeDame = canChargeSize || canChargeBullet;
            canChargeRange = canChargeSize || canChargeBullet;
            
            ChargeController.SetProjectile(LevelUtility.CurrentSkill.projectiles[PlayerProjectileType.ChargeBullet]);
            ChargeController.Cam = Cam;
            
            // Setup shot radius
            InputManager.PlayerVisual.UpdateShotRadius(
                LevelManager.Instance.CurrentTower.GetBaseCenter(),
                LevelUtility.GetSkillRange(
                    1f,
                    Vector2.right));
        }
        
        public virtual void OnMouseClick()
        {
            if (!CanShoot) return;
            
            CanShoot = false;
            
            var isCharge = (canChargeBullet && bulletChargeAdded > 0) || (canChargeDame && dameChargeAdded > 0) ||
                           (canChargeSize && sizeChargeAdded > 0) || (canChargeRange && rangeChargeAdded > 0);
            
            var tempMousePos = Cam.ScreenToWorldPoint(mousePosition);
            var (damage, criticalDamage) = LevelUtility.GetPlayerBulletDamage(
                canChargeDame && dameChargeAdded > 0 ? 1 + dameChargeAdded : 1f);
            var critRate = LevelUtility.GetCriticalRate();
            var bulletNum = LevelUtility.GetNumberOfBullets(bulletChargeAdded);
            var skillSize = LevelUtility.GetSkillSize(
                canChargeSize && sizeChargeAdded > 0 ? 1 + sizeChargeAdded : 1f);
            var skillRange = LevelUtility.GetSkillRange(
                canChargeRange && rangeChargeAdded > 0 ? 1 + rangeChargeAdded : 1f,
                tempMousePos - LevelManager.Instance.CurrentTower.GetBaseCenter());
            var maxHit = 1 + LevelUtility.BonusInfo.skillBonus.bulletMaxHitPlus;
            var stagger = LevelUtility.GetBulletStagger();

            InputManager.BlockTeleport = true;
            var delayShot = 0f;
            if (isCharge)
            {
                delayShot = 0f;
                InputManager.PlayerVisual.EndChargeAndShoot();
            }
            else
            {
                delayShot = InputManager.PlayerVisual.PlayShoot(worldMousePosition);
            }
            InputManager.DelayCall(delayShot, () =>
            {
                InputManager.PlayerVisual.Weapon.GetAllEnemiesInRange(skillRange);
                
                LevelUtility.CurrentSkill.Shoot(
                    LevelUtility.CurrentSkill.projectiles[PlayerProjectileType.Normal],
                    InputManager.ProjectileSpawnPos.position,
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
                    LevelUtility.BonusInfo.skillBonus.GetProjectileActivateActions(isCharge),
                    LevelUtility.BonusInfo.skillBonus.GetProjectileHitActions(isCharge));
                
                if (isCharge)
                    ChargeController.Attack((projectile, direction) =>
                    {
                        projectile.Init(
                            projectile.transform.position, 
                            direction.normalized, 
                            skillRange,
                            skillSize, 
                            LevelUtility.CurrentSkill.speedScale,
                            damage,
                            criticalDamage, 
                            critRate, 
                            LevelUtility.CurrentSkill.stagger, 
                            true, 
                            maxHit, 
                            null, 
                            LevelUtility.BonusInfo.skillBonus.GetProjectileHitActions(true),
                            ProjectileType.PlayerProjectile);
                        
                        projectile.Activate(0f);
                    });

                InputManager.BlockTeleport = false;
            });

            Cooldown = LevelUtility.GetSkillCooldown(isCharge);
            
            if (isCharge)
                CombatActions.OnAttackCharge?.Invoke(Cooldown);
            else
                CombatActions.OnAttackNormal?.Invoke(Cooldown);
            
            cdCounter = Cooldown;
            cdCounter += delayShot;

            // Reset range
            InputManager.PlayerVisual.UpdateShotRadius(
                LevelManager.Instance.CurrentTower.GetBaseCenter(),
                LevelUtility.GetSkillRange(
                    1f,
                    Vector2.right), false);
            
            // Do cursor effect
            cursor.UpdateScale(0f);
            cursor.UpdateChargeUnitAdd(false);
            cursor.UpdateCooldown(false, 0f);
            DOTween.Complete(this);
            var seq = DOTween.Sequence(this);
            seq.Append(cursor.transform.DOPunchScale(0.3f * Vector3.one, 0.13f).SetEase(Ease.InQuad))
                .Join(cursor.visual.DOFade(0.3f, 0.13f).SetEase(Ease.InQuad).SetLoops(2, LoopType.Yoyo))
                .Join(DOTween.To(() => cursor.content.localScale.x - 1f, x =>
                {
                    cursor.UpdateScale(x);
                }, 0f, 0.13f));
            seq.Play().OnComplete(() => cursor.UpdateCooldown(false, 0f));
        }

        public void OnHoldStarted()
        {
            if (!CanShoot) return;
            if (isCharging) return; 
            
            ResetChargeVariable();

            if (canChargeBullet && bulletChargeMaxStep > 0) isCharging = true;
            else if (canChargeDame && dameChargeMaxStep > 0) isCharging = true;
            else if (canChargeSize && sizeChargeMaxStep > 0) isCharging = true;
            else if (canChargeRange && rangeChargeMaxStep > 0) isCharging = true;
            
            if (isCharging)
                InputManager.PlayerVisual.PlayCharge();
        }

        public void OnHoldReleased()
        {
            isCharging = false;
        }

        public void ResetChargeVariable()
        {
            chargeStepTime = LevelUtility.GetChargeStepTime();
            
            bulletChargeAdded = 0;
            bulletPerStep = LevelUtility.GetChargeBulletPerStep();
            bulletChargeMaxStep = LevelUtility.GetChargeBulletMaxStep();

            dameChargeAdded = 0f;
            damePerStep = LevelUtility.GetChargeDamePerStep();
            dameChargeMaxStep = LevelUtility.GetChargeDameMaxStep();

            sizeChargeAdded = 0f;
            sizePerStep = LevelUtility.GetChargeSizePerStep();
            sizeChargeMaxStep = LevelUtility.GetChargeSizeMaxStep();
            
            rangeChargeAdded = 0f;
            rangePerStep = LevelUtility.GetChargeRangePerStep();
            rangeChargeMaxStep = LevelUtility.GetChargeRangeMaxStep();

            isCharging = false;
            chargeStep = 0;
            chargeTimer = 0f;
            chargeMaxStep = Mathf.Max(bulletChargeMaxStep, dameChargeMaxStep, sizeChargeMaxStep, rangeChargeMaxStep);
        }

        public bool CanMove => true;

        public virtual void OnUpdate()
        {
            worldMousePosition = Cam.ScreenToWorldPoint(Input.mousePosition);
            
            mousePosition = Input.mousePosition;
            mousePosition.z = 0; // Set z to 0 for 2D
            cursorRect.position = mousePosition;    
            InputManager.PlayerVisual.SetDirection(worldMousePosition);
            
            // Cooldown if player can not shoot
            if (!CanShoot)
            {
                cdCounter -= Time.deltaTime;
                if (cdCounter <= 0)
                    CanShoot = true;
            }
            else
            {
                if (isCharging && chargeMaxStep > 0 && chargeStep < chargeMaxStep)
                {
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
                            for (int i = 0; i < bulletChargeAdded - ChargeController.TotalBulletAdded; i++)
                            {
                                ChargeController.AddBullet(InputManager.PlayerVisual.transform.position,
                                    worldMousePosition - InputManager.PlayerVisual.transform.position);
                            }
                        }

                        if (canChargeDame && dameChargeMaxStep > 0)
                        {
                            dameChargeAdded = damePerStep * Math.Min(chargeStep, dameChargeMaxStep);
                        }

                        if (canChargeSize && sizeChargeMaxStep > 0)
                        {
                            sizeChargeAdded = sizePerStep * Math.Min(chargeStep, sizeChargeMaxStep);
                            ChargeController.AddSize(LevelUtility.GetSkillSize(
                                sizeChargeMaxStep > 0 ? 1 + sizeChargeAdded : 1f));
                        }

                        if (canChargeRange && rangeChargeMaxStep > 0)
                        {
                            rangeChargeAdded = rangePerStep * Math.Min(chargeStep, rangeChargeMaxStep);
                            InputManager.PlayerVisual.UpdateShotRadius(
                                LevelManager.Instance.CurrentTower.GetBaseCenter(),
                                LevelUtility.GetSkillRange(
                                    canChargeRange && rangeChargeAdded > 0 ? 1 + rangeChargeAdded : 1f,
                                    Vector2.right));
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

        public virtual void OnDrawGizmos()
        {
            
        }

        public void Dispose()
        {
            cursor.gameObject.SetActive(false);
        }
    }
}