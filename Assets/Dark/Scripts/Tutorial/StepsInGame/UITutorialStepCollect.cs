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

        private void OnDropResource(EnemyEntity enemy)
        {
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
            if (mainCamera)
                actionUpdateFocus?.Invoke(mainCamera.WorldToScreenPoint(collector.transform.position), false, false);
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
        }
    }
}