using System;
using System.Collections;
using UnityEngine;

namespace Economic.InGame
{
    public class ClaimEchoeVisual : MonoBehaviour
    {
        [SerializeField] private GameObject vfxEchoe;
        [SerializeField] private float delayHide = 2f;

        private Coroutine coroutineShowVfx;
        
        private void Start()
        {
            WealthManager.Instance.OnUpGrade += OnEchoAdded;
        }

        private void OnDestroy()
        {
            WealthManager.Instance.OnUpGrade -= OnEchoAdded;
        }

        private void OnEchoAdded(int newEchoe)
        {
            if (coroutineShowVfx != null) StopCoroutine(coroutineShowVfx);
            vfxEchoe.SetActive(false);
            coroutineShowVfx = StartCoroutine(IEShowVfx());
        }

        private IEnumerator IEShowVfx()
        {
            yield return new WaitForEndOfFrame();
            vfxEchoe.SetActive(true);
            yield return new WaitForSeconds(delayHide);
            vfxEchoe.SetActive(false);
        }
    }
}