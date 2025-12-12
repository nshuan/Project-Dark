public class GameConst
{
    public const string SteamWishlistURL = "https://store.steampowered.com/app/3913310/Ash_Warden/";
    public const string FeedbackURL = "https://forms.gle/3f5XtYzSyrJen8yW8";
    public const int MaxDemoLevel = 3; // Tester can only play 3 levels in demo version
    public const bool IsDemo = true;

    #region Log

    public const bool EnableLogManagerDebugLog = false; // Enable debug log for LogManager
    public const bool EnableLog = true; // Enable event log in LogManager

    #endregion
    
    public const float IsoRatio = 0.5837f; // max height / max width 
    public const float EnemyEliteScale = 1.5f; // scale normal enemy to be elite    
    public const bool DefaultAutoAttack = true; // Default enable auto-attack
    
    public static bool HideLockedNode = true; // Hide locked nodes in skill tree
    public static bool HideLaserWaveOnSpawnTree = true; // Hide laser wave on tree spawn animation

    public const string FloatFormat = "0.##";
}