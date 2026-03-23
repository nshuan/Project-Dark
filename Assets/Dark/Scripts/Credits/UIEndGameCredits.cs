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
                ReturnToHome();
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
                ReturnToHome();
            }
        }

        private void ReturnToHome()
        {
            this.DelayCall(3f, () =>
            {
                Loading.Instance.LoadScene(SceneConstants.SceneMenu);
            });
        }
    }
}