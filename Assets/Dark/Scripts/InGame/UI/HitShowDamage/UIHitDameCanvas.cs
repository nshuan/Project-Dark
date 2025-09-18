using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace InGame.UI.HitShowDamage
{
    public class UIHitDameCanvas : MonoBehaviour
    {
        [SerializeField] private Camera cam;
        
        [Space]
        [Header("Text color")]
        [SerializeField] private List<Color> colorInfos;
        [SerializeField] private int damageGap;

        public void ShowDamage(TextMeshProUGUI txtDamage, int damage, Vector3 worldPos)
        {
            StartCoroutine(IEShowDamage(txtDamage, damage, worldPos));
        }

        private IEnumerator IEShowDamage(TextMeshProUGUI txtDamage, int damage, Vector3 worldPos)
        {
            var indexColor = damage / damageGap;
            txtDamage.color = colorInfos[Math.Clamp(indexColor, 0, colorInfos.Count - 1)];
            txtDamage.transform.position = cam.WorldToScreenPoint(worldPos) + new Vector3(Random.Range(-30f, 30f), 0f, 0f);
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