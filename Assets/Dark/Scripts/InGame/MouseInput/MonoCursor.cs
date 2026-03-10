using System;
using Coffee.UIExtensions;
using Dark.Scripts.Settings;
using Dark.Scripts.Tutorial;
using Dark.Tools.Language.Runtime;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InGame
{
    public class MonoCursor : MonoBehaviour
    {
        [Header("Normal cursor")]
        public Image visual;
        public CanvasGroup content;
        [SerializeField] private float contentMaxScale;
        [SerializeField] private GameObject groupCooldown;
        [SerializeField] private GameObject cooldownGlow;
        [SerializeField] private Image cooldown;
        [SerializeField] private Color cooldownMaxColor;
        [SerializeField] private TextMeshProUGUI txtChargeUnitAdd;
        [SerializeField] private TextMeshProUGUI txtMax;
        [SerializeField] private TextMeshProUGUI txtReadyToCharge;
        [SerializeField] private UIParticle vfxReadyToCharge;
        [SerializeField] private TextMeshProUGUI txtAuto;

        [Header("Move cursor")] 
        public CanvasGroup contentMove;
        [SerializeField] private TextMeshProUGUI txtMoveInstruction;

        [Header("Collect cursor")] 
        public CanvasGroup contentAimAndMove;
        [SerializeField] private CanvasGroup contentCollect;

        private Vector3 defaultScale;

        private void Awake()
        {
            defaultScale = transform.localScale;
        }

        public void UpdateCooldown(bool active, float value)
        {
            groupCooldown.SetActive(active);
            cooldown.fillAmount = value;
            cooldown.color = Color.white;
        }

        public void UpdateChargeUnitAdd(bool active, int value = 1)
        {
            txtMax.gameObject.SetActive(false);
            cooldownGlow.gameObject.SetActive(false);
            txtChargeUnitAdd.gameObject.SetActive(active);
            txtChargeUnitAdd.SetText($"+{value}");
        }

        public void UpdateMax()
        {
            txtChargeUnitAdd.gameObject.SetActive(false);
            txtMax.gameObject.SetActive(true);
            cooldownGlow.gameObject.SetActive(true);
            cooldown.color = cooldownMaxColor;
        }

        public void UpdateScale(float value)
        {
            value = Mathf.Clamp(value, 0f, 1f);
            content.transform.localScale = Vector3.one * (1 + value);
        }

        public void SetAuto(bool active)
        {
            // Tạm thời tawts đi
            txtAuto.gameObject.SetActive(false);
            return;
            
            DOTween.Kill(txtAuto);
            if (active)
            {
                txtAuto.transform.localScale = 0.3f * Vector3.one;
                txtAuto.gameObject.SetActive(true);
                txtAuto.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack).SetTarget(txtAuto);
            }
            else
            {
                txtAuto.transform.DOScale(0.3f, 0.3f).SetEase(Ease.InBack).SetTarget(txtAuto)
                    .OnComplete(() => txtAuto.gameObject.SetActive(false));
            }
        }

        public void SetReadyToCharge()
        {
            DOTween.Kill(txtReadyToCharge);
            
            txtChargeUnitAdd.SetAlpha(0f);
            txtMax.SetAlpha(0f);
            txtAuto.SetAlpha(0f);
            txtReadyToCharge.SetAlpha(1f);
            txtReadyToCharge.font = LanguageData.Instance.GetFontAssetRuntime(LanguageManager.Instance.CurrentLanguage);
            var text = LanguageData.Instance.GetLocalizedString("key_charge_ready",
                LanguageManager.Instance.CurrentLanguage);
            var textPart = text.Split(" ");
            if (textPart.Length == 2) text = textPart[0] + "\n" + textPart[1];
            else if (textPart.Length > 2)
            {
                text = textPart[0] + " " + textPart[1] + "\n" + textPart[2];
                for (var i = 3; i < textPart.Length; i++)
                {
                    text += " " + textPart[i];
                }
            }
            txtReadyToCharge.SetText(text);
            txtReadyToCharge.gameObject.SetActive(true);
            vfxReadyToCharge.Play();

            DOTween.Sequence().SetTarget(txtReadyToCharge)
                .Append(txtReadyToCharge.transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.2f))
                .AppendInterval(1f)
                .AppendCallback(() =>
                {
                    txtChargeUnitAdd.SetAlpha(1f);
                    txtMax.SetAlpha(1f);
                    txtAuto.SetAlpha(1f);
                    txtReadyToCharge.SetAlpha(0f);
                    txtReadyToCharge.gameObject.SetActive(false);
                });
        }

        public void SetMoveCursor(bool active, int towerId)
        {
            if (active)
            {
                content.alpha = 0f;
                contentMove.alpha = 1f;
                
                if (UITutorialStepMoveTowers.ShouldShowHotKeyInstruction)
                {
                    var keyMoveTower = towerId switch
                    {
                        0 => GameSettings.KeyMoveTower0,
                        1 => GameSettings.KeyMoveTower1,
                        2 => GameSettings.KeyMoveTower2,
                        3 => GameSettings.KeyMoveTower3,
                        _ => GameSettings.KeyMoveTower0
                    };
                    
                    var keyMoveTowerStr = keyMoveTower.ToString();
                    if (keyMoveTower is >= KeyCode.Alpha0 and <= KeyCode.Alpha9)
                        keyMoveTowerStr = (keyMoveTower - KeyCode.Alpha0).ToString();
                    txtMoveInstruction.gameObject.SetActive(true);
                    txtMoveInstruction.SetTextLanguage("key_tutorial_move_tower", ("%{value}", keyMoveTowerStr));
                }
            }
            else
            {
                content.alpha = 1f;
                contentMove.alpha = 0f;
                
                txtMoveInstruction.gameObject.SetActive(false);
            }
        }

        public void SetCollectCursor(bool active)
        {
            if (active)
            {
                contentAimAndMove.alpha = 0f;
                contentCollect.alpha = 1f;
            }
            else
            {
                contentAimAndMove.alpha = 1f;
                contentCollect.alpha = 0f;
            }
        }

        public void PunchCollectCursor()
        {
            DOTween.Kill(contentCollect, true);
            contentCollect.transform.DOPunchScale(0.3f * Vector3.one, 0.13f).SetEase(Ease.InQuad)
                .SetTarget(contentCollect);
        }

        public void SetDefaultScale()
        {
            transform.localScale = defaultScale;
        }
    }
}