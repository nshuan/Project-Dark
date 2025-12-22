using System;
using System.Collections;
using Dark.Scripts.ForDemo;
using Economic.InGame.DropItems;
using InGame;
using UnityEngine;

namespace Economic.InGame
{
    public class EItemDropAutoCollect : MonoBehaviour
    {
        [SerializeField] private float cooldown;

        private Coroutine coroutineAutoCollect;
        private Transform visualTarget;
        
        private void Awake()
        {
            LevelManager.Instance.OnLevelLoaded += OnLevelLoaded;
            LevelManager.Instance.OnWin += OnWin;
            LevelManager.Instance.OnLose += OnLose;
            CombatActions.OnResourceCollectorInitialized += OnItemCollectorInitialized;
        }

        private void OnItemCollectorInitialized(EItemDropCollector collector)
        {
            visualTarget = collector.transform;
        }
        
        private void OnLose()
        {
            LevelManager.Instance.OnLevelLoaded -= OnLevelLoaded;
            CombatActions.OnResourceCollectorInitialized -= OnItemCollectorInitialized;
            if (coroutineAutoCollect != null) StopCoroutine(coroutineAutoCollect);
        }

        private void OnWin()
        {
            LevelManager.Instance.OnLevelLoaded -= OnLevelLoaded;
            CombatActions.OnResourceCollectorInitialized -= OnItemCollectorInitialized;
            if (coroutineAutoCollect != null) StopCoroutine(coroutineAutoCollect);
        }

        private void OnLevelLoaded(LevelConfig level)
        {
            if (DemoConfig.CollectLogicType == 1)
            {
                if (coroutineAutoCollect != null) StopCoroutine(coroutineAutoCollect);
                coroutineAutoCollect = StartCoroutine(IEAutoCollect());
            }
        }

        private IEnumerator IEAutoCollect()
        {
            while (true)
            {
                if (!enableAutoCollect) yield return null;
                yield return new WaitForSeconds(cooldown);
                yield return new WaitForEndOfFrame();
                
                if (!enableAutoCollect) yield return null;
                if (visualTarget)
                    EItemDropManager.Instance.CollectAll(visualTarget, true);
            }
        }

#if UNITY_EDITOR
        private bool enableAutoCollect = true;
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
                enableAutoCollect = !enableAutoCollect;
                
        }
#endif
    }
}