using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace InGame.UI.HitShowDamage
{
    [RequireComponent(typeof(Canvas))]
    public class UIHitDameCanvas : MonoBehaviour
    {
        private Camera cam;

        private void Awake()
        {
            cam = Camera.main;
        }

        public void ShowDamage(TextMeshProUGUI txtDamage, int damage, Vector3 worldPos)
        {
            StartCoroutine(IEShowDamage(txtDamage, damage, worldPos));
        }

        private IEnumerator IEShowDamage(TextMeshProUGUI txtDamage, int damage, Vector3 worldPos)
        {
            txtDamage.transform.position = cam.WorldToScreenPoint(worldPos);
            txtDamage.SetText(damage.ToString());
            txtDamage.gameObject.SetActive(true);
            var endPos = txtDamage.transform.position + new Vector3(0f, 50f, 0f);
            var timer = 0f;
            
            while (timer < 0.6f)
            {
                txtDamage.transform.position = Vector2.Lerp(txtDamage.transform.position, endPos, Time.deltaTime * 3f);
                timer += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForEndOfFrame();
            UIHitDameTextPool.Instance.Release(txtDamage);
        }
    }
}