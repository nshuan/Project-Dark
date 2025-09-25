using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace Economic.UI
{
    public class UIEconomic : MonoBehaviour
    {
        private Coroutine coroutineAnimatedUpdating;
        protected int current;
        protected int target;
        protected float updateInterval = 0.05f;

        public virtual void UpdateUI()
        {
            
        }
        
        public void AnimateUpdating(int target)
        {
            current = target;
            UpdateUI();
            return;
            if (target == this.target) return;
            this.target = target;
            
            if (coroutineAnimatedUpdating == null)
            {
                coroutineAnimatedUpdating = StartCoroutine(IEAnimatedUpdating());
            }
        }

        private IEnumerator IEAnimatedUpdating()
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

            coroutineAnimatedUpdating = null;
        }
    }
}