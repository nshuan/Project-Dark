using UnityEngine;

namespace InGame
{
    public class LevelUtility
    {
        public static (int, int) GetPlayerBulletDamage(int playerDamage, int skillDamage, float criticalDame)
        {
            var bulletDamage = playerDamage + skillDamage;
            return (bulletDamage, Mathf.RoundToInt(bulletDamage * criticalDame));
        }
    }
}