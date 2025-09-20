using UnityEngine;

namespace InGame
{
    public class MoveCollectResource : IMoveMouseInput
    {
        private MoveCollectResourceCursor collector;
        private Camera Cam { get; set; }
        public bool CanMove { get; }
        public void OnUpdate()
        {
            collector.transform.position = Cam.ScreenToWorldPoint(Input.mousePosition);
        }

        public MoveCollectResource(Camera cam, Transform target)
        {
            Cam = cam;
            
            var collectorGo = new GameObject("[Resource Collector]")
            {
                transform =
                {
                    localPosition = cam.ScreenToWorldPoint(Input.mousePosition)
                }
            };
            var collectorCld = collectorGo.AddComponent<CircleCollider2D>();
            collectorCld.isTrigger = true;
            collectorCld.radius = 0.7f;
            var collectorRb = collectorGo.AddComponent<Rigidbody2D>();
            collectorRb.isKinematic = true;
            collectorRb.simulated = true;
            collectorRb.gravityScale = 0f;
            collector = collectorGo.AddComponent<MoveCollectResourceCursor>();
            
            collector.Target = target;
            collector.Enabled = true;
        }
    }
}