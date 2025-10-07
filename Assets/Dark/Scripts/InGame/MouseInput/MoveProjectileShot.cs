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
        private bool isChargingBullet;
        private int bulletAdd;
        private int maxBulletAdd;
        private float bulletAddInterval;
        private float bulletAddTimer;
        private bool isChargeBulletMax;

        private bool canChargeDame;
        private bool isChargingDame;
        private float maxDameMultiplierAdd;
        private float maxDameChargeTime;
        private float dameChargeTime;

        private bool canChargeSize;
        private bool isChargingSize;
        private float maxSizeMultiplierAdd;
        private float maxSizeChargeTime;
        private float sizeChargeTime;

        private bool canChargeRange;
        private bool isChargingRange;
        private float maxRangeMultiplierAdd;
        private float maxRangeChargeTime;
        private float rangeChargeTime;

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
            Cooldown = LevelUtility.GetSkillCooldown(
                InputManager.CurrentSkillConfig.skillId,
                InputManager.PlayerStats.cooldown,
                InputManager.CurrentSkillConfig.cooldown);

            var skillBonusInfo = LevelUtility.BonusInfo.skillBonus;
            canChargeBullet = skillBonusInfo.unlockedChargeBullet;
            canChargeDame = skillBonusInfo.unlockedChargeDame;
            canChargeSize = skillBonusInfo.unlockedChargeSize;
            canChargeRange = skillBonusInfo.unlockedChargeRange;
            
            ChargeController.SetProjectile(InputManager.CurrentSkillConfig.projectiles[PlayerProjectileType.ChargeBullet]);
            ChargeController.Cam = Cam;
        }
        
        public virtual void OnMouseClick()
        {
            if (!CanShoot) return;
            
            CanShoot = false;
            
            var isCharge = (canChargeBullet && bulletAdd > 0) || (canChargeDame && dameChargeTime > 0) ||
                           (canChargeSize && sizeChargeTime > 0) || (canChargeRange && rangeChargeTime > 0);
            
            var tempMousePos = Cam.ScreenToWorldPoint(mousePosition);
            var (damage, criticalDamage) = LevelUtility.GetPlayerBulletDamage(
                InputManager.CurrentSkillConfig.skillId,
                InputManager.PlayerStats.damage,
                InputManager.CurrentSkillConfig.damePerBullet,
                InputManager.PlayerStats.criticalDamage,
                canChargeDame && dameChargeTime > 0 ? 1 + Mathf.Max(dameChargeTime / maxDameChargeTime * maxDameMultiplierAdd, 0f) : 1f);
            var critRate = LevelUtility.GetCriticalRate(InputManager.PlayerStats.criticalRate);
            var bulletNum = LevelUtility.GetNumberOfBullets(InputManager.CurrentSkillConfig.skillId, InputManager.CurrentSkillConfig.numberOfBullets, bulletAdd);
            var skillSize = LevelUtility.GetSkillSize(InputManager.CurrentSkillConfig.skillId,
                InputManager.CurrentSkillConfig.size,
                canChargeSize && sizeChargeTime > 0 ? 1 + Mathf.Min(sizeChargeTime / maxSizeChargeTime, 1f) * maxSizeMultiplierAdd : 1f);
            var skillRange = LevelUtility.GetSkillRange(InputManager.CurrentSkillConfig.skillId,
                InputManager.CurrentSkillConfig.range,
                canChargeRange && rangeChargeTime > 0 ? 1 + Mathf.Min(rangeChargeTime / maxRangeChargeTime, 1f) * maxRangeMultiplierAdd : 1f,
                tempMousePos - LevelManager.Instance.CurrentTower.GetBaseCenter());
            var maxHit = 1 + LevelUtility.BonusInfo.skillBonus.bulletMaxHitPlus;
            var stagger = LevelUtility.GetBulletStagger(InputManager.CurrentSkillConfig.skillId,
                InputManager.CurrentSkillConfig.stagger);

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
                
                InputManager.CurrentSkillConfig.Shoot(
                    InputManager.CurrentSkillConfig.projectiles[PlayerProjectileType.Normal],
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
                            InputManager.CurrentSkillConfig.speedScale,
                            damage,
                            criticalDamage, 
                            critRate, 
                            InputManager.CurrentSkillConfig.stagger, 
                            true, 
                            maxHit, 
                            null, 
                            LevelUtility.BonusInfo.skillBonus.GetProjectileHitActions(true),
                            ProjectileType.PlayerProjectile);
                        
                        projectile.Activate(0f);
                    });

                InputManager.BlockTeleport = false;
            });

            if (isCharge)
                CombatActions.OnAttackCharge?.Invoke(Cooldown);
            else
                CombatActions.OnAttackNormal?.Invoke(Cooldown);
            
            cdCounter = Cooldown;
            cdCounter += delayShot;
            
            // Do cursor effect
            cursor.UpdateBulletAdd(false);
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
            if (isChargingBullet
                || isChargingDame
                || isChargingSize
                || isChargingRange) return; 
            
            ResetChargeVariable();
            
            if (canChargeBullet && InputManager.CurrentSkillConfig.chargeBulletMaxAdd > 0)
                isChargingBullet = true;
            if (canChargeDame && maxDameMultiplierAdd > 0) isChargingDame = true;
            if (canChargeSize && maxSizeMultiplierAdd > 0) isChargingSize = true;
            if (canChargeRange && maxRangeMultiplierAdd > 0) isChargingRange = true;
            
            if (isChargingBullet || isChargingDame || isChargingSize || isChargingRange)
                InputManager.PlayerVisual.PlayCharge();
        }

        public void OnHoldReleased()
        {
            isChargingBullet = false;
            isChargeBulletMax = false;
            isChargingDame = false;
            isChargingSize = false;
            isChargingRange = false;
        }

        public void ResetChargeVariable()
        {
            // bullet number
            bulletAdd = 0;
            var maxBullet = LevelUtility.GetChargeBulletMax(InputManager.CurrentSkillConfig.chargeBulletMaxAdd,
                InputManager.CurrentSkillConfig.chargeBulletInterval);
            maxBulletAdd = maxBullet.Item1;
            bulletAddInterval = maxBullet.Item2;
            bulletAddTimer = bulletAddInterval;
            
            // damage
            var maxDame = LevelUtility.GetChargeDameMax(InputManager.CurrentSkillConfig.chargeDameMax,
                InputManager.CurrentSkillConfig.chargeDameTime);
            maxDameMultiplierAdd = maxDame.Item1;
            maxDameChargeTime = maxDame.Item2;
            dameChargeTime = -1f;
            
            // Size
            var maxSize = LevelUtility.GetChargeSizeMax(InputManager.CurrentSkillConfig.chargeSizeMax,
                InputManager.CurrentSkillConfig.chargeSizeTime);
            maxSizeMultiplierAdd = maxSize.Item1;
            maxSizeChargeTime = maxSize.Item2;
            sizeChargeTime = 0f;
            
            // Range
            var maxRange = LevelUtility.GetChargeRangeMax(InputManager.CurrentSkillConfig.chargeRangeMax,
                InputManager.CurrentSkillConfig.chargeRangeTime);
            maxRangeMultiplierAdd = maxRange.Item1;
            maxRangeChargeTime = maxRange.Item2;
            rangeChargeTime = 0f;
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
                if (canChargeBullet)
                {
                    // Charge bullets
                    if (isChargingBullet)
                    {
                        if (bulletAddTimer > 0)
                        {
                            bulletAddTimer -= Time.deltaTime;
                            // Update UI
                            cursor.UpdateCooldown(true, 1 - Mathf.Clamp(bulletAddTimer / bulletAddInterval, 0f, 1f));
                        }
                        else if (bulletAdd < maxBulletAdd)
                        {
                            bulletAdd += 1;
                            ChargeController.AddBullet(InputManager.PlayerVisual.transform.position,
                                worldMousePosition - InputManager.PlayerVisual.transform.position);
                            bulletAddTimer = bulletAddInterval;
                            cursor.UpdateBulletAdd(true, bulletAdd);
                            cursor.transform.DOPunchScale(0.2f * Vector3.one, 0.13f).SetEase(Ease.InQuad)
                                .OnComplete(() => cursor.UpdateCooldown(true, 0f));
                        }
                        else if (bulletAdd == maxBulletAdd)
                        {
                            if (!isChargeBulletMax)
                            {
                                cursor.transform.DOPunchScale(0.2f * Vector3.one, 0.13f).SetEase(Ease.InQuad);
                                isChargeBulletMax = true;
                            }
                            cursor.UpdateMax();
                        }
                    }
                    else
                    {
                        // Update UI
                        cursor.UpdateBulletAdd(false);
                    }
                }
                
                // Charge dame
                if (canChargeDame && isChargingDame)
                {
                    dameChargeTime += Time.deltaTime;
                }
                
                // Charge size
                if (canChargeSize && isChargingSize)
                {
                    sizeChargeTime += Time.deltaTime;
                    var size = LevelUtility.GetSkillSize(InputManager.CurrentSkillConfig.skillId,
                        InputManager.CurrentSkillConfig.size,
                        maxSizeMultiplierAdd > 0
                            ? 1 + Mathf.Min(sizeChargeTime / maxSizeChargeTime, 1f) * maxSizeMultiplierAdd
                            : 1f);
                    ChargeController.AddSize(size);
                    cursor.UpdateScale(Mathf.Min(sizeChargeTime / maxSizeChargeTime, 1f));
                }
                
                // Charge range
                if (canChargeRange && isChargingRange)
                {
                    rangeChargeTime += Time.deltaTime;
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
            
            InputManager.PlayerVisual.DebugUpdateShotRadius(
                LevelManager.Instance.CurrentTower.GetBaseCenter(),
                LevelUtility.GetSkillRange(InputManager.CurrentSkillConfig.skillId, 
                    InputManager.CurrentSkillConfig.range, 
                    canChargeRange && rangeChargeTime > 0 ? 1 + Mathf.Min(rangeChargeTime / maxRangeChargeTime, 1f) * maxRangeMultiplierAdd : 1f,
                    Vector2.right));
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