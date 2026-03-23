using System;
using Dark.Scripts.Analytics;
using Dark.Scripts.AudioV2;
using Dark.Scripts.CoreUI;
using Dark.Scripts.SceneNavigation;
using Dark.Tools.Language.Runtime;
using Data;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InGame.UI
{
    public class PopupWin : MonoBehaviour
    {
        [SerializeField] private UIPopup ui;
        [SerializeField] private float delayShowPopup = 2f;
        [SerializeField] private AudioPlayComponentV2 sfxWin;
        
        [Space]
        [SerializeField] private Button btnBackToTree;
        [SerializeField] private Button btnNextLevel;
        [SerializeField] private Button btnCredit;
        [SerializeField] private Button btnBackToTree1;
        [SerializeField] private GameObject groupBtnWin;
        [SerializeField] private GameObject groupEndLevel;

        [Space] 
        [SerializeField] private GameObject popupCredit;

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
            AudioManagerV2.Instance.FadeVolumeMusic(0.08f, 2f);
            ui.DoOpenFadeIn(delayShowPopup).OnComplete(() =>
            {
                onShowPopup?.Invoke();
                DoShowUIPopup();
            });
        }

        private void UpdateUI()
        {
            ResetPopupUI();

            var completed = PlayerDataManager.Instance.Data.completed;
            var winLevel = LevelManager.Instance.Level.level;
            if (winLevel + 1 <= LevelManifest.Instance.GetMaxLevel(PlayerDataManager.Instance.Data.Class))
            {
                groupBtnWin.SetActive(true);
                groupEndLevel.SetActive(false);
                
                btnBackToTree.onClick.RemoveAllListeners();
                btnBackToTree.onClick.AddListener(() =>
                {
                    Loading.Instance.QuickLoadScene(SceneConstants.SceneUpgrade);
                });
            
                // Todo load next level
                btnNextLevel.onClick.RemoveAllListeners();
                btnNextLevel.onClick.AddListener(() =>
                {
                    if (winLevel + 1 > LevelManifest.Instance.GetMaxLevel(PlayerDataManager.Instance.Data.Class))
                        return;
                
                    ui.gameObject.SetActive(false);
                    Loading.Instance.QuickLoadScene(SceneConstants.SceneInGame);
                    Loading.Instance.onSceneLoaded += () =>
                    {
                        LevelManager.Instance.LoadLevel(winLevel + 1);
                    };
                
                    LogManager.Log(LogConst.EventLogStartLevel, $"level_{winLevel + 1}", "from popup win");
                });    
            }
            else
            {
                groupBtnWin.SetActive(false);
                groupEndLevel.SetActive(true);

                if (completed)
                {
                    btnCredit.gameObject.SetActive(false);
                    btnBackToTree1.gameObject.SetActive(true);
                    
                    btnBackToTree1.onClick.RemoveAllListeners();
                    btnBackToTree1.onClick.AddListener(() =>
                    {
                        Loading.Instance.QuickLoadScene(SceneConstants.SceneUpgrade);
                    });
                }
                else
                {
                    btnCredit.gameObject.SetActive(true);
                    btnBackToTree1.gameObject.SetActive(false);
                    
                    btnCredit.onClick.RemoveAllListeners();
                    btnCredit.onClick.AddListener(() =>
                    {
                        popupCredit.SetActive(true);
                    });
                }
                
            }
        }

        [Space] [Header("UI Tween")] 
        
        private string keyTextBossDown = "key_win_boss_was_down";
        
        [SerializeField] private Image imgTitle;
        [SerializeField] private Image imgTitleBg;
        [SerializeField] private TextMeshProUGUI txtDescription;
        [SerializeField] private TextMeshProUGUI txtTitleResourceCollected;
        [SerializeField] private CanvasGroup groupResourceCollected;
        [SerializeField] private CanvasGroup groupTimePlayed;
        [SerializeField] private Transform rectLine;
        [SerializeField] private CanvasGroup groupBtnBackToTree;
        [SerializeField] private CanvasGroup groupBtnReplay;
        [SerializeField] private CanvasGroup groupBtnCredits;
        [SerializeField] private CanvasGroup groupBtnBackToTree1;

        [Header("UI Tween Config")] 
        [SerializeField] private float durationTitle = 2f;
        [SerializeField] private float delayBeforeResourceGroup = 1f;
        [SerializeField] private float durationItemResourceGroup = 0.3f;
        
        private void ResetPopupUI()
        {
            imgTitle.SetAlpha(0f);
            imgTitleBg.SetAlpha(0f);
            txtDescription.SetTextLanguage(keyTextBossDown, ("%{value}", LevelManager.Instance.LevelBossName));
            txtDescription.SetAlpha(0f);
            txtTitleResourceCollected.SetAlpha(0f);
            groupResourceCollected.alpha = 0f;
            groupTimePlayed.alpha = 0f;
            rectLine.localScale = new Vector3(0f, 1f, 1f);
            groupBtnBackToTree.alpha = 0f;
            groupBtnReplay.alpha = 0f;
            groupBtnCredits.alpha = 0f;
            groupBtnBackToTree1.alpha = 0f;
        }
        
        private Tween DoShowUIPopup()
        {
            DOTween.Kill(ui);
            var seq = DOTween.Sequence(ui).SetUpdate(true);

            seq.AppendCallback(ResetPopupUI)
                .AppendCallback((() =>
                {
                    sfxWin.Play();
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
                    if (groupBtnBackToTree.gameObject.activeInHierarchy)
                    {
                        groupBtnBackToTree.transform.localPosition += new Vector3(0f, 10f, 0f);
                        groupBtnBackToTree.transform.DOLocalMoveY(-10f, durationItemResourceGroup).SetUpdate(true).SetRelative(true);
                        groupBtnBackToTree.DOFade(1f, durationItemResourceGroup).SetUpdate(true);
                    }

                    if (groupBtnReplay.gameObject.activeInHierarchy)
                    {
                        groupBtnReplay.transform.localPosition += new Vector3(0f, 10f, 0f);
                        groupBtnReplay.transform.DOLocalMoveY(-10f, durationItemResourceGroup).SetUpdate(true).SetRelative(true);
                        groupBtnReplay.DOFade(1f, durationItemResourceGroup).SetUpdate(true);
                    }

                    if (groupBtnCredits.gameObject.activeInHierarchy)
                    {
                        groupBtnCredits.transform.localPosition += new Vector3(0f, 10f, 0f);
                        groupBtnCredits.transform.DOLocalMoveY(-10f, durationItemResourceGroup).SetUpdate(true).SetRelative(true);
                        groupBtnCredits.DOFade(1f, durationItemResourceGroup).SetUpdate(true);
                    }
                    
                    if (groupBtnBackToTree1.gameObject.activeInHierarchy)
                    {
                        groupBtnBackToTree1.transform.localPosition += new Vector3(0f, 10f, 0f);
                        groupBtnBackToTree1.transform.DOLocalMoveY(-10f, durationItemResourceGroup).SetUpdate(true).SetRelative(true);
                        groupBtnBackToTree1.DOFade(1f, durationItemResourceGroup).SetUpdate(true);
                    }
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