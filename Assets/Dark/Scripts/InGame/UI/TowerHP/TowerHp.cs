using System;
using UnityEngine;

namespace InGame.UI
{
    public class TowerHp : MonoBehaviour
    {
        [SerializeField] private TowerEntity tower;
        [SerializeField] private SpriteRenderer hpFill;
        [SerializeField] private SpriteRenderer hpGlow;

        [Space] [Header("Config")] 
        [SerializeField] private float warningThreshold;
        [SerializeField] private Color warningColor;
        [SerializeField] private Color glowStartColor;

        private Vector3 tempHpScale;
        
        private void Start()
        {
            tower.OnHit += OnHit;
            tower.OnRegenerate += OnTowerHpChanged;
        }

        private void OnHit(int damage, DamageType dmgType)
        {
            OnTowerHpChanged(damage);    
        }
        
        private void OnTowerHpChanged(int valueChanged)
        {
            tempHpScale.x = hpFill.transform.localScale.x;
            tempHpScale.y = hpFill.transform.localScale.y;
            tempHpScale.z = hpFill.transform.localScale.z;
            tempHpScale.y = Mathf.Clamp((float)tower.CurrentHp / tower.MaxHp, 0f, 1f);
            hpFill.transform.localScale = tempHpScale;

            if (tempHpScale.y <= warningThreshold)
            {
                var c = Color.Lerp(Color.white, warningColor,
                    (warningThreshold - tempHpScale.y) / warningThreshold);
                hpFill.color = c;
                c = Color.Lerp(glowStartColor, warningColor,
                    (warningThreshold - tempHpScale.y) / warningThreshold);
                c.a = 0.5f;
                hpGlow.color = c;
            }
            else
            {
                hpFill.color = Color.white;
                hpGlow.color = glowStartColor;
            }
        }
    }
}