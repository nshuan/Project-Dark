using System;
using InGame.Effects;
using UnityEngine;

namespace InGame
{
    public class PlayerCharacter : MonoBehaviour
    {
        [SerializeField] private PlayerAnimController animController;
        [SerializeField] private DashGhostEffect dashEffect;

        [Space] [Header("Config")] 
        [SerializeField] private Vector2 offset;

        [Space] [Header("Visual")] 
        [SerializeField] private Transform range;
        
        private void Start()
        {
            LevelManager.Instance.OnChangeSkill += OnChangeSkill;
        }

        // Return the duration to finish the 1st animation phase, when the skill is actually strike
        public float PlayShoot(Vector2 target)
        {
            transform.localScale =
                new Vector3(Mathf.Sign(target.x - transform.position.x), 1f, 1f);
            return animController.PlayAttack();
        }

        private void OnChangeSkill(PlayerSkillConfig skillConfig)
        {
            range.localScale = skillConfig.range * Vector3.one;
        }

        public void SetRangeVisual(bool active)
        {
            range.gameObject.SetActive(active);
        }

        public void PlayDashEffect()
        {
            dashEffect.DoEffect();
        }

        public void StopDashEffect()
        {
            dashEffect.StopEffect();
        }
    }
}