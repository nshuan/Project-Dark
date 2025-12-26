using System;
using UnityEngine;

namespace Dark.Scripts.Tutorial
{
    public abstract class UIAbstractTutorialStep : MonoBehaviour
    {
        public Action OnComplete { get; set; }

        public abstract bool IsValid();
        public abstract void Setup();
        public abstract void Setup(Action<Vector2, Vector2, float, bool, bool> actionUpdateFocus); // <Position, Size, roundness, enableRaycast, enableTapToHideCover>

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }
    }
    
    public abstract class UIAbstractTutorialStepInGame : UIAbstractTutorialStep
    {
        protected Action<Vector2, Vector2, float,  bool, bool> actionUpdateFocus;

        public override void Setup(Action<Vector2, Vector2, float, bool, bool> actionUpdateFocus)
        {
            this.actionUpdateFocus = actionUpdateFocus;  
            Setup();
        }
    }
}