using System;
using System.Collections;
using System.Collections.Generic;
using Dark.Scripts.SceneNavigation;
using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.UI;

namespace Dark.Scripts.Common.Init
{
    public class InitAndLoadHome : SerializedMonoBehaviour
    {
        [OdinSerialize, NonSerialized] private List<IEarlyInitialize> initializers;
        [SerializeField] private Image imgColorSplashScene;
        [SerializeField] private bool autoLoadHome = true;

        private IEnumerator Start()
        {
            foreach (var initializer in initializers)
            {
                initializer.Initialize();
                yield return new WaitForEndOfFrame();
            }

            if (autoLoadHome)
            {
                Action actionLoadHome = () => Loading.Instance.LoadScene(SceneConstants.SceneMenu);
                if (imgColorSplashScene)
                {
                    imgColorSplashScene.DOFade(0f, 0.5f)
                        .OnComplete(() => actionLoadHome?.Invoke());
                }
                else
                {
                    actionLoadHome?.Invoke();
                }
            }
            else
            {
                imgColorSplashScene?.gameObject.SetActive(false);
            }
        }
    }
}