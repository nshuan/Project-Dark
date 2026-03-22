using InGame.CharacterClass;
using Steamworks;

namespace Dark.Scripts.Leaderboard
{
    public class LeaderboardEntryData
    {
        public CSteamID steamID;
        public int rank;
        public int score;
        public string playerName;
        public CharacterClass classType;
    }
}