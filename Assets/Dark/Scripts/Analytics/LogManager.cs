
using Core;
using Newtonsoft.Json;

namespace Dark.Scripts.Analytics
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using UnityEngine;

    public class LogManager : MonoSingleton<LogManager>
    {
        [Header("Batch Settings")]
        public int batchSize = 5;
        public float uploadInterval = 10f;

        private float timer;
        private string queuePath;
        private List<LogEntry> logQueue = new List<LogEntry>();

        [Header("Logger")]
        [SerializeField] private SupabaseLog supabaseLog;
        
        private void Start()
        {
            
#if UNITY_EDITOR
            queuePath = Path.Combine(Application.dataPath, "logqueue.json");
#else
            queuePath = Path.Combine(Application.persistentDataPath, "logqueue.json");
#endif
            LoadQueue();

            // Application.logMessageReceived += HandleUnityLog;
        }

        private void Update()
        {
            timer += Time.deltaTime;
            if (timer >= uploadInterval)
            {
                timer = 0;
                UploadNextBatch();
            }
        }

        // -------- PUBLIC API --------
        public static void Log(string eventType, string message)
        {
            if (!GameConst.EnableLog) return;
            if (Instance == null) return;

            var entry = new LogEntry(
                eventType,
                message,
                SystemInfo.deviceUniqueIdentifier,
                Application.version,
                Application.platform.ToString()
            );

            Instance.logQueue.Add(entry);
            Instance.SaveQueue();
        }

        // -------- UNITY ERROR LOGGING --------
        private void HandleUnityLog(string condition, string stackTrace, LogType type)
        {
            Log(type.ToString(), condition + "\n" + stackTrace);
        }

        // -------- BATCH PROCESSING --------
        public void UploadNextBatch()
        {
            if (logQueue.Count == 0)
                return;

            int count = Mathf.Min(batchSize, logQueue.Count);
            List<LogEntry> batch = logQueue.GetRange(0, count);

            // Hand over batch to Supabase
            supabaseLog.SendBatch(batch, OnBatchUploadedSuccess);
        }

        private void OnBatchUploadedSuccess(int uploadedCount)
        {
            logQueue.RemoveRange(0, uploadedCount);
            SaveQueue();
        }

        // -------- QUEUE SAVE/LOAD --------
        void SaveQueue()
        {
            File.WriteAllText(queuePath, JsonConvert.SerializeObject(logQueue));
        }

        void LoadQueue()
        {
            if (File.Exists(queuePath))
            {
                string json = File.ReadAllText(queuePath);
                var items = JsonConvert.DeserializeObject<List<LogEntry>>(json);
                logQueue = new List<LogEntry>(items);
            }
        }
    }

    [Serializable]
    public class LogEntry
    {
        public string event_type;
        public string message;
        public string device_id;
        public string build_version;
        public string platform;

        public LogEntry(string e, string m, string d, string b, string p)
        {
            event_type = e;
            message = m;
            device_id = d;
            build_version = b;
            platform = p;
        }
    }

}