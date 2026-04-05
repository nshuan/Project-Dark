using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace InGame
{
    public class BackgroundSpawner : MonoBehaviour
    {
        [SerializeField] private List<BackgroundInfo> listBackground;
        
        private BackgroundInfo currentBackground;

        [Button]
        private void GetAllSprites()
        {
            listBackground ??= new List<BackgroundInfo>();
            foreach (var bgInfo in listBackground)
            {
                bgInfo.sprites = new List<SpriteRenderer>();
                bgInfo.spriteDefaultAlpha = new List<float>();
                if (bgInfo.bg == null) continue;
                
                var all = bgInfo.bg.GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
                foreach (var sr in all)
                {
                    bgInfo.sprites.Add(sr);
                    bgInfo.spriteDefaultAlpha.Add(sr.color.a);
                }
            }
        }

        public void Spawn(int index)
        {
            if (listBackground == null || listBackground.Count == 0) return;
            index = Mathf.Clamp(index, 0, listBackground.Count - 1);
            for (var i = 0; i < listBackground.Count; i++)
            {
                listBackground[i].bg.SetActive(i == index);
                if (i == index)
                {
                    currentBackground = listBackground[i];
                    AllBackgroundInGame.Instance.SetCurrentBackground(i);
                }
            }
        }

        private Coroutine coroutineTransition;
        [Button]
        private void TestTransition(int index)
        {
            if (index < 0) return;
            if (index > 2) return;
            
            if (coroutineTransition != null) StopCoroutine(coroutineTransition);
            coroutineTransition = StartCoroutine(IETransition(index));
        }
        
        public IEnumerator IETransition(int index)
        {
            var lastBackground = currentBackground;
            for (var i = 0; i < listBackground.Count; i++)
            {
                if (i == index)
                {
                    currentBackground = listBackground[i];
                    AllBackgroundInGame.Instance.SetCurrentBackground(i);
                    break;
                }
            }

            if (currentBackground == null || currentBackground == lastBackground) yield break;

            DOTween.Kill(this, true);
            var seq = DOTween.Sequence(this);
            
            if (lastBackground != null)
            {
                foreach (var s in lastBackground.sprites)
                {
                    seq.Join(s.DOFade(0f, 0.25f));
                }
            }

            // Chắc chắn newBackground đã khác null rồi
            {
                var cacheColor = new Color(0f, 0f, 0f, 0f);
                for (var i = 0; i < currentBackground.sprites.Count; i++)
                {
                    var s = currentBackground.sprites[i];
                    var sAlpha = currentBackground.spriteDefaultAlpha[i];
                    cacheColor.r = s.color.r;
                    cacheColor.g = s.color.g;
                    cacheColor.b = s.color.b;
                    cacheColor.a = 0;
                    s.color = cacheColor;
                    seq.Join(s.DOFade(sAlpha, 0.25f));
                }
                
                currentBackground.bg.SetActive(true);
            }
            
            seq.AppendCallback(() => lastBackground?.bg.SetActive(false));
            
            yield return seq.WaitForCompletion();
        }
        
        [Serializable]
        public class BackgroundInfo
        {
            public GameObject bg;
            public List<SpriteRenderer> sprites;
            public List<float> spriteDefaultAlpha;
        }
    }
}