using System;
using UnityEngine;

namespace Dark.Scripts.Tutorial
{
    public abstract class UIAbstractTutorialStep : MonoBehaviour
    {
        public Action OnComplete { get; set; }

        public abstract bool IsValid();
        public abstract void Setup();

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }
    }
    
    public abstract class UIAbstractTutorialStepInGame : UIAbstractTutorialStep
    {

    }
}