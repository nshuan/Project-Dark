using System.Collections.Generic;
using Dark.Scripts.AudioV2;
using UnityEngine;

namespace InGame
{
    public class TowerCooldownRing : MonoBehaviour
    {
        private static readonly int RadialProgress = Shader.PropertyToID("_RadialProgress");
        
        [SerializeField] private TowerEntity tower;
        [SerializeField] private SpriteRenderer cooldownRing;
        [SerializeField] private ParticleSystem vfxCooldownComplete;
        [SerializeField] private GameObject vfxSelected;
        [SerializeField] private AudioPlayComponentV2 sfxCooldown;

        [Header("Setup Materials")]
        [SerializeField] private List<SpriteRenderer> cdRingSprites;
        [SerializeField] private List<ParticleSystem> cdRingParticles;
        
        private Material ringMaterial;
        private bool inProgress;
        private float cooldown;
        private float cooldownCounter;

        private void Awake()
        {
            ringMaterial = new Material(cooldownRing.sharedMaterial);

            foreach (var spriteRenderer in cdRingSprites)
            {
                spriteRenderer.material = ringMaterial;
            }

            foreach (var ps in cdRingParticles)
            {
                ps.GetComponent<ParticleSystemRenderer>().material = ringMaterial;
            }
        }

        private void Start()
        {
            CombatActions.OnMoveTowerComplete += OnMoveTower;
        }

        private void OnDestroy()
        {
            CombatActions.OnMoveTowerComplete -= OnMoveTower;
        }

        private void Update()
        {
            if (!inProgress) return;

            if (cooldownCounter > 0)
            {
                cooldownCounter -= Time.deltaTime;
                ringMaterial.SetFloat(RadialProgress, (1f - cooldownCounter / cooldown) * -360f);
            }
            else
            {
                inProgress = false;
                ringMaterial.SetFloat(RadialProgress, 0f);
                vfxCooldownComplete.Play(true);
                sfxCooldown.Play();
            }
        }

        private void OnMoveTower(float cooldown)
        {
            if (tower.Id == LevelManager.Instance.CurrentTower.Id)
            {
                // vfxSelected.SetActive(false);
                return;
            }
            
            // vfxSelected.SetActive(false);
            ringMaterial.SetFloat(RadialProgress, 0f);
            this.cooldown = cooldown;
            cooldownCounter = cooldown;
            inProgress = true;
        }
    }
}