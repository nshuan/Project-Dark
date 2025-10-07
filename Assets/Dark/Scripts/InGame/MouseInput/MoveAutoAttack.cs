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
        protected float cdCounter;

        private bool isActivating;

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
            CanShoot = false;

            InputManager = manager;
            Cooldown = LevelUtility.GetSkillCooldown(
                InputManager.CurrentSkillConfig.skillId,
                InputManager.PlayerStats.cooldown,
                InputManager.CurrentSkillConfig.cooldown);
        }
        
        public virtual void OnMouseClick()
        {
            if (!CanShoot) return;
            
            var tempMousePos = Cam.ScreenToWorldPoint(mousePosition);
            var (damage, criticalDamage) = LevelUtility.GetPlayerBulletDamage(
                InputManager.CurrentSkillConfig.skillId,
                InputManager.PlayerStats.damage,
                InputManager.CurrentSkillConfig.damePerBullet,
                InputManager.PlayerStats.criticalDamage,
                1f);
            var critRate = LevelUtility.GetCriticalRate(InputManager.PlayerStats.criticalRate);
            var bulletNum = LevelUtility.GetNumberOfBullets(InputManager.CurrentSkillConfig.skillId, InputManager.CurrentSkillConfig.numberOfBullets, 0);
            var skillSize = LevelUtility.GetSkillSize(InputManager.CurrentSkillConfig.skillId,
                InputManager.CurrentSkillConfig.size,
                1f);
            var skillRange = LevelUtility.GetSkillRange(InputManager.CurrentSkillConfig.skillId,
                InputManager.CurrentSkillConfig.range,
                1f,
                tempMousePos - LevelManager.Instance.CurrentTower.GetBaseCenter());
            var maxHit = 1 + LevelUtility.BonusInfo.skillBonus.bulletMaxHitPlus;
            var stagger = LevelUtility.GetBulletStagger(InputManager.CurrentSkillConfig.skillId,
                InputManager.CurrentSkillConfig.stagger);

            InputManager.BlockTeleport = true;
            var delayShot = InputManager.PlayerVisual.PlayShoot(worldMousePosition);
            InputManager.DelayCall(delayShot, () =>
            {
                InputManager.PlayerVisual.Weapon.GetAllEnemiesInRange(skillRange);
                
                InputManager.CurrentSkillConfig.Shoot(
                    InputManager.CurrentSkillConfig.projectiles[PlayerProjectileType.Normal],
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
                
                InputManager.BlockTeleport = false;
            });

            CombatActions.OnAttackNormal?.Invoke(Cooldown);
            
            cdCounter = Cooldown;
            
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
            if (CanShoot) return;
            isActivating = true;
            cdCounter = 1f;
        }

        public void OnHoldReleased()
        {
            if (CanShoot) return;
            isActivating = false;
            cursor.UpdateCooldown(false, 0f);
        }

        public void ResetChargeVariable()
        {
            CanShoot = false;
            isActivating = false;
        }

        public bool CanMove => true;

        public virtual void OnUpdate()
        {
            if (!CanShoot && !isActivating) return;
            
            if (isActivating)
            {
                cdCounter -= Time.deltaTime;
                cursor.UpdateCooldown(true, 1 - Mathf.Clamp(cdCounter / Cooldown, 0f, 1f));
                if (cdCounter <= 0f)
                {
                    isActivating = false;
                    CanShoot = true;
                }
                
                return;
            }
            
            worldMousePosition = Cam.ScreenToWorldPoint(Input.mousePosition);
            
            mousePosition = Input.mousePosition;
            mousePosition.z = 0; // Set z to 0 for 2D
            cursorRect.position = mousePosition;    
            InputManager.PlayerVisual.SetDirection(worldMousePosition);
            
            cdCounter -= Time.deltaTime;
            cursor.UpdateCooldown(true, 1 - Mathf.Clamp(cdCounter / Cooldown, 0f, 1f));
            if (cdCounter <= 0)
                OnMouseClick();
        }

        public virtual void OnDrawGizmos()
        {
            
        }

        public void Dispose()
        {
            
        }
    }
}