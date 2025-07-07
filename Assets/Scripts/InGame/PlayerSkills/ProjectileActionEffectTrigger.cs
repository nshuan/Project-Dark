namespace InGame
{
    public class ProjectileActionEffectTrigger : ActionEffectTrigger
    {
        public ProjectileEntity Projectile { get; set; }
        
        protected override void Setup(ActionEffectConfig effectConfig)
        {
            if (!Projectile) return;
            
            effectConfig.logicType.Initialize();

            switch (effectConfig.triggerType)
            {
                case ActionEffectTriggerType.TriggerOnDie:
                    Projectile.OnHit += () =>
                        ActionEffectManager.Instance.CheckAndTrigger(transform.position, effectConfig);
                    break;
            }
        }
    }
}