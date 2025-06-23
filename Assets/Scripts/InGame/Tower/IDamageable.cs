namespace InGame
{
    public interface IDamageable
    {
        void Damage(int damage);
        bool IsDestroyed { get; set; }
    }
}