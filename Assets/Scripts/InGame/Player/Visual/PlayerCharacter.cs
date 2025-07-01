using System;
using UnityEngine;

namespace InGame
{
    public class PlayerCharacter : MonoBehaviour
    {
        [SerializeField] private PlayerAnimController animController;

        [Space] [Header("Config")] 
        [SerializeField] private Vector2 offset;

        [Space] [Header("Visual")] 
        [SerializeField] private Transform range;
        
        private void Start()
        {
            LevelManager.Instance.OnChangeTower += OnChangeTower;
            LevelManager.Instance.OnChangeSkill += OnChangeSkill;
        }

        public void PlayShoot(Vector2 target)
        {
            transform.localScale =
                new Vector3(Mathf.Sign(target.x - transform.position.x), 1f, 1f);
            animController.PlayAttack();
        }

        private void OnChangeTower(Transform tower)
        {
            transform.position = tower.position + (Vector3)offset;
        }

        private void OnChangeSkill(PlayerSkillConfig skillConfig)
        {
            range.localScale = skillConfig.range * Vector3.one;
        }

        public void SetRangeVisual(bool active)
        {
            range.gameObject.SetActive(active);
        }
    }
}