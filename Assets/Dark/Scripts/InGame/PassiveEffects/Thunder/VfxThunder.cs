using System;
using System.Collections;
using UnityEngine;

namespace InGame
{
    public class VfxThunder : MonoBehaviour
    {
        [SerializeField] private LineRenderer[] lines;
        [SerializeField] private float topYGlobal;
        [SerializeField] private float delayHideLine = 0.1f;

        private void OnEnable()
        {
            foreach (var line in lines)
            {
                var position1 = line.GetPosition(1);
                position1.y = topYGlobal - transform.position.y;
                line.SetPosition(1, position1);
                line.gameObject.SetActive(true);
            }

            StartCoroutine(IEFlashLine(delayHideLine));
        }

        private IEnumerator IEFlashLine(float delay)
        {
            yield return new WaitForSeconds(delay);
            foreach (var line in lines)
                line.gameObject.SetActive(false);
        }
    }
}