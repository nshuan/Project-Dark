using UnityEngine;

namespace InGame
{
    public class EnemyActionEffectTrigger : ActionEffectTrigger
    {
        public EnemyEntity Enemy { get; set; }

        protected override void Setup(ActionEffectConfig effectConfig)
        {
            if (!Enemy) return;
            
            effectConfig.logicType.Initialize();
            
            switch (effectConfig.triggerType)
            {
                case ActionEffectTriggerType.TriggerOnDie:
                    Enemy.OnDead += () => ActionEffectManager.Instance.CheckAndTrigger(transform.position, effectConfig);
                    break;
            }
        }
    }
}