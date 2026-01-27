
using Core;
using Newtonsoft.Json;

namespace Dark.Scripts.Analytics
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using UnityEngine;
    using UnityEngine.Networking;

    public class SupabaseLog : MonoBehaviour
    {
        [Header("Supabase Settings")]
        [SerializeField] private string projectUrl = "https://YOUR_PROJECT_ID.supabase.co";
        [SerializeField] private string apiKey = "YOUR_ANON_KEY";
        [SerializeField] private string tableName = "game_logs";
        
        // Called by LogManager
        public void SendBatch(List<LogEntry> batch, System.Action<int> onSuccess)
        {
            StartCoroutine(UploadBatchRoutine(batch, onSuccess));
        }

        IEnumerator UploadBatchRoutine(List<LogEntry> batch, System.Action<int> onSuccess)
        {
            string url = projectUrl + "/rest/v1/" + tableName;
            string json = JsonConvert.SerializeObject(batch);
            var data = Encoding.UTF8.GetBytes(json);
#if UNITY_EDITOR
            DebugUtility.Log($"SupabaseLog: Uploading {batch.Count} logs. Data: \n{json}");
#else
            if (GameConst.EnableLogManagerDebugLog)
                DebugUtility.Log($"SupabaseLog: Uploading {batch.Count} logs. Data: \n{json}");
#endif

            UnityWebRequest req = new UnityWebRequest(url, "POST");
            req.uploadHandler = new UploadHandlerRaw(data);
            req.downloadHandler = new DownloadHandlerBuffer();

            req.SetRequestHeader("apikey", apiKey);
            req.SetRequestHeader("Content-Type", "application/json");
            req.SetRequestHeader("Prefer", "return=minimal");

            yield return req.SendWebRequest();

            if (!req.isNetworkError && !req.isHttpError)
            {
                onSuccess?.Invoke(batch.Count);
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogWarning("Supabase upload failed: " + req.error);
#else
            if (GameConst.EnableLogManagerDebugLog)
                Debug.LogWarning("Supabase upload failed: " + req.error);
#endif
            }
        }
    }

}