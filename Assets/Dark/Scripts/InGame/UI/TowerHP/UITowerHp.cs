using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace InGame.UI
{
    public class UITowerHp : MonoBehaviour
    {
        [SerializeField] private int towerIndex;
        [SerializeField] private Image hpFill;
        [SerializeField] private Image hpGlow;
        [SerializeField] private Image iconGlowLowHp;

        [Space] [Header("Config")] 
        [SerializeField] private float warningThreshold;
        [SerializeField] private Color warningColor;
        [SerializeField] private Color glowStartColor;
        
        private TowerEntity tower;
        private bool isLowHp = false;
        
        private void Start()
        {
            if (towerIndex >= 0 && towerIndex < LevelManager.Instance.Towers.Length)
                tower = LevelManager.Instance.Towers[towerIndex];

            if (tower)
            {
                tower.OnHit += OnTowerHpChanged;
                tower.OnRegenerate += OnTowerHpChanged;
            }
        }

        private void OnTowerHpChanged(int valueChanged)
        {
            var fill = Mathf.Clamp((float)tower.CurrentHp / tower.MaxHp, 0f, 1f);
            hpFill.fillAmount = fill;

            if (fill <= warningThreshold)
            {
                var c = Color.Lerp(Color.white, warningColor,
                    (warningThreshold - fill) / warningThreshold);
                hpFill.color = c;
                c = Color.Lerp(glowStartColor, warningColor,
                    (warningThreshold - fill) / warningThreshold);
                c.a = 0.5f;
                hpGlow.color = c;
                
                if (!isLowHp)
                {
                    isLowHp = true;
                    DOTween.Kill(iconGlowLowHp);
                    iconGlowLowHp.SetAlpha(0f);
                    iconGlowLowHp.gameObject.SetActive(true);
                    iconGlowLowHp.DOFade(1f, 0.3f).SetTarget(iconGlowLowHp);
                }
                DOTween.Kill(this);
                transform.DOPunchScale(0.2f * Vector3.one, 0.5f).SetTarget(this);
            }
            else
            {
                hpFill.color = Color.white;
                hpGlow.color = glowStartColor;
                
                if (isLowHp)
                {
                    isLowHp = false;
                    DOTween.Kill(iconGlowLowHp);
                    iconGlowLowHp.DOFade(0f, 0.3f).SetTarget(iconGlowLowHp)
                        .OnComplete(() => iconGlowLowHp.gameObject.SetActive(false));
                }
            }
        }
    }
}