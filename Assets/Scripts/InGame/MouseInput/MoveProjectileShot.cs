using System;
using System.Linq;
using DG.Tweening;
using InGame.Effects;
using InGame.Trap;
using UnityEngine;
using UnityEngine.UI;

namespace InGame
{
    [Serializable]
    public class MoveProjectileShot : IMouseInput
    {
        public InputInGame InputManager { get; set; }
        
        protected Camera Cam { get; set; }
        protected MonoCursor cursor;
        protected RectTransform cursorRect;
        protected Vector3 mousePosition;
        protected Vector3 worldMousePosition;
        
        protected float Cooldown => LevelManager.Instance.GameStats.pShotCooldown;
        public bool CanShoot { get; set; }
        private bool OutOfRange { get; set; }
        protected float cdCounter;

        #region Charge
        
        private bool isChargingBullet;
        private int bulletAdd;
        private int maxBulletAdd;
        private float bulletAddInterval;
        private float bulletAddTimer;

        private bool isChargingDame;
        private float maxDameMultiplierAdd;
        private float maxDameChargeTime;
        private float dameChargeTime;
        
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

        public virtual void OnMouseClick()
        {
            OnMouseClick(0f);
        }
        
        public virtual void OnMouseClick(float delay)
        {
            if (!CanShoot) return;
            if (OutOfRange) return;

            var (damage, criticalDamage) = LevelUtility.GetPlayerBulletDamage(
                InputManager.PlayerStats.damage,
                InputManager.CurrentSkillConfig.damePerBullet,
                InputManager.PlayerStats.criticalDamage,
                1 + Mathf.Min(dameChargeTime / maxDameChargeTime, 1f) * maxDameMultiplierAdd);
            
            CanShoot = false;
            cdCounter = InputManager.CurrentSkillConfig.cooldown + delay;

            var tempMousePos = Cam.ScreenToWorldPoint(mousePosition);
            LevelManager.Instance.SetTeleportTowerState(false);
            InputManager.playerVisual.PlayShoot(worldMousePosition);
            InputManager.DelayCall(delay, () =>
            {
                InputManager.CurrentSkillConfig.Shoot(
                    InputManager.CursorRangeCenter.position, 
                    tempMousePos,
                    damage,
                    InputManager.CurrentSkillConfig.numberOfBullets + bulletAdd,
                    criticalDamage,
                    InputManager.PlayerStats.criticalRate);

                LevelManager.Instance.SetTeleportTowerState(true);
            });
            
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
            if (OutOfRange) return;
            if (isChargingBullet
                || isChargingDame) return; 
            ResetChargeVariable();
            
            if (InputManager.CurrentSkillConfig.chargeBulletMaxAdd > 0)
                isChargingBullet = true;
            if (maxDameMultiplierAdd > 0) isChargingDame = true;
        }

        public void OnHoldReleased()
        {
            isChargingBullet = false;
            isChargingDame = false;
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
        }

        public virtual void OnUpdate()
        {
            worldMousePosition = Cam.ScreenToWorldPoint(Input.mousePosition);
            
            if (OutOfRange)
            {
                if (Vector2.Distance(worldMousePosition, InputManager.CursorRangeCenter.position)
                    <= InputManager.CursorRangeRadius)
                {
                    OutOfRange = false;
                    mousePosition = Input.mousePosition;
                    mousePosition.z = 0; // Set z to 0 for 2D
                    cursorRect.position = mousePosition;
                    cursor.gameObject.SetActive(true);
                    InputManager.playerVisual.SetRangeVisual(false);
                }
                return;
            }

            if (Vector2.Distance(worldMousePosition, InputManager.CursorRangeCenter.position) 
                > InputManager.CursorRangeRadius)
            {
                cursor.gameObject.SetActive(false);
                InputManager.playerVisual.SetRangeVisual(true);

                if (CanShoot)
                {
                    OutOfRange = true;
                    ResetChargeVariable();
                    return;
                }
            }
            else
            {
                cursor.gameObject.SetActive(true);
                InputManager.playerVisual.SetRangeVisual(false);
            }
            
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
                
                // Charge dame
                if (isChargingDame)
                {
                    dameChargeTime += Time.deltaTime;
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

        protected virtual float CalculateDmg()
        {
            return LevelManager.Instance.GameStats.pDmgPerShot;
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