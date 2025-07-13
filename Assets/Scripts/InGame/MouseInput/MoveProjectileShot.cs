using System;
using System.Linq;
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

        public void Initialize(InputInGame manager)
        {
            InputManager = manager;
            Cooldown = LevelUtility.GetSkillCooldown(
                InputManager.CurrentSkillConfig.skillId,
                InputManager.PlayerStats.cooldown,
                InputManager.CurrentSkillConfig.cooldown);

            var skillBonusInfo = LevelUtility.BonusInfo.skillBonusMapById[InputManager.CurrentSkillConfig.skillId];
            canChargeBullet = skillBonusInfo.unlockedChargeBullet;
            canChargeDame = skillBonusInfo.unlockedChargeDame;
            canChargeSize = skillBonusInfo.unlockedChargeSize;
            canChargeRange = skillBonusInfo.unlockedChargeRange;
        }
        
        public virtual void OnMouseClick()
        {
            if (!CanShoot) return;
            
            CanShoot = false;
            
            var (damage, criticalDamage) = LevelUtility.GetPlayerBulletDamage(
                InputManager.CurrentSkillConfig.skillId,
                InputManager.PlayerStats.damage,
                InputManager.CurrentSkillConfig.damePerBullet,
                InputManager.PlayerStats.criticalDamage,
                1 + Mathf.Min(dameChargeTime / maxDameChargeTime, 1f) * maxDameMultiplierAdd);
            var critRate = LevelUtility.GetCriticalRate(InputManager.PlayerStats.criticalRate);
            var bulletNum = LevelUtility.GetNumberOfBullets(InputManager.CurrentSkillConfig.skillId, InputManager.CurrentSkillConfig.numberOfBullets, bulletAdd);
            var skillSize = LevelUtility.GetSkillSize(InputManager.CurrentSkillConfig.skillId,
                InputManager.CurrentSkillConfig.size,
                1 + Mathf.Min(sizeChargeTime / maxSizeChargeTime, 1f) * maxSizeMultiplierAdd);
            var skillRange = LevelUtility.GetSkillRange(InputManager.CurrentSkillConfig.skillId,
                InputManager.CurrentSkillConfig.range,
                1 + Mathf.Min(rangeChargeTime / maxRangeChargeTime, 1f) * maxRangeMultiplierAdd);

            var tempMousePos = Cam.ScreenToWorldPoint(mousePosition);
            LevelManager.Instance.SetTeleportTowerState(false);
            var delayShot = InputManager.playerVisual.PlayShoot(worldMousePosition);
            InputManager.DelayCall(delayShot, () =>
            {
                InputManager.playerVisual.Weapon.GetAllEnemiesInRange(skillRange);
                
                InputManager.CurrentSkillConfig.Shoot(
                    canChargeBullet
                        ? InputManager.CurrentSkillConfig.chargeProjectilePrefab
                        : InputManager.CurrentSkillConfig.projectilePrefab,
                    InputManager.CursorRangeCenter.position, 
                    tempMousePos,
                    damage,
                    bulletNum,
                    skillSize,
                    skillRange,
                    criticalDamage,
                    critRate,
                    canChargeBullet || canChargeDame || canChargeSize || canChargeRange,
                    LevelUtility.BonusInfo.skillBonusMapById[InputManager.CurrentSkillConfig.skillId].projectileHitActions);

                LevelManager.Instance.SetTeleportTowerState(true);
            });
            
            cdCounter = Cooldown;
            cdCounter += delayShot;
            
            // Do cursor effect
            DOTween.Complete(this);
            var seq = DOTween.Sequence(this);
            seq.Append(cursor.transform.DOPunchScale(0.2f * Vector3.one, 0.13f))
                .Join(cursor.transform.DOShakeRotation(0.13f, new Vector3(0f, 0f, 10f)));
            seq.Play();
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
        }

        public void OnHoldReleased()
        {
            isChargingBullet = false;
            isChargingDame = false;
            isChargingSize = false;
            isChargingRange = false;
        }

        public void ResetChargeVariable()
        {
            // bullet number
            bulletAdd = 0;
            maxBulletAdd = InputManager.CurrentSkillConfig.chargeBulletMaxAdd;
            bulletAddInterval = InputManager.CurrentSkillConfig.chargeBulletInterval;
            bulletAddTimer = bulletAddInterval;
            
            // damage
            maxDameMultiplierAdd = InputManager.CurrentSkillConfig.chargeDameMax;
            maxDameChargeTime = InputManager.CurrentSkillConfig.chargeDameTime;
            dameChargeTime = 0f;
            
            // Size
            maxSizeMultiplierAdd = InputManager.CurrentSkillConfig.size;
            maxSizeChargeTime = InputManager.CurrentSkillConfig.chargeSizeTime;
            sizeChargeTime = 0f;
            
            // Range
            maxRangeMultiplierAdd = InputManager.CurrentSkillConfig.range;
            maxRangeChargeTime = InputManager.CurrentSkillConfig.chargeRangeTime;
            rangeChargeTime = 0f;
        }

        public virtual void OnUpdate()
        {
            worldMousePosition = Cam.ScreenToWorldPoint(Input.mousePosition);
            
            mousePosition = Input.mousePosition;
            mousePosition.z = 0; // Set z to 0 for 2D
            cursorRect.position = mousePosition;    
            
            // Cooldown if player can not shoot
            if (!CanShoot)
            {
                cdCounter -= Time.deltaTime;
                if (cdCounter <= 0)
                    CanShoot = true;
                
                // Update UI
                cursor.UpdateCooldown(Mathf.Clamp(cdCounter / Cooldown, 0f, 1f));
                cursor.UpdateBulletAdd(false);
            }
            else
            {
                if (canChargeBullet)
                {
                    // Charge bullets
                    if (isChargingBullet)
                    {
                        if (bulletAddTimer > 0)
                            bulletAddTimer -= Time.deltaTime;
                        else if (bulletAdd < maxBulletAdd)
                        {
                            bulletAdd += 1;
                            bulletAddTimer = bulletAddInterval;
                        }
                    
                        // Update UI
                        cursor.UpdateBulletAdd(true, bulletAdd);
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