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
                yield return new WaitForSeconds(cooldown);
                yield return new WaitForEndOfFrame();
                
                EItemDropManager.Instance.CollectAll(visualTarget, true);
            }
        }
    }
}