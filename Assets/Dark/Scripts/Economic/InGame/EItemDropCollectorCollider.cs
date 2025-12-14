using UnityEngine;

namespace Economic.InGame
{
    public class EItemDropCollectorCollider : MonoBehaviour
    {
        [SerializeField] private EItemDropCollector collector;
        
        public void Break()
        {
            collector.Break();    
        }
    }
}