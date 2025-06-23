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
        protected RectTransform cursor;
        protected Image uiCursorCd;
        protected Vector3 mousePosition;
        
        protected float Cooldown => LevelManager.Instance.GameStats.pShotCooldown;
        public bool CanShoot { get; set; }
        private bool OutOfRange { get; set; }
        protected float cdCounter;

        public MoveProjectileShot()
        {
            
        }
        
        public MoveProjectileShot(Camera cam, MonoCursor cursor)
        {
            Cam = cam;
            this.cursor = (RectTransform)cursor.transform;
            this.uiCursorCd = cursor.UICooldown;
        }

        public virtual void OnMouseClick()
        {
            if (!CanShoot) return;
            if (OutOfRange) return;
            
            CanShoot = false;
            cdCounter = InputManager.CurrentSkillConfig.cooldown;
            InputManager.CurrentSkillConfig.shootLogic.Shoot(
                InputManager.CursorRangeCenter.position, 
                Cam.ScreenToWorldPoint(mousePosition),
                LevelUtility.GetPlayerBulletDamage(InputManager.CurrentSkillConfig),
                InputManager.CurrentSkillConfig.numberOfBullets);
            
            // Do cursor effect
            DOTween.Complete(this);
            var seq = DOTween.Sequence(this);
            seq.Append(cursor.transform.DOPunchScale(0.2f * Vector3.one, 0.13f))
                .Join(cursor.transform.DOShakeRotation(0.13f, new Vector3(0f, 0f, 10f)));
            seq.Play();
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
                    cursor.position = mousePosition;
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
            cursor.position = mousePosition;
            
            // Cooldown if player can not shoot
            if (!CanShoot)
            {
                cdCounter -= Time.deltaTime;
                if (cdCounter <= 0)
                    CanShoot = true;
                
                // Update UI
                if (uiCursorCd)
                    uiCursorCd.fillAmount = Mathf.Clamp(cdCounter / Cooldown, 0f, 1f);
            }
#if UNITY_EDITOR
            var corners = new Vector3[4];
            cursor.GetWorldCorners(corners);
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