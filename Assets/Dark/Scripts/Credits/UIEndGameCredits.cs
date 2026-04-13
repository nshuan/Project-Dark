using Dark.Scripts.SceneNavigation;
using Dark.Scripts.Utils;
using Data;
using UnityEngine;

namespace Dark.Scripts.Credits
{
    public class UIEndGameCredits : UIGameCreditsScroll
    {
        protected override void Awake()
        {
            btnClose.onClick.RemoveAllListeners();
            btnClose.onClick.AddListener(() =>
            {
                StopCredits();
                ReturnToUpgrade(0.5f);
            });
        }

        protected override void StartCredits()
        {
            PlayerDataManager.Instance.SetFlagCompletedAllLevel();
            base.StartCredits();
        }

        protected override void Update()
        {
            if (!started) return;
            content.localPosition += new Vector3(0f, movePointPerUpdate, 0f);
            if (content.position.y > anchorEnd.position.y)
            {
                StopCredits();
                ReturnToUpgrade(3f);
            }
        }

        private void ReturnToHome(float delay)
        {
            this.DelayCall(delay, () =>
            {
                Loading.Instance.LoadScene(SceneConstants.SceneMenu);
            });
        }

        private void ReturnToUpgrade(float delay)
        {
            this.DelayCall(delay, () =>
            {
                Loading.Instance.QuickLoadScene(SceneConstants.SceneUpgrade);
            });
        }
    }
}