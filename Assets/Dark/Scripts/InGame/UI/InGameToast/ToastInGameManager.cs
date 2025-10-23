using System;
using System.Collections.Generic;
using Core;
using UnityEngine;

namespace InGame.UI.InGameToast
{
    public class ToastInGameManager : Singleton<ToastInGameManager>, IDisposable
    {
        private event Action<ToastInGame, Action> onShowToast; 
        /// <summary>
        /// Action is invoked when a toast is called to show,
        /// giving the information of the toast and callback to call when it is completed showing
        /// </summary>
        public event Action<ToastInGame, Action> OnShowToast
        {
            add => onShowToast += value;
            remove => onShowToast -= value;
        }
        
        private Queue<ToastInGame> queue;
        private bool isPending;

        public ToastInGameManager()
        {
            queue = new Queue<ToastInGame>();
        }

        /// <summary>
        /// Register a toast
        /// The registered will be added to queue
        /// </summary>
        /// <param name="message"></param>
        /// <param name="icon"></param>
        public void Register(string message, Sprite icon)
        {
            queue.Enqueue(new ToastInGame(message, icon));
            CheckAll();
        }

        /// <summary>
        /// Check to show toast
        /// If there is 1 activating toast, ignore,
        /// else show the first toast in queue (if queue length is > 0)
        /// </summary>
        public void CheckAll()
        {
            if (isPending) return;
            if (queue.Count == 0) return;
            isPending = true;
            var toast = queue.Dequeue();
            onShowToast?.Invoke(toast, () =>
            {
                isPending = false;
                CheckAll();
            });
        }

        public void Dispose()
        {
            _instance = null;
        }
    }
    
    public class ToastInGame
    {
        public string message;
        public Sprite icon;

        public ToastInGame(string message, Sprite icon)
        {
            this.message = message;
            this.icon = icon;
        }
    }
}