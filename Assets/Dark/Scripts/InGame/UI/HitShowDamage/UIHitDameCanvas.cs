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
        [Header("Text config")]
        [SerializeField] private List<float> scaleInfos;
        [SerializeField] private Color criticalColor;
        [SerializeField] private int damageGap;

        public void ShowDamage(TextMeshProUGUI txtDamage, int damage, Vector3 worldPos, DamageType dmgType)
        {
            StartCoroutine(IEShowDamage(txtDamage, damage, worldPos, dmgType));
        }

        private IEnumerator IEShowDamage(TextMeshProUGUI txtDamage, int damage, Vector3 worldPos, DamageType dmgType)
        {
            var indexScale = damage / damageGap;
            txtDamage.color = dmgType == DamageType.NormalCritical ? criticalColor : Color.white;
            txtDamage.transform.position = cam.WorldToScreenPoint(worldPos) + new Vector3(Random.Range(-30f, 30f), 0f, 0f);
            txtDamage.transform.localScale = scaleInfos[Math.Clamp(indexScale, 0, scaleInfos.Count - 1)] * Vector3.one;
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