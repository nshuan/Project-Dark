namespace Dark.Scripts.STOVE
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    internal static class STOVEPCSDK3GameSupportRequestQueue
    {
        private static readonly Queue<Action<Action>> Requests = new Queue<Action<Action>>();
        private static bool requestInProgress;

        public static void Enqueue(Action<Action> request)
        {
            if (request == null)
                return;

            Requests.Enqueue(request);
            TryRunNext();
        }

        private static void TryRunNext()
        {
            if (requestInProgress || Requests.Count <= 0)
                return;

            if (!STOVEPCSDK3Manager.Instance.IsGameSupportInitialized)
            {
                Requests.Clear();
                return;
            }

            requestInProgress = true;
            var request = Requests.Dequeue();
            var completed = false;

            void Complete()
            {
                if (completed)
                    return;

                completed = true;
                CompleteCurrentRequest();
            }

            try
            {
                request.Invoke(Complete);
            }
            catch (Exception exception)
            {
                Debug.LogError($"STOVE GameSupport request failed before callback registration: {exception}");
                Complete();
            }
        }

        private static void CompleteCurrentRequest()
        {
            if (!requestInProgress)
                return;

            requestInProgress = false;
            TryRunNext();
        }
    }
}
