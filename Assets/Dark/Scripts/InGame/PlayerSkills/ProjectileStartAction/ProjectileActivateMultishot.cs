using System;
using Dark.Scripts.Utils;
using UnityEngine;

namespace InGame
{
    [Serializable]
    public class ProjectileActivateMultishot : IProjectileActivate
    {
        public ProjectileEntity projectile;
        public int amount;
        
        private PlayerCharacter character;
        
        public void DoAction(ProjectileEntity parentProjectile, Vector2 direction)
        {
            if (amount == 0) return;

            character = LevelManager.Instance.Player;
            var characterAttackAnimDelay = character.GetShootPrepareDuration();
            
            direction.Normalize();
            
            var baseDamage = parentProjectile.Damage;
            
            // Viên đầu tiên là đạn chính
            var p = ProjectilePool.Instance.Get(projectile, null, false);
            p.transform.rotation = Quaternion.Euler(0f, 0f,  Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
            p.transform.position = parentProjectile.transform.position;
            p.Init(
                parentProjectile.RangeCenter, 
                direction, 
                parentProjectile.Range, 
                parentProjectile.Size, 
                parentProjectile.SpeedScale, 
                baseDamage, 
                LevelUtilityV2.ToInt(baseDamage * LevelUtilityV2.GetBaseCriticalDmgScale()), 
                parentProjectile.CriticalRate, 
                parentProjectile.Stagger, 
                parentProjectile.IsCharge, 
                parentProjectile.MaxHit, 
                null,
                parentProjectile.HitActions,
                ProjectileType.PlayerProjectile
            );
            p.Activate(0f);
            if (LevelUtilityV2.BonusInfo.bonusUnlockSkill.unlockNormalAttackPiercing)
                p.OnHit += () =>
                {
                    p.Damage = LevelUtilityV2.ToInt(p.Damage * LevelUtilityV2.GetNormalPiercingDamageScale());
                    p.CriticalDamage =
                        LevelUtilityV2.ToInt(p.Damage * LevelUtilityV2.GetNormalPiercingDamageScale());
                };
            
            // Đạn sẽ giảm dame, nếu có normal piercing thì vẫn apply
            baseDamage = LevelUtilityV2.ToInt(baseDamage * LevelUtilityV2.GetNormalBulletDamageScale());
            for (var i = 0; i <= amount; i++)
            {
                var p1 = ProjectilePool.Instance.Get(projectile, null, false);
                p1.transform.position = parentProjectile.transform.position;
                p1.transform.rotation = Quaternion.Euler(0f, 0f,  Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
                p1.Init(
                    parentProjectile.RangeCenter, 
                    direction, 
                    parentProjectile.Range, 
                    parentProjectile.Size, 
                    parentProjectile.SpeedScale, 
                    baseDamage, 
                    LevelUtilityV2.ToInt(baseDamage * LevelUtilityV2.GetBaseCriticalDmgScale()), 
                    parentProjectile.CriticalRate, 
                    parentProjectile.Stagger, 
                    parentProjectile.IsCharge, 
                    parentProjectile.MaxHit, 
                    null,
                    parentProjectile.HitActions,
                    ProjectileType.PlayerProjectile
                );
                if (characterAttackAnimDelay > 0.25f * i)
                    character?.PlayShoot((Vector2)character.transform.position + direction);
                else
                    character?.DelayCall(0.25f * i - characterAttackAnimDelay, () =>
                    {
                        character?.PlayShoot((Vector2)character.transform.position + direction);
                    });
                p1.Activate(0.25f * i);
                if (LevelUtilityV2.BonusInfo.bonusUnlockSkill.unlockNormalAttackPiercing)
                    p1.OnHit += () =>
                    {
                        p1.Damage = LevelUtilityV2.ToInt(p1.Damage *
                                                     LevelUtilityV2.GetNormalPiercingDamageScale());
                        p1.CriticalDamage = LevelUtilityV2.ToInt(p1.CriticalDamage *
                                                                 LevelUtilityV2.GetNormalPiercingDamageScale());
                    };
            }
        }

        public void Combine<T>(T combineWith) where T : IProjectileActivate
        {
            if (combineWith is not ProjectileActivateMultishot casted) return;
            this.amount += casted.amount;
        }

        public void TryCombineAndRevert<T>(T combineWith, Action combinedAction) where T : IProjectileActivate
        {
            if (combineWith is not ProjectileActivateMultishot casted)
            {
                combinedAction?.Invoke();
                return;
            }
            var tempAmount = this.amount;
            this.amount += casted.amount;
            combinedAction?.Invoke();
            this.amount = tempAmount;
        }

        public float GetValue()
        {
            return amount;
        }

        public IProjectileActivate Clone()
        {
            return new ProjectileActivateMultishot()
            {
                projectile = projectile,
                amount = amount,
            };
        }
    }
}