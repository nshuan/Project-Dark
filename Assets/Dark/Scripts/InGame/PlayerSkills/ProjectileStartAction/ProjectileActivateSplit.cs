using System;
using UnityEngine;

namespace InGame
{
    [Serializable]
    public class ProjectileActivateSplit : IProjectileActivate
    {
        public ProjectileEntity projectile;
        public int amount;
        [Range(0f, 180f)] 
        public float angle;
        
        public void DoAction(ProjectileEntity parentProjectile, Vector2 direction)
        {
            if (amount == 0) return;
            if (angle == 0) return;
            if (!projectile) return;
            
            direction.Normalize();

            var spawnPos = new Vector2();
            var calculatedAmount = amount + 1; // +1 đạn chính
            var angleOffset = 0f;
            
            var baseDamage = parentProjectile.Damage;
            if (calculatedAmount % 2 == 1)
            {
                var p = ProjectilePool.Instance.Get(projectile, null, false);
                spawnPos.x = parentProjectile.transform.position.x;
                spawnPos.y = parentProjectile.transform.position.y;
                p.transform.rotation = Quaternion.Euler(0f, 0f,  Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
                p.transform.position = spawnPos;
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
            
                calculatedAmount -= 1;
            }
            else
            {
                angleOffset = angle / calculatedAmount / 2;
            }

            // Đạn tỏa ra sẽ giảm dame, nếu có normal piercing thì vẫn apply
            baseDamage = LevelUtilityV2.ToInt(baseDamage * LevelUtilityV2.GetNormalBulletDamageScale());
            for (var i = 1; i <= calculatedAmount / 2; i++)
            {
                var pDir = (Vector2)(Quaternion.Euler(0f, 0f, i * angle / calculatedAmount - angleOffset) * direction);
                var p1 = ProjectilePool.Instance.Get(projectile, null, false);
                spawnPos.x = parentProjectile.transform.position.x;
                spawnPos.y = parentProjectile.transform.position.y;
                p1.transform.position = spawnPos;
                p1.transform.rotation = Quaternion.Euler(0f, 0f,  Mathf.Atan2(pDir.y, pDir.x) * Mathf.Rad2Deg);
                p1.Init(
                    parentProjectile.RangeCenter, 
                    pDir, 
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
                p1.Activate(0f);
                if (LevelUtilityV2.BonusInfo.bonusUnlockSkill.unlockNormalAttackPiercing)
                    p1.OnHit += () =>
                    {
                        p1.Damage = LevelUtilityV2.ToInt(p1.Damage *
                                                     LevelUtilityV2.GetNormalPiercingDamageScale());
                        p1.CriticalDamage = LevelUtilityV2.ToInt(p1.CriticalDamage *
                                                                 LevelUtilityV2.GetNormalPiercingDamageScale());
                    };
            
                pDir = Quaternion.Euler(0f, 0f, - i * angle / calculatedAmount + angleOffset) * direction;
                var p2 = ProjectilePool.Instance.Get(projectile, null, false);
                spawnPos.x = parentProjectile.transform.position.x;
                spawnPos.y = parentProjectile.transform.position.y;
                p2.transform.position = spawnPos;
                p2.transform.rotation = Quaternion.Euler(0f, 0f,  Mathf.Atan2(pDir.y, pDir.x) * Mathf.Rad2Deg);
                p2.Init(
                    parentProjectile.RangeCenter, 
                    pDir, 
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
                p2.Activate(0f);
                if (LevelUtilityV2.BonusInfo.bonusUnlockSkill.unlockNormalAttackPiercing)
                    p2.OnHit += () =>
                    {
                        p2.Damage = Mathf.RoundToInt(p2.Damage *
                                                     LevelUtilityV2.GetNormalPiercingDamageScale());
                    };
            }
        }

        public void Combine<T>(T combineWith) where T : IProjectileActivate
        {
            if (combineWith is not ProjectileActivateSplit casted) return;
            this.amount += casted.amount;
            this.angle += casted.angle;
        }

        public void TryCombineAndRevert<T>(T combineWith, Action combinedAction) where T : IProjectileActivate
        {
            if (combineWith is not ProjectileActivateSplit casted)
            {
                combinedAction?.Invoke();
                return;
            }
            var tempAmount = this.amount;
            var tempAngle = this.angle;
            this.amount += casted.amount;
            this.angle += casted.angle;
            combinedAction?.Invoke();
            this.amount = tempAmount;
            this.angle = tempAngle;
        }

        public float GetValue()
        {
            return amount;
        }

        public IProjectileActivate Clone()
        {
            return new ProjectileActivateSplit()
            {
                projectile = projectile,
                amount = amount,
                angle = angle,
            };
        }

        public float GetAttackDuration()
        {
            return 0f;
        }
    }
}