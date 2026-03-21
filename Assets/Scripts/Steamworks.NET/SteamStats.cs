using System.Collections;
using Core;
using UnityEngine;

namespace Steamworks.NET
{
    public class SteamStats : MonoSingleton<SteamStats>
    {
        private int totalKills;
        private int totalRuns;
        private int highestDayCompleted;
        
#if STEAMWORKS_NET
        Callback<UserStatsReceived_t> callbackStatsReceived;
#endif
        
        public bool StatReceived { get; private set; }
        private bool initializeStarted;
        
        public void Initialize()
        {
#if STEAMWORKS_NET
            if (initializeStarted) return;
            StatReceived = false;
            initializeStarted = true;
            StartCoroutine(IEInitialize());
#endif
        }

        private IEnumerator IEInitialize()
        {
            yield return new WaitUntil(() => SteamManager.Initialized);
            callbackStatsReceived = Callback<UserStatsReceived_t>.Create(OnStatsReceived);
            SteamUserStats.RequestUserStats(SteamUser.GetSteamID());
        }

#if STEAMWORKS_NET
        private void OnStatsReceived(UserStatsReceived_t callback)
        {
            if (!SteamManager.Initialized) return;
            if (callback.m_eResult != EResult.k_EResultOK)
                return;

            StatReceived = true;
            
            SteamUserStats.GetStat(SteamStatsAPIName.TOTAL_KILL, out totalKills);
            SteamUserStats.GetStat(SteamStatsAPIName.TOTAL_RUNS, out totalRuns);
            SteamUserStats.GetStat(SteamStatsAPIName.HIGHEST_DAY_COMPLETED, out highestDayCompleted);
        }
#endif

        #region Stats

        public void AddKills(int addedValue, bool saveImmediately = false)
        {
#if STEAMWORKS_NET
            if (!StatReceived) return;
            if (!SteamManager.Initialized) return;
            totalKills += addedValue;
            if (saveImmediately)
            { 
                SteamUserStats.SetStat(SteamStatsAPIName.TOTAL_KILL, totalKills); 
                SteamUserStats.StoreStats();
            }
#endif
        }

        public void AddRuns(int addedValue, bool saveImmediately = false)
        {
#if STEAMWORKS_NET
            if (!StatReceived) return;
            if (!SteamManager.Initialized) return;
            totalRuns += addedValue;
            if (saveImmediately)
            { 
                SteamUserStats.SetStat(SteamStatsAPIName.TOTAL_RUNS, totalRuns);
                SteamUserStats.StoreStats();
            }
#endif
        }

        public void CompleteDay(int day, bool saveImmediately = false)
        {
#if STEAMWORKS_NET
            if (!StatReceived) return;
            if (!SteamManager.Initialized) return;
            if (day <= highestDayCompleted) return; 
            highestDayCompleted = day;
            if (saveImmediately)
            {
                SteamUserStats.SetStat(SteamStatsAPIName.HIGHEST_DAY_COMPLETED, highestDayCompleted);
                SteamUserStats.StoreStats();
            }
#endif
        }

        public void SaveStats()
        {
            if (!SteamManager.Initialized) return;
            SteamUserStats.SetStat(SteamStatsAPIName.TOTAL_KILL, totalKills);
            SteamUserStats.SetStat(SteamStatsAPIName.TOTAL_RUNS, totalRuns);
            SteamUserStats.SetStat(SteamStatsAPIName.HIGHEST_DAY_COMPLETED, highestDayCompleted);
            SteamUserStats.StoreStats();
        }
        
        #endregion

        #region Achievements

        public void TryClaimAchievement(string achievementName)
        {
#if STEAMWORKS_NET
            if (!StatReceived) return;
            if (!SteamManager.Initialized) return;
            if (!SteamUserStats.GetAchievement(achievementName, out var achieved)) return;
            if (achieved) return;
            SteamUserStats.SetAchievement(achievementName);
            SteamUserStats.StoreStats();
#endif
        }

        #endregion
    }
}