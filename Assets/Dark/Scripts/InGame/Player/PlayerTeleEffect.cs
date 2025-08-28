using System.Collections;
using Dark.Scripts.Audio;
using UnityEngine;

namespace InGame
{
    public class PlayerTeleEffect : MonoBehaviour
    {
        [SerializeField] private GameObject teleStartChargingGO;
        [SerializeField] private GameObject teleStartGO;
        [SerializeField] private GameObject teleEndChargingGO;
        [SerializeField] private GameObject teleEndGO;
        [SerializeField] private AudioComponent sfxTele;
        [SerializeField] private AudioComponent sfxTeleEnd;
        
        public float startDuration = 0.5f;

        public void PlayStartCharging()
        {
            teleStartChargingGO.transform.SetParent(null);
            teleStartChargingGO.SetActive(true);
            StartCoroutine(IEHideObject(teleStartChargingGO, 2f));
        }
        
        public void PLayStart()
        {
            teleStartGO.transform.SetParent(null);
            teleStartGO.SetActive(true);
            StartCoroutine(IEHideObject(teleStartGO, 2f));
            sfxTele.Play();
        }

        public void PlayEndCharging(Vector2 position)
        {
            teleEndChargingGO.transform.SetParent(null);
            teleEndChargingGO.transform.position = position;
            teleEndChargingGO.transform.SetParent(null);
            teleEndChargingGO.SetActive(true);
            StartCoroutine(IEHideObject(teleEndChargingGO, 2f));
        }
        
        public void PLayEnd()
        {
            teleEndGO.transform.SetParent(null);
            teleEndGO.SetActive(true);
            StartCoroutine(IEHideObject(teleEndGO, 1.5f));
            sfxTeleEnd.Play();
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