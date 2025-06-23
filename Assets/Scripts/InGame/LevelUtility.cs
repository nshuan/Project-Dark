namespace InGame
{
    public class LevelUtility
    {
        public static int GetPlayerBulletDamage(PlayerSkillConfig skillConfig)
        {
            return skillConfig.damePerBullet;
        }
    }
}