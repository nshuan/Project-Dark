using UnityEngine;

namespace InGame
{
    public class LevelTemporaryUtility
    {
        public static bool activatedTemporaryDamageOnMove;
        public static bool activatedTemporaryDamageOnKill;
        public static bool activatedTemporaryAtkSpeOnMove;
        public static bool activatedTemporaryAtkSpeOnKill;

        public static (int, int) FilterPlayerBulletDamage(int damage, int criticalDamage, UpgradeBonusInfo bonusInfo)
        {
            if (activatedTemporaryDamageOnKill)
            {
                damage = (int)((1f + bonusInfo.tempDamageBonusOnKill.bonusValue) * damage);
                criticalDamage = (int)((1f + bonusInfo.tempDamageBonusOnKill.bonusValue) * criticalDamage);
            }

            if (activatedTemporaryDamageOnMove)
            {
                damage = (int)((1f + bonusInfo.tempDamageBonusOnMove.bonusValue) * damage);
                criticalDamage = (int)((1f + bonusInfo.tempDamageBonusOnMove.bonusValue) * criticalDamage);
            }
            
            return (damage, criticalDamage);
        }

        public static float FilterSkillCooldown(float cooldown, UpgradeBonusInfo bonusInfo)
        {
            if (activatedTemporaryAtkSpeOnKill)
            {
                cooldown = 1f / (1f / cooldown * (1f + bonusInfo.tempAtkSpeBonusOnKill.bonusValue));
            }

            if (activatedTemporaryAtkSpeOnMove)
            {
                cooldown = 1f / (1f / cooldown * (1f + bonusInfo.tempAtkSpeBonusOnMove.bonusValue));
            }

            return cooldown;
        }
    }
}