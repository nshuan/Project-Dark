namespace InGame.Boss.BossSkillSystem
{
    public interface IBossSkill
    {
        void Attack(EnemyEntity boss, TowerEntity target);
    }
}