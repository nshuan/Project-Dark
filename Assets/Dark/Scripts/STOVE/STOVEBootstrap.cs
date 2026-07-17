using UnityEngine;

namespace Dark.Scripts.STOVE
{
    public sealed class STOVEBootstrap : MonoBehaviour
    {
        [SerializeField] private bool initializeOnAwake = true;
        [SerializeField] private STOVEPCSDK3Config config = STOVEPCSDK3Config.Default();

        private void Awake()
        {
            if (!initializeOnAwake)
                return;

            STOVEPCSDK3Manager.Instance.Initialize(config);
        }
    }
}
