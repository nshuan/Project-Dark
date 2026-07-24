using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;
using static Stove.PCSDK.GameSupport;

namespace Dark.Scripts.STOVE
{
    public class GameSupportStats : MonoSingleton<GameSupportStats>
    {
        private int totalKills;
        private int totalRuns;
        private int highestDayCompleted;
        private int pendingStatQueries;
        private bool initializeStarted;
        private bool startupStatsUnavailable;
        private int pendingKillsToAdd;
        private int pendingRunsToAdd;
        private int pendingHighestDayCompleted = -1;
        private bool pendingSaveStats;
        private bool startupStatsFinalized;
        private readonly List<string> pendingAchievements = new List<string>();
        private readonly HashSet<string> pendingStartupStatIds = new HashSet<string>();
        private readonly Queue<StartupStatQuery> startupStatQueries = new Queue<StartupStatQuery>();
        [SerializeField] private bool enableDebugLogs = true;
        [SerializeField] private float startupStatQueryTimeoutSeconds = 10f;

        public bool StatReceived { get; private set; }

        protected override void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            base.Awake();
            gameObject.name = nameof(GameSupportStats);

            if (transform.parent != null)
                transform.SetParent(null);

            DontDestroyOnLoad(gameObject);
        }

        public void Initialize()
        {
            if (initializeStarted) return;

            StatReceived = false;
            initializeStarted = true;
            Log("Initialize requested.");
            StartCoroutine(IEInitialize());
        }

        private IEnumerator IEInitialize()
        {
            var stoveManager = STOVEPCSDK3Manager.Instance;

            while (stoveManager.IsInitializing)
                yield return null;

            if (!stoveManager.IsGameSupportInitialized)
            {
                startupStatsUnavailable = true;
                Debug.LogWarning("STOVE GameSupport stats skipped because GameSupport SDK is not initialized.");
                ClearPendingCalls();
                yield break;
            }

            Log("Querying startup stats.");
            startupStatsFinalized = false;
            pendingStartupStatIds.Clear();
            startupStatQueries.Clear();
            AddStartupStatQuery(GameSupportStatsAPIName.TOTAL_KILL, value => totalKills = value);
            AddStartupStatQuery(GameSupportStatsAPIName.TOTAL_RUNS, value => totalRuns = value);
            AddStartupStatQuery(GameSupportStatsAPIName.HIGHEST_DAY_COMPLETED, value => highestDayCompleted = value);
            pendingStatQueries = startupStatQueries.Count;
            QueryNextStartupStat();
            StartCoroutine(IEStartupStatQueryTimeout());
        }

        public void AddKills(int addedValue, bool saveImmediately = false)
        {
            if (!CanUseStats())
            {
                QueueAddKills(addedValue, saveImmediately);
                return;
            }

            totalKills += addedValue;
            Log($"AddKills. added={addedValue}, totalKills={totalKills}, saveImmediately={saveImmediately}.");

            if (saveImmediately)
                SetStatValue(GameSupportStatsAPIName.TOTAL_KILL, totalKills);
        }

        public void AddRuns(int addedValue, bool saveImmediately = false)
        {
            if (!CanUseStats())
            {
                QueueAddRuns(addedValue, saveImmediately);
                return;
            }

            totalRuns += addedValue;
            Log($"AddRuns. added={addedValue}, totalRuns={totalRuns}, saveImmediately={saveImmediately}.");

            if (saveImmediately)
                SetStatValue(GameSupportStatsAPIName.TOTAL_RUNS, totalRuns);
        }

        public void CompleteDay(int day, bool saveImmediately = false)
        {
            if (!CanUseStats())
            {
                QueueCompleteDay(day, saveImmediately);
                return;
            }

            if (day <= highestDayCompleted) return;

            highestDayCompleted = day;
            Log($"CompleteDay. highestDayCompleted={highestDayCompleted}, saveImmediately={saveImmediately}.");

            if (saveImmediately)
                SetStatValue(GameSupportStatsAPIName.HIGHEST_DAY_COMPLETED, highestDayCompleted);
        }

        public void SaveStats()
        {
            if (!CanUseStats())
            {
                QueueSaveStats();
                return;
            }

            Log($"SaveStats. totalKills={totalKills}, totalRuns={totalRuns}, highestDayCompleted={highestDayCompleted}.");
            SetStatValue(GameSupportStatsAPIName.TOTAL_KILL, totalKills);
            SetStatValue(GameSupportStatsAPIName.TOTAL_RUNS, totalRuns);
            SetStatValue(GameSupportStatsAPIName.HIGHEST_DAY_COMPLETED, highestDayCompleted);
        }

        public void TryClaimAchievement(string achievementName)
        {
            if (string.IsNullOrWhiteSpace(achievementName)) return;

            if (!CanUseStats())
            {
                QueueAchievement(achievementName);
                return;
            }

            var statId = ParseStatIdFromAchievementId(achievementName);
            
            Log($"TryClaimAchievement. achievementName='{achievementName}'.");
            STOVEPCSDK3GameSupportRequestQueue.Enqueue(complete =>
            {
                try
                {
                    GameSupport_Achievement(achievementName, (callbackResult, achievement) =>
                    {
                        STOVEPCSDK3Manager.Instance.PrintCallbackResult(callbackResult);
                        Log($"Achievement callback. achievementName='{achievementName}', success={callbackResult.result.IsSuccessful()}, resultCode={callbackResult.result.resultCode}, error='{callbackResult.errorMessage}', status='{achievement.status}'.");

                        if (callbackResult.result.IsSuccessful() &&
                            string.Equals(achievement.status, "ACHIEVED", StringComparison.OrdinalIgnoreCase))
                        {
                            complete();
                            return;
                        }

                        SetStatValue(statId, 1);
                        complete();
                    });
                }
                catch (Exception exception)
                {
                    Debug.LogError($"STOVE achievement query failed for '{achievementName}': {exception}");
                    SetStatValue(statId, 1);
                    complete();
                }
            });
        }

        private void AddStartupStatQuery(string statId, Action<int> onValue)
        {
            pendingStartupStatIds.Add(statId);
            startupStatQueries.Enqueue(new StartupStatQuery(statId, onValue));
        }

        private void QueryNextStartupStat()
        {
            if (startupStatsFinalized || startupStatQueries.Count <= 0)
                return;

            var query = startupStatQueries.Dequeue();
            QueryStat(query.StatId, query.OnValue);
        }

        private void QueryStat(string statId, Action<int> onValue)
        {
            STOVEPCSDK3GameSupportRequestQueue.Enqueue(complete =>
            {
                try
                {
                    Log($"QueryStat. statId='{statId}'.");
                    GameSupport_Stat(statId, (callbackResult, stat) =>
                    {
                        try
                        {
                            if (startupStatsFinalized)
                            {
                                Log($"Late stat callback ignored. statId='{statId}', value={stat.currentValue}.");
                                return;
                            }

                            STOVEPCSDK3Manager.Instance.PrintCallbackResult(callbackResult);
                            Log($"Stat callback. statId='{statId}', success={callbackResult.result.IsSuccessful()}, resultCode={callbackResult.result.resultCode}, error='{callbackResult.errorMessage}', value={stat.currentValue}.");

                            if (callbackResult.result.IsSuccessful())
                                onValue?.Invoke(stat.currentValue);

                            CompleteStartupStatQuery(statId);
                        }
                        finally
                        {
                            complete();
                        }
                    });
                }
                catch (Exception exception)
                {
                    Debug.LogError($"STOVE stat query failed for '{statId}': {exception}");
                    CompleteStartupStatQuery(statId);
                    complete();
                }
            });
        }

        private void SetStatValue(string statId, int value)
        {
            if (!STOVEPCSDK3Manager.Instance.IsGameSupportInitialized)
                return;

            STOVEPCSDK3GameSupportRequestQueue.Enqueue(complete =>
            {
                try
                {
                    Log($"SetStatValue. statId='{statId}', value={value}.");
                    GameSupport_ModifyStat(statId, value, (callbackResult, _) =>
                    {
                        STOVEPCSDK3Manager.Instance.PrintCallbackResult(callbackResult);
                        Log($"SetStatValue callback. statId='{statId}', value={value}, success={callbackResult.result.IsSuccessful()}, resultCode={callbackResult.result.resultCode}, error='{callbackResult.errorMessage}'.");
                        complete();
                    });
                }
                catch (Exception exception)
                {
                    Debug.LogError($"STOVE stat update failed for '{statId}': {exception}");
                    complete();
                }
            });
        }

        private bool CanUseStats()
        {
            if (!StatReceived)
            {
                return false;
            }

            if (!STOVEPCSDK3Manager.Instance.IsGameSupportInitialized)
            {
                Log("Stats call skipped because GameSupport SDK is not initialized.");
                return false;
            }

            return true;
        }

        private IEnumerator IEStartupStatQueryTimeout()
        {
            var timeoutSeconds = Mathf.Max(1f, startupStatQueryTimeoutSeconds);
            yield return new WaitForSecondsRealtime(timeoutSeconds);

            if (startupStatsFinalized || pendingStatQueries <= 0)
                yield break;

            Debug.LogWarning($"STOVE startup stat query timed out after {timeoutSeconds:0.#} seconds. Continuing with default values for {pendingStatQueries} missing stat callback(s): {string.Join(", ", pendingStartupStatIds)}.");
            FinalizeStartupStats();
        }

        private void CompleteStartupStatQuery(string statId)
        {
            if (startupStatsFinalized)
                return;

            if (pendingStartupStatIds.Remove(statId))
                pendingStatQueries--;

            if (pendingStatQueries <= 0)
            {
                FinalizeStartupStats();
                return;
            }

            QueryNextStartupStat();
        }

        private void FinalizeStartupStats()
        {
            if (startupStatsFinalized)
                return;

            startupStatsFinalized = true;
            pendingStatQueries = 0;
            pendingStartupStatIds.Clear();
            startupStatQueries.Clear();
            StatReceived = true;
            FlushPendingCalls();
        }

        private void QueueAddKills(int addedValue, bool saveImmediately)
        {
            if (!CanQueuePendingCall("AddKills")) return;

            pendingKillsToAdd += addedValue;
            pendingSaveStats |= saveImmediately;
            Log($"AddKills queued until startup stats are ready. added={addedValue}, pendingKillsToAdd={pendingKillsToAdd}, saveImmediately={saveImmediately}.");
        }

        private void QueueAddRuns(int addedValue, bool saveImmediately)
        {
            if (!CanQueuePendingCall("AddRuns")) return;

            pendingRunsToAdd += addedValue;
            pendingSaveStats |= saveImmediately;
            Log($"AddRuns queued until startup stats are ready. added={addedValue}, pendingRunsToAdd={pendingRunsToAdd}, saveImmediately={saveImmediately}.");
        }

        private void QueueCompleteDay(int day, bool saveImmediately)
        {
            if (!CanQueuePendingCall("CompleteDay")) return;

            pendingHighestDayCompleted = Mathf.Max(pendingHighestDayCompleted, day);
            pendingSaveStats |= saveImmediately;
            Log($"CompleteDay queued until startup stats are ready. day={day}, pendingHighestDayCompleted={pendingHighestDayCompleted}, saveImmediately={saveImmediately}.");
        }

        private void QueueSaveStats()
        {
            if (!CanQueuePendingCall("SaveStats")) return;

            pendingSaveStats = true;
            Log("SaveStats queued until startup stats are ready.");
        }

        private void QueueAchievement(string achievementName)
        {
            if (!CanQueuePendingCall("TryClaimAchievement")) return;

            if (!pendingAchievements.Contains(achievementName))
                pendingAchievements.Add(achievementName);

            Log($"TryClaimAchievement queued until startup stats are ready. achievementName='{achievementName}'.");
        }

        private bool CanQueuePendingCall(string callName)
        {
            if (StatReceived)
            {
                Log($"{callName} skipped because GameSupport SDK is not initialized.");
                return false;
            }

            if (startupStatsUnavailable)
            {
                Log($"{callName} skipped because startup stats are unavailable.");
                return false;
            }

            if (!initializeStarted)
            {
                Log($"{callName} requested before startup stats. Starting startup stat request now.");
                Initialize();
            }

            return true;
        }

        private void FlushPendingCalls()
        {
            if (!STOVEPCSDK3Manager.Instance.IsGameSupportInitialized)
            {
                ClearPendingCalls();
                return;
            }

            Log($"Startup stats ready. pendingKillsToAdd={pendingKillsToAdd}, pendingRunsToAdd={pendingRunsToAdd}, pendingHighestDayCompleted={pendingHighestDayCompleted}, pendingSaveStats={pendingSaveStats}, pendingAchievements={pendingAchievements.Count}.");

            if (pendingKillsToAdd != 0)
            {
                totalKills += pendingKillsToAdd;
                pendingKillsToAdd = 0;
            }

            if (pendingRunsToAdd != 0)
            {
                totalRuns += pendingRunsToAdd;
                pendingRunsToAdd = 0;
            }

            if (pendingHighestDayCompleted > highestDayCompleted)
                highestDayCompleted = pendingHighestDayCompleted;

            pendingHighestDayCompleted = -1;

            if (pendingSaveStats)
            {
                pendingSaveStats = false;
                SaveStats();
            }

            if (pendingAchievements.Count <= 0)
                return;

            var achievementsToClaim = pendingAchievements.ToArray();
            pendingAchievements.Clear();

            foreach (var achievementName in achievementsToClaim)
                TryClaimAchievement(achievementName);
        }

        private void ClearPendingCalls()
        {
            pendingKillsToAdd = 0;
            pendingRunsToAdd = 0;
            pendingHighestDayCompleted = -1;
            pendingSaveStats = false;
            pendingAchievements.Clear();
        }

        private void Log(string message)
        {
            if (!enableDebugLogs)
                return;

            Debug.Log($"[STOVE Stats] {message}");
        }

        private readonly struct StartupStatQuery
        {
            public StartupStatQuery(string statId, Action<int> onValue)
            {
                StatId = statId;
                OnValue = onValue;
            }

            public string StatId { get; }
            public Action<int> OnValue { get; }
        }

        private string ParseStatIdFromAchievementId(string achievementId)
        {
            if (string.IsNullOrEmpty(achievementId))
                return achievementId;

            var lastSeparatorIndex = achievementId.LastIndexOf('_');
            if (lastSeparatorIndex < 0)
                return achievementId;

            return achievementId.Substring(0, lastSeparatorIndex);
        }
    }
}
