using System;
using Economic.InGame;
using InGame;
using UnityEngine;

namespace Dark.Scripts.Tutorial
{
    public class UITutorialStepCollect : UIAbstractTutorialStepInGame
    {
        [SerializeField] private GameObject objInstruction;

        [Space] [Header("Config")] 
        [SerializeField] private int amountToShowInstruction = 2;
        
        private int totalVestigeDropAmount;
        private Transform instructionTarget;
        [SerializeField] private Camera mainCamera;

        private void Awake()
        {
            CombatActions.OnResourceCollectorInitialized += OnResourceCollectorInitialized;
        }

        public override bool IsValid()
        {
            return true;
        }

        public override void Setup()
        {
            CombatActions.OnDropResource += OnDropResource;
        }

        private void OnDropResource(EnemyEntity enemy, bool hasVestige)
        {
            if (hasVestige)
                totalVestigeDropAmount += enemy.Dark;
            if (totalVestigeDropAmount >= amountToShowInstruction)
            {
                CombatActions.OnDropResource -= OnDropResource;
                objInstruction.SetActive(true);
                CombatActions.OnResourceCollectorDamaged += OnCollectEntityDamaged;
            }
        }

        private void OnResourceCollectorInitialized(EItemDropCollector collector)
        {
            CombatActions.OnResourceCollectorInitialized -= OnResourceCollectorInitialized;
            instructionTarget = collector.transform;
        }

        private void OnCollectEntityDamaged(EItemDropCollector collector)
        {
            CombatActions.OnResourceCollectorDamaged -= OnCollectEntityDamaged;
            OnComplete?.Invoke();
        }

        private void Update()
        {
            if (!objInstruction.activeInHierarchy) return;
            if (!instructionTarget) return;
            if (!mainCamera) return;
            objInstruction.transform.position = mainCamera.WorldToScreenPoint(instructionTarget.position);
            // actionUpdateFocus?.Invoke(
            //     objInstruction.transform.localPosition - new Vector3(0f, 96f, 0f),
            //     new Vector2(0.03f, 0.05f),
            //     2f,
            //     false, false);
        }
    }
}