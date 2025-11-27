using System;
using Dark.Scripts.CoreUI;
using Dark.Scripts.SceneNavigation;
using Dark.Scripts.Utils;
using Data;
using DG.Tweening;
using InGame.UI.EndingLevel;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InGame.UI
{
    public class PopupLose : MonoBehaviour
    {
        [SerializeField] private UIPopup ui;
        [SerializeField] private CanvasGroup popupLoseCanvasGroup;
        [SerializeField] private float delayShowPopup = 5f; // Do có vfx endgame khi trụ bị phá nên cần delay xong vfx mới show popup
        
        [Space]
        [SerializeField] private Button btnBackToTree;
        [SerializeField] private Button btnReplay;
        
        private IEndGameLoseAnimation endingLevel;

        public static event Action onShowPopup;

        private void Awake()
        {
            matSymbol = new Material(imgSymbol.material);
            imgSymbol.material = matSymbol;
        }

        private void Start()
        {
            LevelManager.Instance.OnLose += OnLose;
        }

        private void OnDestroy()
        {
            onShowPopup = null;
        }

        [Button]
        private void OnLose()
        {
            UpdateUI();

            delayShowPopup = 0f;
            endingLevel = TowerDestroyedAnim.Instance;
            if (endingLevel != null) delayShowPopup = endingLevel.Play();
            
            ui.DoOpenFadeIn().SetDelay(delayShowPopup).OnComplete(() =>
            {
                onShowPopup?.Invoke();
                DoShowUIPopup();
            });
        }

        private void UpdateUI()
        {
            ResetPopupUI();
            
            btnBackToTree.onClick.RemoveAllListeners();
            btnBackToTree.onClick.AddListener(() =>
            {
                btnReplay.interactable = false;
                btnBackToTree.interactable = false;;
                popupLoseCanvasGroup.DOFade(0f, 0.2f)
                    .OnComplete(() =>
                    {
                        var returnDuration = endingLevel.PlayReturn();
                        Loading.Instance.QuickLoadScene(SceneConstants.SceneUpgrade, null, returnDuration + 0.2f);
                    });
            });
            
            // Todo reload level
            btnReplay.onClick.RemoveAllListeners();
            btnReplay.onClick.AddListener(() =>
            {
                btnReplay.interactable = false;
                btnBackToTree.interactable = false;
                Loading.Instance.QuickLoadScene(SceneConstants.SceneInGame, () =>
                {
                    LevelManager.Instance.LoadLevel(PlayerDataManager.Instance.Data.level + 1);
                });
            });
        }

        [Space] [Header("UI Tween")] 
        private static readonly int MatDisolveValue = Shader.PropertyToID("Disolve_Value");
        
        [SerializeField] private Image imgSymbol;
        [SerializeField] private Image imgSymbolShiny;
        [SerializeField] private Image imgTitle;
        [SerializeField] private Image imgTitleBg;
        [SerializeField] private TextMeshProUGUI txtDescription;
        [SerializeField] private TextMeshProUGUI txtTitleResourceCollected;
        [SerializeField] private CanvasGroup groupResourceCollected;
        [SerializeField] private CanvasGroup groupTimePlayed;
        [SerializeField] private Transform rectLine;
        [SerializeField] private CanvasGroup groupBtnBackToTree;
        [SerializeField] private CanvasGroup groupBtnReplay;

        [Header("UI Tween Config")] 
        [SerializeField] private float durationTitle = 2f;
        [SerializeField] private float delayBeforeResourceGroup = 1f;
        [SerializeField] private float durationItemResourceGroup = 0.3f;

        private Material matSymbol;

        private void ResetPopupUI()
        {
            matSymbol.SetFloat(MatDisolveValue, 1f);
            imgSymbolShiny.gameObject.SetActive(false);
            imgTitle.SetAlpha(0f);
            imgTitleBg.SetAlpha(0f);
            txtDescription.SetAlpha(0f);
            txtTitleResourceCollected.SetAlpha(0f);
            groupResourceCollected.alpha = 0f;
            groupTimePlayed.alpha = 0f;
            rectLine.localScale = new Vector3(0f, 1f, 1f);
            groupBtnBackToTree.alpha = 0f;
            groupBtnReplay.alpha = 0f;
        }
        
        private Tween DoShowUIPopup()
        {
            DOTween.Kill(ui);
            var seq = DOTween.Sequence(ui).SetUpdate(true);

            seq.AppendCallback(ResetPopupUI)
                .Append(DOTween.To(() => 1f, (x) => matSymbol.SetFloat(MatDisolveValue, x), 0f, 1f))
                .AppendCallback((() =>
                {
                    imgSymbolShiny.gameObject.SetActive(true);
                    imgTitleBg.DOFade(1f, 0.3f).SetUpdate(true);
                    imgTitle.DOFade(1f, durationTitle).SetUpdate(true).SetDelay(0.1f);
                    
                    txtDescription.transform.localPosition += new Vector3(0f, 10f, 0f);
                    txtDescription.transform.DOLocalMoveY(-10f, 0.5f).SetUpdate(true).SetDelay(0.5f).SetRelative(true);
                    txtDescription.DOFade(1f, 0.5f).SetUpdate(true).SetDelay(0.5f);
                }))
                .AppendInterval(delayBeforeResourceGroup)
                .AppendCallback(() =>
                {
                    txtTitleResourceCollected.transform.localPosition += new Vector3(0f, 10f, 0f);
                    txtTitleResourceCollected.transform.DOLocalMoveY(-10f, durationItemResourceGroup).SetUpdate(true).SetRelative(true);
                    txtTitleResourceCollected.DOFade(1f, durationItemResourceGroup).SetUpdate(true);
                    
                    groupResourceCollected.transform.localPosition += new Vector3(0f, 10f, 0f);
                    groupResourceCollected.transform.DOLocalMoveY(-10f, durationItemResourceGroup).SetUpdate(true).SetRelative(true).SetDelay(0.2f);
                    groupResourceCollected.DOFade(1f, durationItemResourceGroup).SetUpdate(true).SetDelay(0.2f);
                    
                    groupTimePlayed.transform.localPosition += new Vector3(0f, 10f, 0f);
                    groupTimePlayed.transform.DOLocalMoveY(-10f, durationItemResourceGroup).SetUpdate(true).SetRelative(true).SetDelay(0.4f);
                    groupTimePlayed.DOFade(1f, durationItemResourceGroup).SetUpdate(true).SetDelay(0.4f);
                })
                .AppendInterval(durationItemResourceGroup + 0.4f)
                .Append(rectLine.DOScaleX(1f, 0.2f))
                .AppendCallback(() =>
                {
                    groupBtnBackToTree.transform.localPosition += new Vector3(0f, 10f, 0f);
                    groupBtnBackToTree.transform.DOLocalMoveY(-10f, durationItemResourceGroup).SetUpdate(true).SetRelative(true);
                    groupBtnBackToTree.DOFade(1f, durationItemResourceGroup).SetUpdate(true);
                    
                    groupBtnReplay.transform.localPosition += new Vector3(0f, 10f, 0f);
                    groupBtnReplay.transform.DOLocalMoveY(-10f, durationItemResourceGroup).SetUpdate(true).SetRelative(true);
                    groupBtnReplay.DOFade(1f, durationItemResourceGroup).SetUpdate(true);
                });
            
            return seq;
        }

        [Button]
        private void TestPlay()
        {
            DoShowUIPopup();
        }
    }
}