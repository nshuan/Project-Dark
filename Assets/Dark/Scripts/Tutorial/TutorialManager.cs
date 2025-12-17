using System;
using Core;
using Data;

namespace Dark.Scripts.Tutorial
{
    public class TutorialManager : Singleton<TutorialManager>
    {
        private const string KeyTutorial = "tutorial";
        public static string KeyTutorialSlot = KeyTutorial + "_" + PlayerDataManager.DataKey;
        
        public int GetCurrentTutorialStep()
        {
            if (DataHandler.Exist<TutorialData>(KeyTutorialSlot)) return DataHandler.Load<TutorialData>(KeyTutorialSlot).step;
            return 0;
        }

        public void CompleteTutorialStep(int step)
        {
            var currentStep = DataHandler.Exist<TutorialData>(KeyTutorialSlot)
                ? DataHandler.Load<TutorialData>(KeyTutorialSlot)
                : new TutorialData();
            
            if (step < currentStep.step) return;
            currentStep.step = step + 1;
            DataHandler.Save(KeyTutorialSlot, currentStep);
        }

        public int GetCurrentTutorialStepTree()
        {
            if (DataHandler.Exist<TutorialData>(KeyTutorialSlot))
                return DataHandler.Load<TutorialData>(KeyTutorialSlot).stepTree;
            return 0;
        } 
        
        public void CompleteTutorialStepTree(int stepTree)
        {
            var currentStep = DataHandler.Exist<TutorialData>(KeyTutorialSlot)
                ? DataHandler.Load<TutorialData>(KeyTutorialSlot)
                : new TutorialData();
            
            if (stepTree < currentStep.stepTree) return;
            currentStep.stepTree = stepTree + 1;
            DataHandler.Save(KeyTutorialSlot, currentStep);
        }

        public void ClearData(string dataKey)
        {
            var trueKey = KeyTutorial + "_" + dataKey;
            if (DataHandler.Exist<TutorialData>(trueKey))
                DataHandler.Clear(trueKey);
        }
    }

    [Serializable]
    public class TutorialData
    {
        public int step;
        public int stepTree;
    }
}