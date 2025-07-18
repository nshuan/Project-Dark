using System.Linq;
using DG.Tweening;
using InGame.Effects;
using UnityEngine;

namespace InGame
{
    public abstract class BaseMoveShot : IMouseInput
    {
        public InputInGame InputManager { get; set; }
        
        protected Camera Cam { get; set; }
        protected MonoCursor cursor;
        protected RectTransform cursorRect;
        protected Vector3 mousePosition;
        protected CameraShake effectCamShake;
        
        protected float Cooldown => LevelManager.Instance.PlayerStats.cooldown;
        public bool CanShoot { get; set; }
        private bool OutOfRange { get; set; }
        protected float cdCounter;
        
        public BaseMoveShot()
        {
            
        }
        
        protected BaseMoveShot(Camera cam, MonoCursor cursor)
        {
            Cam = cam;
            effectCamShake = new CameraShake() { Cam = cam };
            this.cursor = cursor;
            cursorRect = (RectTransform)cursor.transform;
        }

        public void Initialize(InputInGame manager)
        {
            InputManager = manager;
        }

        public virtual void OnMouseClick()
        {
           OnMouseClick(0f);
        }

        public void OnMouseClick(float delay)
        {
            if (!CanShoot) return;
            if (OutOfRange) return;
            
            CanShoot = false;
            cdCounter = Cooldown;
            
            // Do cursor effect
            DOTween.Complete(this);
            EffectHelper.Instance.PlayEffect(effectCamShake);
            var seq = DOTween.Sequence(this);
            seq.Append(cursor.transform.DOPunchScale(0.2f * Vector3.one, 0.13f))
                .Join(cursor.transform.DOShakeRotation(0.13f, new Vector3(0f, 0f, 10f)));
            seq.Play();
        }

        public void OnHoldStarted()
        {
            
        }

        public void OnHoldReleased()
        {
            
        }

        public void ResetChargeVariable()
        {
            
        }

        // Check if can spawn elemental effect on hit enemy
        protected virtual void CheckElemental(EnemyEntity enemyEntity)
        {
            // // Chance to create lightning burst
            // if (!enemyEntity.IsInLightning && !enemyEntity.IsDead && Random.Range(0f, 1f) <= LevelManager.Instance.GameStats.sLightningChance)
            // {
            //     enemyEntity.IsInLightning = true;
            //     var lightningTrap = LightningTrapPool.Instance.Get(null);
            //     lightningTrap.Setup(Cam, enemyEntity, 3f * Vector2.one, LevelManager.Instance.GameStats.sLightningDamage, 0.5f, () =>
            //     {
            //         enemyEntity.IsInLightning = false;
            //     });
            //     effectCamShake.Duration = 0.5f;
            // }
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

        protected virtual int CalculateDmg()
        {
            return (int)LevelManager.Instance.PlayerStats.damage;
        }

        public virtual void OnDrawGizmos()
        {
            
        }

        public void Dispose()
        {
            cursor.gameObject.SetActive(false);
            effectCamShake = null;
        }
    }
}