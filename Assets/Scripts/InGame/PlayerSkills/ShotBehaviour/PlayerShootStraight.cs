using UnityEngine;

namespace InGame
{
    [CreateAssetMenu(menuName = "InGame/Player Skill/Player Shoot Straight", fileName = "PlayerShootStraight")]
    public class PlayerShootStraight : PlayerSkillBehaviour
    {
        public override void Shoot(Vector2 spawnPos, Vector2 target)
        {
            var p = Object.Instantiate(projectilePrefab, spawnPos, Quaternion.identity, null);
            p.gameObject.SetActive(false);
            p.Init(5f, target);
            p.Activate();
        }
    }
}