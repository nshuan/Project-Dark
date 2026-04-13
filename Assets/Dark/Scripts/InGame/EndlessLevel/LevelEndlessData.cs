using Data;

namespace InGame.EndlessLevel
{
    public class LevelEndlessData
    {
        private static string KeyData = "level_endless";

        public static bool IsUnlockedFeature => DataHandler.Exist<bool>(KeyData) && DataHandler.Load<bool>(KeyData);
        
        public static void UnlockEndlessFeature() => DataHandler.Save<bool>(KeyData, true);
    }
}