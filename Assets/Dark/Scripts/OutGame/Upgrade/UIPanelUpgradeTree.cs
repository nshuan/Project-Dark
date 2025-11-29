using System;
using System.Collections;
using Coffee.UIExtensions;
using Dark.Scripts.SceneNavigation;
using Dark.Scripts.Utils;
using DG.Tweening;
using InGame;
using UnityEngine;

namespace Dark.Scripts.OutGame.Upgrade
{
    public class UIPanelUpgradeTree : MonoBehaviour
    {
        [SerializeField] private UIParticle vfxSpawn;
        [SerializeField] private CanvasGroup groupUIHiddenOnSpawn;
        [SerializeField] private ZoomInOut treeZoom;
        [SerializeField] private float delaShowUI;

        private void OnEnable()
        {
            StopAllCoroutines();
            StartCoroutine(IESpawn());
        }

        private IEnumerator IESpawn()
        {
            var delaySpawn = Loading.Instance.CurrentTotalDurationAfterSceneLoaded;
            groupUIHiddenOnSpawn.alpha = 0f;
            vfxSpawn.gameObject.SetActive(false);
            treeZoom.SetZoom(1f);
            treeZoom.DoZoomTo(0.7f, Vector2.zero, 0.5f).SetDelay(delaySpawn + 0.3f);
            yield return new WaitForSeconds(delaySpawn);
            if (!GameConst.HideLaserWaveOnSpawnTree)
                vfxSpawn.gameObject.SetActive(true);
            yield return new WaitForSeconds(delaShowUI);
            yield return groupUIHiddenOnSpawn.DOFade(1f, 0.3f).SetEase(Ease.OutQuad).WaitForCompletion();
        }
    }
}