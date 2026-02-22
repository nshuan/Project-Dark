using System;
using System.Collections;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace InGame.Boss.BossSkillSystem
{
    public class BossSkillComponent : SerializedMonoBehaviour
    {
        [OdinSerialize, NonSerialized] private IBossSkill[] onSpawn;
        [OdinSerialize, NonSerialized] private IBossSkill[] onHit;
        [OdinSerialize, NonSerialized] private BossIntervalSkillInfo[] intervalSkill;

        public BossEntity BossScript { get; set; }
        
        public void TriggerSpawn(float delay)
        {
            StartCoroutine(IETriggerSkill(onSpawn, delay));
        }

        public void TriggerHit(float delay)
        {
            StartCoroutine(IETriggerSkill(onHit, delay));
        }

        public void StartInterval()
        {
            if (intervalSkill == null) return;
            foreach (var skill in intervalSkill)
            {
                StartCoroutine(IEInterval(skill.skills, skill.interval, skill.triggerFirst, skill.delay));
            }
        }
        
        private IEnumerator IETriggerSkill(IBossSkill[] skills, float delay)
        {
            if (skills == null || skills.Length == 0) yield break;
            
            yield return new WaitForSeconds(delay);
            foreach (var skill in skills)
            {
                skill.Attack(BossScript, BossScript.TargetTower);
            }
        }

        private IEnumerator IEInterval(IBossSkill[] skills, float interval, bool triggerFirst, float delay)
        {
            if (skills == null || skills.Length == 0) yield break;
            
            yield return new WaitForSeconds(delay);

            Action actionTriggerSkills = () =>
            {
                foreach (var skill in skills)
                {
                    skill.Attack(BossScript, BossScript.TargetTower);
                }
            };
            
            if (triggerFirst)
                actionTriggerSkills.Invoke();
            
            while (true)
            {
                yield return new WaitForSeconds(interval);
                actionTriggerSkills.Invoke();
            }
        }
    }

    [Serializable]
    public class BossIntervalSkillInfo
    {
        public float delay;
        public float interval;
        public bool triggerFirst;
        public IBossSkill[] skills;
    }
}