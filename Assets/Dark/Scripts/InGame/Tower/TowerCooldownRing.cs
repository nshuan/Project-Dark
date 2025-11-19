using System;
using System.Collections;
using UnityEngine;

namespace InGame
{
    public class TowerCooldownRing : MonoBehaviour
    {
        private static readonly int RadialProgress = Shader.PropertyToID("_RadialProgress");
        
        [SerializeField] private TowerEntity tower;
        [SerializeField] private SpriteRenderer cooldownRing;
        [SerializeField] private ParticleSystem vfxCooldownComplete;

        private Material ringMaterial;
        private bool inProgress;
        private float cooldown;
        private float cooldownCounter;

        private void Awake()
        {
            ringMaterial = cooldownRing.material;
        }

        private void Start()
        {
            CombatActions.OnMoveTower += OnMoveTower;
        }

        private void OnDestroy()
        {
            CombatActions.OnMoveTower -= OnMoveTower;
        }

        private void Update()
        {
            if (!inProgress) return;

            if (cooldownCounter > 0)
            {
                cooldownCounter -= Time.deltaTime;
                ringMaterial.SetFloat(RadialProgress, (1f - cooldownCounter / cooldown) * 360f);
            }
            else
            {
                inProgress = false;
                vfxCooldownComplete.Play(true);
            }
        }

        private void OnMoveTower(float cooldown)
        {
            if (tower.Id != LevelManager.Instance.CurrentTower.Id) return;
            
            ringMaterial.SetFloat(RadialProgress, 0f);
            this.cooldown = cooldown;
            cooldownCounter = cooldown;
            inProgress = true;
        }
    }
}