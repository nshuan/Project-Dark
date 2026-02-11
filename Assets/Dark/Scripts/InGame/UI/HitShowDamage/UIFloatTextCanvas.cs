using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace InGame.UI.HitShowDamage
{
    public class UIFloatTextCanvas : MonoBehaviour
    {
        [SerializeField] private Camera cam;

        public Camera Cam => cam;
        
        public void ShowText(TextMeshProUGUI tmp, string text, Vector3 worldPos, Color color, float scale = 1f, Action<TextMeshProUGUI> callbackComplete = null)
        {
            StartCoroutine(IEShowText(tmp, text, worldPos, color, scale, callbackComplete));
        }

        private IEnumerator IEShowText(TextMeshProUGUI tmp, string damage, Vector3 worldPos, Color color, float scale, Action<TextMeshProUGUI> callbackComplete)
        {
            tmp.color = color;
            tmp.transform.position = cam.WorldToScreenPoint(worldPos) + new Vector3(RandomUtil.Range(-30f, 30f), 0f, 0f);
            tmp.transform.localScale = scale * Vector3.one;
            tmp.SetText(damage);
            tmp.gameObject.SetActive(true);
            var endPos = tmp.transform.position + new Vector3(0f, 50f, 0f);
            var timer = 0f;
            
            while (timer < 0.6f)
            {
                tmp.transform.position = Vector2.Lerp(tmp.transform.position, endPos, Time.deltaTime * 3f);
                timer += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForEndOfFrame();
            callbackComplete?.Invoke(tmp);
        }
    }
}