using System.Linq;
using DefaultNamespace;
using DG.Tweening;
using InGame.Effects;
using UnityEngine;

namespace InGame.MouseInput
{
    public class MoveBoxShot : IMouseInput
    {
        private Camera Cam { get; set; }
        private RectTransform cursor;
        private Vector3 mousePosition;
        private CameraShake effectCamShake;
        
        public MoveBoxShot(Camera cam, RectTransform cursor)
        {
            Cam = cam;
            effectCamShake = new CameraShake() { Cam = cam };
            this.cursor = cursor;
        }
        
        public void OnMouseClick()
        {
            // Check hit enemy, only nearest enemy is hit
            var mousePos = Cam.ScreenToWorldPoint(mousePosition);
            var hits = Physics2D.BoxCastAll(mousePos, WorldUtility.GetWorldSize(Cam, cursor), 0, Vector2.zero, 0f, LayerMask.GetMask("Entity"));
            var damage = Random.Range(15f, 30f);
            foreach (var hit in hits)
            {
                if (hit.collider != null && hit.transform.TryGetComponent<EnemyEntity>(out var enemyEntity))
                {
                    enemyEntity.OnHit(damage);
                }
            }
            // Do cursor effect
            DOTween.Complete(this);
            EffectHelper.Instance.PlayEffect(effectCamShake);
            var seq = DOTween.Sequence(this);
            seq.Append(cursor.transform.DOPunchScale(0.2f * Vector3.one, 0.13f))
                .Join(cursor.transform.DOShakeRotation(0.13f, new Vector3(0f, 0f, 10f)));
            seq.Play();
        }

        public void OnUpdate()
        {
            mousePosition = Input.mousePosition;
            mousePosition.z = 0; // Set z to 0 for 2D
            cursor.position = mousePosition;
            
#if UNITY_EDITOR
            var corners = new Vector3[4];
            cursor.GetWorldCorners(corners);
            corners = corners.Select((corner) => Cam.ScreenToWorldPoint(corner)).ToArray();
            
            // Draw lines between corners to visualize the box
            Debug.DrawLine(corners[0], corners[1], Color.red); // Bottom Left -> Top Left
            Debug.DrawLine(corners[1], corners[2], Color.red); // Top Left -> Top Right
            Debug.DrawLine(corners[2], corners[3], Color.red); // Top Right -> Bottom Right
            Debug.DrawLine(corners[3], corners[0], Color.red); // Bottom Right -> Bottom Left
            
            var ray = Cam.ScreenPointToRay(mousePosition);

            // Default: draw a ray forward
            // Raydistance = 100f
            var rayEnd = ray.origin + ray.direction * 100f;

            // Draw the ray in Scene view
            Debug.DrawLine(ray.origin, rayEnd, Color.green);
#endif
        }

        public void OnDrawGizmos()
        {
            DrawBoxGizmo(Cam.ScreenToWorldPoint(mousePosition), WorldUtility.GetWorldSize(Cam, cursor), 0f, Color.cyan);
        }
        
        private void DrawBoxGizmo(Vector2 center, Vector2 size, float angle, Color color)
        {
            var half = size / 2f;

            var rot = Quaternion.Euler(0, 0, angle);
            var corners = new Vector2[4]
            {
                center + new Vector2(-half.x, -half.y),
                center + new Vector2(-half.x,  half.y),
                center + new Vector2( half.x,  half.y),
                center + new Vector2( half.x, -half.y)
            };

            Debug.DrawLine(corners[0], corners[1], color, 1f);
            Debug.DrawLine(corners[1], corners[2], color, 1f);
            Debug.DrawLine(corners[2], corners[3], color, 1f);
            Debug.DrawLine(corners[3], corners[0], color, 1f);
        }
    }
}