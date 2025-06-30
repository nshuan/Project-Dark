using System;
using UnityEngine;

namespace InGame
{
    public class PlayerCharacter : MonoBehaviour
    {
        [SerializeField] private InputInGame inputInGame;
        [SerializeField] private PlayerAnimController animController;

        [Space] [Header("Config")] 
        [SerializeField] private Vector2 offset;
        
        private void Start()
        {
            inputInGame.CursorRangeCenter = transform;
            inputInGame.OnShootStart += PlayShoot;
            LevelManager.Instance.OnChangeTower += OnChangeTower;
        }

        private void OnDestroy()
        {
            inputInGame.OnShootStart -= PlayShoot;
        }

        private void PlayShoot(Vector2 target)
        {
            transform.localScale =
                new Vector3(Mathf.Sign(target.x - transform.position.x), 1f, 1f);
            animController.PlayAttack();
        }

        private void OnChangeTower(Transform tower)
        {
            transform.position = tower.position + (Vector3)offset;
        }
    }
}