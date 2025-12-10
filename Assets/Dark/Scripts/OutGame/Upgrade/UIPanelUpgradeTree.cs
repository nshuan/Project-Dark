using System;
using System.Collections;
using Coffee.UIExtensions;
using Dark.Scripts.Audio;
using Dark.Scripts.AudioV2;
using Dark.Scripts.SceneNavigation;
using Dark.Scripts.Utils;
using Data;
using DG.Tweening;
using InGame;
using InGame.CharacterClass;
using UnityEngine;

namespace Dark.Scripts.OutGame.Upgrade
{
    public class UIPanelUpgradeTree : MonoBehaviour
    {
        [SerializeField] private UIUpgradeScrollView scrollView;
        [SerializeField] private Transform treeParent;
        [SerializeField] private UIParticle vfxSpawn;
        [SerializeField] private CanvasGroup groupUIHiddenOnSpawn;
        [SerializeField] private ZoomInOut treeZoom;
        [SerializeField] private float delaShowUI;
        [SerializeField] private string cueSpawnTree;
        [SerializeField] private string cueSpawnNodes; 

        private UIUpgradeTree tree;
        public UIUpgradeTree Tree => tree; 
        
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
            yield return new WaitForSeconds(delaySpawn);
            AudioManagerV2.Instance.PlayInGame(cueSpawnTree);
            AudioManagerV2.Instance.PlayInGame(cueSpawnNodes);
            treeZoom.ZoomTo(0.7f, ((RectTransform)transform).position, 0.5f, 0.3f);
            if (!GameConst.HideLaserWaveOnSpawnTree)
                vfxSpawn.gameObject.SetActive(true);
            yield return new WaitForSeconds(delaShowUI);
            yield return groupUIHiddenOnSpawn.DOFade(1f, 0.3f).SetEase(Ease.OutQuad).WaitForCompletion();
        }

        public void SpawnTree()
        {
            tree = Instantiate(UpgradeTreeManifest.GetTreePrefab((CharacterClass)PlayerDataManager.Instance.Data.characterClass), treeParent);
            // tree.OnNodeUpgraded += (node) => scrollView.FocusTo((RectTransform)node.transform);
        }
    }
}