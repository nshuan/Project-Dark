using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InGame.UI.Waves
{
    public class UIWaveProcessV2 : MonoBehaviour
    {
        [SerializeField] private List<UIWaveProcessItemV2> waveItems;
        [SerializeField] private Image waveLineActive;
        [SerializeField] private Image waveLineInactive;
        [SerializeField] private Image currentWave;
        [SerializeField] private TextMeshProUGUI txtWave;

        [Space] [Header("Config")] 
        public float gapDuration = 0.2f;
        public float headItemDuration = 0.1f;

        private int totalWave = 10;
        private float fillAmountPerWave;
        private Vector3 wavesGapLength = new Vector3(0, 0, 0);
        private float txtWaveOffsetFromCurrentWave;
        private int currentWaveIndex;
        private float waveTotalDuration;
        private float waveCurrentDuration;
        private bool isLevelEnded = false;
        private bool isLevelStarted = false;
        
        private void Awake()
        {
            // Hide all wave items and wave line on initialize
            foreach (var item in waveItems)
            {
                item.gameObject.SetActive(false);
            }
            waveLineActive.gameObject.SetActive(false);
            waveLineInactive.gameObject.SetActive(false);
            currentWave.gameObject.SetActive(false);
            txtWave.gameObject.SetActive(false);
            
            LevelManager.Instance.OnLevelLoaded += OnLevelLoaded;
            LevelManager.Instance.OnWaveStart += OnWaveStart;
            LevelManager.Instance.OnWin += () => isLevelEnded = true;
            LevelManager.Instance.OnLose += () => isLevelEnded = true;
        }
        
        private void OnLevelLoaded(LevelConfig level)
        {
            totalWave = level.waveInfo.Length;
            currentWave.transform.position = waveItems[0].transform.position;
            txtWaveOffsetFromCurrentWave = txtWave.transform.position.y - currentWave.transform.position.y; 
            txtWave.transform.position = currentWave.transform.position + new Vector3(0f, txtWaveOffsetFromCurrentWave, 0f);
            
            // Cook an animation to show all wave nodes
            DoShowAllNodes().Play();
        }

        private void OnWaveStart(int waveIndex, float waveDuration)
        {
            currentWaveIndex = waveIndex;
            waveTotalDuration = waveDuration;
            waveCurrentDuration = 0f;
            
            wavesGapLength = new Vector3(
                waveIndex >= totalWave - 1 ? 0 : waveItems[waveIndex + 1].transform.position.x - waveItems[waveIndex].transform.position.x,
                0f, 0f);
            
            waveItems[waveIndex].DoPassed();
            if (waveIndex > 0) currentWave.transform.DOPunchScale(0.2f * Vector3.one, 0.2f).Play();
            if (!isLevelStarted) isLevelStarted = true;
            
            txtWave.SetText($"wave {waveIndex + 1}");
            if (waveIndex > 0) txtWave.transform.DOPunchScale(0.2f * Vector3.one, 0.2f).SetDelay(0.1f).Play();
        }

        private Tween DoShowAllNodes()
        {
            DOTween.Kill(this);
            
            // Check if there is less spawn nodes than needed, instantiate more
            if (totalWave > waveItems.Count)
            {
                for (var i = waveItems.Count; i <= totalWave; i++)
                {
                    var newItem = Instantiate(waveItems[0], waveItems[0].transform.parent);
                    waveItems.Add(newItem);
                }
            }
            
            // Hide all wave items and wave line
            foreach (var item in waveItems)
            {
                item.transform.localScale = Vector3.zero;
                item.gameObject.SetActive(true);
            }
            waveLineActive.fillAmount = 0f;
            waveLineInactive.fillAmount = 0f;
            waveLineActive.gameObject.SetActive(true);
            waveLineInactive.gameObject.SetActive(true);
            currentWave.transform.localScale = Vector3.zero;
            currentWave.transform.position = waveItems[0].transform.position;
            currentWave.gameObject.SetActive(true);
            txtWave.transform.localScale = Vector3.zero;
            txtWave.gameObject.SetActive(true);
            
            // Continuously show node from 0 to end
            var seq = DOTween.Sequence(this);
            fillAmountPerWave = 1f / 9;

            seq.Append(waveItems[0].transform.DOScale(1f, headItemDuration).SetEase(Ease.OutBack));
            for (var i = 1; i < totalWave; i++)
            {
                var index = i;
                seq.Append(waveLineActive.DOFillAmount(fillAmountPerWave * index, gapDuration).SetEase(Ease.Unset))
                    .AppendCallback(() => waveItems[index].transform.DOScale(1f, headItemDuration).SetEase(Ease.OutBack));
            }

            seq.AppendCallback(() =>
            {
                currentWave.transform.DOScale(1f, 0.2f).SetEase(Ease.OutBack);
                txtWave.transform.DOScale(1f, 0.2f).SetDelay(0.1f).SetEase(Ease.OutBack);
            });
            
            return seq;
        }
        
        private void Update()
        {
            if (!isLevelStarted) return;
            if (isLevelEnded) return;
            if (currentWaveIndex >= totalWave - 1) return;

            var ratio = waveCurrentDuration / waveTotalDuration;
            waveLineInactive.fillAmount = fillAmountPerWave * (currentWaveIndex + ratio);
            currentWave.transform.position = waveItems[currentWaveIndex].transform.position + wavesGapLength * ratio;
            txtWave.transform.position = currentWave.transform.position + new Vector3(0f, txtWaveOffsetFromCurrentWave, 0f);
            waveCurrentDuration += Time.deltaTime;
        }
    }
}