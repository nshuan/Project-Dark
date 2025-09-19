using System;
using Dark.Scripts.Utils;
using Economic;
using UnityEngine;

namespace InGame.Economic.DropItems
{
    public class EItemDrop : MonoBehaviour
    {
        [SerializeField] private GameObject vfxClaim;
        
        public WealthType kind;
        public int Quantity { get; set; }
        
        public void Drop()
        {
            
        }

        public void Collect(Action onCompleteVfx)
        {
            vfxClaim.SetActive(true);
            this.DelayCall(1f, () =>
            {
                vfxClaim.SetActive(false);
                onCompleteVfx?.Invoke();
            });
        }
    }
}