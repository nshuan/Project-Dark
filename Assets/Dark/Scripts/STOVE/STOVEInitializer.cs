using UnityEngine;

namespace Dark.Scripts.STOVE
{
    public sealed class STOVEInitializer : MonoBehaviour
    {
        [SerializeField] private string myShopKey;
        
        private void Awake()
        {
            STOVEPCSDK3Manager.Instance.Initialize(myShopKey);
        }
    }
}