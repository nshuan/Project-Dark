namespace NeatBullets.Core.Scripts.Interfaces {
    
    public interface IDamagable {
        public float HealthPoints { get; set; }
        public void TakeDamage(float damage);
    }
}


