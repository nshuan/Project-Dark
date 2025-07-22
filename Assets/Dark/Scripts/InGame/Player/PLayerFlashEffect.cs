using System.Collections;
using UnityEngine;

namespace InGame
{
    public class PLayerFlashEffect : MonoBehaviour
    {
        [SerializeField] private GameObject flashStartGO;
        [SerializeField] private GameObject flashEndGO;

        public float startDuration = 0.5f;

        public void PLayStart()
        {
            flashStartGO.transform.SetParent(null);
            flashStartGO.SetActive(true);
            StartCoroutine(IEHideObject(flashStartGO, 2f));
        }

        public void PLayEnd()
        {
            flashEndGO.transform.SetParent(null);
            flashEndGO.SetActive(true);
            StartCoroutine(IEHideObject(flashEndGO, 2f));
        }

        private IEnumerator IEHideObject(GameObject go, float delay)
        {
            yield return new WaitForSeconds(delay);
            go.SetActive(false);
            go.transform.SetParent(transform);
            go.transform.localPosition = Vector3.zero;
        }
    }
}