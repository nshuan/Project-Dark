using System.Collections;
using UnityEngine;

namespace InGame.UI.Economic
{
    public class UIInGameEconomic : MonoBehaviour
    {
        private Coroutine coroutineAnimatedIncreasing;
        protected int current;
        protected int target;
        protected float updateInterval = 0.05f;

        public virtual void UpdateUI()
        {
            
        }
        
        public void AnimateIncreasing(int target)
        {
            if (target == this.target) return;
            this.target = target;
            
            if (coroutineAnimatedIncreasing == null)
            {
                coroutineAnimatedIncreasing = StartCoroutine(IEAnimatedIncreasing());
            }
        }

        private IEnumerator IEAnimatedIncreasing()
        {
            if (current < target)
            {
                while (current < target)
                {
                    current += 1;
                    UpdateUI();
                    yield return new WaitForSeconds(updateInterval);
                }
            }
            else if (current > target)
            {
                while (current > target)
                {
                    current -= 1;
                    UpdateUI();
                    yield return new WaitForSeconds(updateInterval);
                }
            }

            coroutineAnimatedIncreasing = null;
        }
    }
}