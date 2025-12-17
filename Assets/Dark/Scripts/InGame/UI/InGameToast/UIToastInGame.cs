using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace InGame.UI.InGameToast
{
    public class UIToastInGame : MonoBehaviour
    {
        [SerializeField] private UIToastInGameItem prefab;

        private Queue<UIToastInGameItem> itemPool;
        
        private void Awake()
        {
            itemPool = new Queue<UIToastInGameItem>();
            
            ToastInGameManager.Instance.OnShowToast += OnShowToast;
            // LevelManager.Instance.OnWin += ClearActions;
            // LevelManager.Instance.OnLose += ClearActions;
        }

        private void OnDestroy()
        {
            ClearActions();
        }

        private void ClearActions()
        {
            ToastInGameManager.Instance.OnShowToast -= OnShowToast;
        }

        private void OnShowToast(ToastInGame toast, Action callbackShown)
        {
            if (string.IsNullOrEmpty(toast.message)) return;

            if (!itemPool.TryDequeue(out var item))
            {
                item = Instantiate(prefab, transform);
            }
            
            item.transform.SetAsLastSibling();
            item.ShowToast(toast, () =>
            {
                item.gameObject.SetActive(false);
                itemPool.Enqueue(item);
            });
            
            // Logic của ToastManager đang là đợi show xong tắt toast rồi mới show cái khác, nhưng mà ở đây sẽ show liên lục luôn nên cần invoke callback luôn
            callbackShown?.Invoke();
        }
    }
}