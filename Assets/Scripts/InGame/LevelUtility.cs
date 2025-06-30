using UnityEngine;

namespace InGame
{
    public class LevelUtility
    {
        public static (int, int) GetPlayerBulletDamage(int playerDamage, int skillDamage, float criticalDameMultiplier, float chargeDameMultiplier)
        {
            var bulletDamage = playerDamage + skillDamage;
            return (
                Mathf.RoundToInt(bulletDamage * chargeDameMultiplier), 
                Mathf.RoundToInt(bulletDamage * criticalDameMultiplier * chargeDameMultiplier)
                );
        }
    }
}