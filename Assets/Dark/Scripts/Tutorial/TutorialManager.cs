using System;
using Core;
using Data;

namespace Dark.Scripts.Tutorial
{
    public class TutorialManager : Singleton<TutorialManager>
    {
        public int GetCurrentTutorialStep(string tutorialKey)
        {
            if (DataHandler.Exist<TutorialData>(tutorialKey)) return DataHandler.Load<TutorialData>(tutorialKey).step;
            return 0;
        }

        public void CompleteTutorialStep(string tutorialKey, int step)
        {
            var currentStep = DataHandler.Exist<TutorialData>(tutorialKey)
                ? DataHandler.Load<TutorialData>(tutorialKey)
                : new TutorialData();
            
            if (step < currentStep.step) return;
            currentStep.step = step + 1;
            DataHandler.Save(tutorialKey, currentStep);
        }
    }

    [Serializable]
    public class TutorialData
    {
        public int step;
    }
}