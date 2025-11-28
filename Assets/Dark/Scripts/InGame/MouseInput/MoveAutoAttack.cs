using System;
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
        protected Vector3 mousePosition;
        protected Vector3 worldMousePosition;
        
        public bool CanShoot { get; set; }
        protected float Cooldown { get; set; }
        protected float ActivateDuration { get; set; } = 1f;
        protected float cdCounter;

        public MoveAutoAttack()
        {

        }

        public MoveAutoAttack(Camera cam, MonoCursor cursor)
        {
            Cam = cam;
            this.cursor = cursor;
            cursorRect = cursor.GetComponent<RectTransform>();
        }

        public void Initialize(InputInGame manager, MoveChargeController chargeController)
        {
            CanShoot = GameConst.DefaultAutoAttack;
            cursor.SetAuto(GameConst.DefaultAutoAttack);

            InputManager = manager;
            Cooldown = LevelUtility.GetSkillCooldown(false);
            ActivateDuration = 1f;
        }
        
        public virtual void OnMouseClick()
        {
            if (!CanShoot) return;
            
            var tempMousePos = Cam.ScreenToWorldPoint(mousePosition);
            var (damage, criticalDamage) = LevelUtility.GetPlayerBulletDamage(1f);
            var critRate = LevelUtility.GetCriticalRate();
            var bulletNum = LevelUtility.GetNumberOfBullets( 0);
            var skillSize = LevelUtility.GetSkillSize(1f);
            var skillRange = LevelUtility.GetSkillRange(
                1f,
                Vector2.right);
            var maxHit = 1 + LevelUtility.BonusInfo.skillBonus.bulletMaxHitPlus;
            var stagger = LevelUtility.GetBulletStagger();
            
            var delayShot = InputManager.PlayerVisual.PlayShoot(worldMousePosition);
            InputManager.DelayCall(delayShot, () =>
            {
                InputManager.PlayerVisual.Weapon.GetAllEnemiesInRange(skillRange);
                
                LevelUtility.CurrentSkill.Shoot(
                    LevelUtility.CurrentSkill.projectiles[PlayerProjectileType.Normal],
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
                    LevelUtility.BonusInfo.skillBonus.GetProjectileActivateActions(false),
                    LevelUtility.BonusInfo.skillBonus.GetProjectileHitActions(false));
            });

            CombatActions.OnAttackNormal?.Invoke(Cooldown);
            
            cdCounter = Cooldown;
            
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
            
            if (!CanShoot) return;

            this.worldMousePosition.x = worldMousePosition.x;
            this.worldMousePosition.y = worldMousePosition.y; 
            
            mousePosition = Input.mousePosition;
            mousePosition.z = 0; // Set z to 0 for 2D
            cursorRect.position = mousePosition;    
            
            cursor.UpdateCooldown(true, 1 - Mathf.Clamp(cdCounter / Cooldown, 0f, 1f));
            if (cdCounter <= 0)
                OnMouseClick();
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
    }
}