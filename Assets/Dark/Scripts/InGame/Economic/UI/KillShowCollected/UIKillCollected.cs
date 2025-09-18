using System;
using System.Collections;
using InGame.UI.HitShowDamage;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace InGame.UI.Economic.KillShowCollected
{
    public class UIKillCollected : MonoBehaviour
    {
        [SerializeField] private Camera cam;
        
        [Space]
        [Header("Text color")]
        [SerializeField] private Color textColor;

        public void ShowCollected(TextMeshProUGUI txtValue, int value, Vector3 worldPos)
        {
            StartCoroutine(IEShowCollected(txtValue, value, worldPos));
        }

        private IEnumerator IEShowCollected(TextMeshProUGUI txtValue, int value, Vector3 worldPos)
        {
            txtValue.color = textColor;
            txtValue.transform.position = cam.WorldToScreenPoint(worldPos) + new Vector3(Random.Range(-30f, 30f), 40f, 0f);
            txtValue.SetText($"+{value}");
            txtValue.gameObject.SetActive(true);
            var endPos = txtValue.transform.position + new Vector3(0, 50f, 0f);
            var timer = 0f;
            
            while (timer < 0.6f)
            {
                txtValue.transform.position = Vector2.Lerp(txtValue.transform.position, endPos, Time.deltaTime * 3f);
                timer += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForEndOfFrame();
            UIKillCollectedPool.Instance.Release(txtValue);
        }
    }
}