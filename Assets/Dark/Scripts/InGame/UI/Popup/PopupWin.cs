using System;
using Dark.Scripts.CoreUI;
using Dark.Scripts.SceneNavigation;
using Data;
using DG.Tweening;
using Economic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InGame.UI
{
    public class PopupWin : MonoBehaviour
    {
        [SerializeField] private UIPopup ui;
        
        [Space]
        [SerializeField] private Button btnBackToTree;
        [SerializeField] private Button btnNextLevel;

        public static event Action onShowPopup;
        
        private void Start()
        {
            LevelManager.Instance.OnWin += OnWin;
        }

        private void OnDestroy()
        {
            onShowPopup = null;
        }

        [Button]
        private void OnWin()
        {
            UpdateUI();
            ui.DoOpenFadeIn(0f).OnComplete(() =>
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
                Loading.Instance.QuickLoadScene(SceneConstants.SceneUpgrade);
            });
            
            // Todo load next level
            btnNextLevel.onClick.RemoveAllListeners();
            btnNextLevel.onClick.AddListener(() =>
            {
                ui.gameObject.SetActive(false);
                Loading.Instance.QuickLoadScene(SceneConstants.SceneInGame, () =>
                {
                    LevelManager.Instance.LoadLevel(PlayerDataManager.Instance.Data.level + 1);
                });
            });
        }
        
        [Space] [Header("UI Tween")] 
        
        [SerializeField] private Image imgTitle;
        [SerializeField] private Image imgTitleBg;
        [SerializeField] private TextMeshProUGUI txtDescription;
        [SerializeField] private TextMeshProUGUI txtTitleResourceCollected;
        [SerializeField] private CanvasGroup groupResourceCollected;
        [SerializeField] private CanvasGroup groupTimePlayed;
        [SerializeField] private Transform rectLine;
        [SerializeField] private CanvasGroup groupBtnBackToTree;
        [SerializeField] private CanvasGroup groupBtnReplay;

        private void ResetPopupUI()
        {
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
                .AppendCallback((() =>
                {
                    imgTitleBg.DOFade(1f, 0.3f).SetUpdate(true);
                    imgTitle.DOFade(1f, 0.7f).SetUpdate(true).SetEase(Ease.OutQuad).SetDelay(0.1f);
                }))
                .AppendInterval(0.3f)
                .AppendCallback(() =>
                {
                    txtDescription.transform.localPosition += new Vector3(0f, 10f, 0f);
                    txtDescription.transform.DOLocalMoveY(-10f, 0.5f).SetUpdate(true).SetRelative(true);
                    txtDescription.DOFade(1f, 0.5f).SetUpdate(true);
                })
                .AppendInterval(0.8f)
                .AppendCallback(() =>
                {
                    txtTitleResourceCollected.transform.localPosition += new Vector3(0f, 10f, 0f);
                    txtTitleResourceCollected.transform.DOLocalMoveY(-10f, 0.5f).SetUpdate(true).SetRelative(true);
                    txtTitleResourceCollected.DOFade(1f, 0.5f).SetUpdate(true);
                    
                    groupResourceCollected.transform.localPosition += new Vector3(0f, 10f, 0f);
                    groupResourceCollected.transform.DOLocalMoveY(-10f, 0.5f).SetUpdate(true).SetRelative(true).SetDelay(0.2f);
                    groupResourceCollected.DOFade(1f, 0.5f).SetUpdate(true).SetDelay(0.2f);
                    
                    groupTimePlayed.transform.localPosition += new Vector3(0f, 10f, 0f);
                    groupTimePlayed.transform.DOLocalMoveY(-10f, 0.5f).SetUpdate(true).SetRelative(true).SetDelay(0.4f);
                    groupTimePlayed.DOFade(1f, 0.5f).SetUpdate(true).SetDelay(0.4f);
                })
                .AppendInterval(1f)
                .Append(rectLine.DOScaleX(1f, 0.3f))
                .AppendCallback(() =>
                {
                    groupBtnBackToTree.transform.localPosition += new Vector3(0f, 10f, 0f);
                    groupBtnBackToTree.transform.DOLocalMoveY(-10f, 0.5f).SetUpdate(true).SetRelative(true);
                    groupBtnBackToTree.DOFade(1f, 0.5f).SetUpdate(true);
                    
                    groupBtnReplay.transform.localPosition += new Vector3(0f, 10f, 0f);
                    groupBtnReplay.transform.DOLocalMoveY(-10f, 0.5f).SetUpdate(true).SetRelative(true);
                    groupBtnReplay.DOFade(1f, 0.5f).SetUpdate(true);
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