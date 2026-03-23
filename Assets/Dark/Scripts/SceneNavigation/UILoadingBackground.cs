using Dark.Scripts.Common.Lore;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Dark.Scripts.SceneNavigation
{
    public class UILoadingBackground : MonoBehaviour
    {
        [SerializeField] private Loading loading;
        [SerializeField] private GameObject[] allBgs;
        [SerializeField] private bool guaranteeFirstBackground;

        [SerializeField, ShowIf("guaranteeFirstBackground")]
        private GameObject firstBg;

        private int[] randomIndexes;
        private int currentRandomIndexIndex;
        private bool shownFirstBackground;
        
        private void Awake()
        {
            loading.onStartLoading += OnStartLoading;
        }

        private void OnStartLoading()
        {
            if (!shownFirstBackground && guaranteeFirstBackground && firstBg)
            {
                if (allBgs != null)
                {
                    foreach (var bg in allBgs)
                    {
                        bg.SetActive(false);
                    }
                }

                firstBg.SetActive(true);
                shownFirstBackground = true;
                
                return;
            }
            
            if (allBgs == null || allBgs.Length == 0) return;

            if (randomIndexes == null || currentRandomIndexIndex >= randomIndexes.Length)
            {
                randomIndexes = RandomUtil.ShuffleIndex(0, allBgs.Length - 1);
                currentRandomIndexIndex = 0;
            }
            
            for (var i = 0; i < allBgs.Length; i++)
            {
                allBgs[i].SetActive(i == randomIndexes[currentRandomIndexIndex]);
            }

            currentRandomIndexIndex += 1;
        }
    }
}