using System;
using System.Collections;
using Economic.InGame.DropItems;
using InGame;
using UnityEngine;

namespace Economic.InGame
{
    public class EItemDropAutoCollect : MonoBehaviour
    {
        [SerializeField] private Transform visualTarget;
        [SerializeField] private float cooldown;

        private Coroutine coroutineAutoCollect;
        
        private void Awake()
        {
            LevelManager.Instance.OnLevelLoaded += OnLevelLoaded;
            LevelManager.Instance.OnWin += OnWin;
            LevelManager.Instance.OnLose += OnLose;
        }

        private void OnLose()
        {
            LevelManager.Instance.OnLevelLoaded -= OnLevelLoaded;
            if (coroutineAutoCollect != null) StopCoroutine(coroutineAutoCollect);
        }

        private void OnWin()
        {
            LevelManager.Instance.OnLevelLoaded -= OnLevelLoaded;
            if (coroutineAutoCollect != null) StopCoroutine(coroutineAutoCollect);
        }

        private void OnLevelLoaded(LevelConfig level)
        {
            if (coroutineAutoCollect != null) StopCoroutine(coroutineAutoCollect);
            coroutineAutoCollect = StartCoroutine(IEAutoCollect());
        }

        private IEnumerator IEAutoCollect()
        {
            while (true)
            {
                if (!enableAutoCollect) yield return null;
                yield return new WaitForSeconds(cooldown);
                yield return new WaitForEndOfFrame();
                
                if (!enableAutoCollect) yield return null;
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