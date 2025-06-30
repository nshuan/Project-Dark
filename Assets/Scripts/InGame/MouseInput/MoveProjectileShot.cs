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
        
        protected float Cooldown => LevelManager.Instance.GameStats.pShotCooldown;
        public bool CanShoot { get; set; }
        private bool OutOfRange { get; set; }
        protected float cdCounter;

        #region Charge

        private bool isCharging;
        
        private int bulletAdd;
        private int maxBulletAdd;
        private float bulletAddInterval;
        private float bulletAddTimer;

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
            if (!CanShoot) return;
            if (OutOfRange) return;

            var (damage, criticalDamage) = LevelUtility.GetPlayerBulletDamage(
                InputManager.PlayerStats.damage,
                InputManager.CurrentSkillConfig.damePerBullet,
                InputManager.PlayerStats.criticalDamage);
            CanShoot = false;
            cdCounter = InputManager.CurrentSkillConfig.cooldown;
            InputManager.CurrentSkillConfig.Shoot(
                InputManager.CursorRangeCenter.position, 
                Cam.ScreenToWorldPoint(mousePosition),
                damage,
                InputManager.CurrentSkillConfig.numberOfBullets + bulletAdd,
                criticalDamage,
                InputManager.PlayerStats.criticalRate);
            
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
            if (isCharging) return; 
            ResetChargeVariable();
            isCharging = true;
        }

        public void OnHoldReleased()
        {
            isCharging = false;
        }

        public void ResetChargeVariable()
        {
            bulletAdd = 0;
            maxBulletAdd = InputManager.CurrentSkillConfig.chargeBulletMaxAdd;
            bulletAddInterval = InputManager.CurrentSkillConfig.chargeBulletInterval;
            bulletAddTimer = bulletAddInterval;
        }

        public virtual void OnUpdate()
        {
            if (OutOfRange)
            {
                if (Vector2.Distance(Cam.ScreenToWorldPoint(Input.mousePosition),
                        InputManager.CursorRangeCenter.position) <=
                    InputManager.CursorRangeRadius)
                {
                    OutOfRange = false;
                    mousePosition = Input.mousePosition;
                    mousePosition.z = 0; // Set z to 0 for 2D
                    cursorRect.position = mousePosition;
                    cursor.gameObject.SetActive(true);
                }
                return;
            }

            if (Vector2.Distance(Cam.ScreenToWorldPoint(Input.mousePosition),
                    InputManager.CursorRangeCenter.position) >
                InputManager.CursorRangeRadius)
            {
                OutOfRange = true;
                cursor.gameObject.SetActive(false);
                return;
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
            else if (isCharging)
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