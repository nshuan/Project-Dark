using System;
using System.Collections.Generic;
using Dark.Scripts.ForDemo;
using Economic.InGame.DropItems;
using InGame;
using UnityEngine;

namespace Economic.InGame
{
    /// <summary>
    /// Handles "absorb to mouse" behaviour for 2D items with colliders (no Rigidbody required).
    /// Attach this to a GameObject (e.g. same as EItemDropMouseCollect) and
    /// set which layers are considered "items".
    /// Items will fly towards the mouse with an "in-back" like easing.
    /// If the mouse moves very fast, the items will smoothly slow down and stop instead of snapping.
    /// </summary>
    public class EItemDropMouseAbsorb : MonoBehaviour
    {
        [Header("Detection")]
        [SerializeField] private float radius = 2f;
        [SerializeField] private LayerMask itemLayer;

        [Header("Movement")]
        [Tooltip("How long (in seconds) it takes an item to reach the mouse position.")]
        [SerializeField] private float absorbDuration = 0.35f;

        [Tooltip("Maximum movement speed of the item.")]
        [SerializeField] private float maxMoveSpeed = 20f;

        [Tooltip("Mouse speed (units / second in world space) above which the item starts to smoothly stop.")]
        [SerializeField] private float mouseStopSpeed = 25f;

        [Tooltip("How quickly the item slows to a stop when mouse is too fast. Higher = faster stop.")]
        [SerializeField] private float stopSmooth = 10f;

        [Tooltip("Maximum distance from mouse before item stops following and is released. Set to 0 to disable.")]
        [SerializeField] private float maxDistance = 5f;

        [Tooltip("Optional extra distance at which items start being absorbed (0 = exactly at radius).")]
        [SerializeField] private float absorbMargin = 0.05f;

        private readonly RaycastHit2D[] _hits = new RaycastHit2D[64];

        private struct AbsorbState
        {
            public Vector3 startPos;
            public float t;
            public Vector3 velocity;
        }

        // We track the Transform directly; no Rigidbody is required.
        private readonly Dictionary<Transform, AbsorbState> _active = new Dictionary<Transform, AbsorbState>();
        // Temporary buffer to safely iterate active items without modifying the collection during enumeration.
        private readonly List<Transform> _updateBuffer = new List<Transform>();
        [SerializeField] private Camera _cam;
        private Vector3 _lastMouseWorld;
        private bool _hasLastMouse;

        private void Start()
        {
            if (DemoConfig.CollectLogicType == 2 && !_cam)
                _cam = Camera.main;
        }

        private void LateUpdate()
        {
            if (!_cam)
            {
                if (DemoConfig.CollectLogicType == 2)
                    _cam = VisualEffectHelper.Instance.DefaultCamera;
            }
            if (!_cam) return;

            Vector3 mouseWorld = _cam.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0f;

            float dt = Time.deltaTime;

            // Mouse "speed" in world units / second
            float mouseSpeed = 0f;
            if (_hasLastMouse && dt > 0f)
            {
                mouseSpeed = Vector3.Distance(mouseWorld, _lastMouseWorld) / dt;
            }
            _lastMouseWorld = mouseWorld;
            _hasLastMouse = true;

            // 1. Discover nearby items under the mouse radius.
            int count = Physics2D.CircleCastNonAlloc(mouseWorld, radius + absorbMargin, Vector2.zero, _hits, 0f, itemLayer);
            for (int i = 0; i < count; i++)
            {
                var col = _hits[i].collider;
                if (!col) continue;
                if (!col.CompareTag("Collectible")) continue;

                Transform target = col.transform;

                if (!_active.ContainsKey(target))
                {
                    _active[target] = new AbsorbState
                    {
                        startPos = target.position,
                        t = 0f,
                        velocity = Vector3.zero
                    };
                }
            }

            // 2. Update movement of all actively absorbed items.
            var toClear = new List<Transform>();

            // Copy keys to buffer so we can modify _active safely while iterating.
            _updateBuffer.Clear();
            foreach (var kvp in _active)
            {
                _updateBuffer.Add(kvp.Key);
            }

            foreach (var tr in _updateBuffer)
            {
                if (!tr)
                {
                    toClear.Add(tr);
                    continue;
                }

                if (!_active.TryGetValue(tr, out var state))
                {
                    continue;
                }

                // Progress (0 → 1) over absorbDuration.
                state.t += dt / Mathf.Max(0.0001f, absorbDuration);
                float eased = InBack01(Mathf.Clamp01(state.t));

                // Desired position using easing along the direction from startPos to current mouse position.
                Vector3 dir = mouseWorld - state.startPos;
                Vector3 desiredPos = state.startPos + dir * eased;

                // Clamp desiredPos to never exceed mouse position (prevent overshooting).
                float desiredDist = Vector2.Distance(state.startPos, desiredPos);
                float maxDist = Vector2.Distance(state.startPos, mouseWorld);
                if (desiredDist > maxDist)
                {
                    desiredPos = mouseWorld;
                }

                Vector3 current = tr.position;
                float distanceToMouse = Vector2.Distance(current, mouseWorld);

                // Check if mouse is too far away - stop
                if (maxDistance > 0f && distanceToMouse > maxDistance)
                {
                    // Smoothly stop the item.
                    state.velocity = Vector3.Lerp(state.velocity, Vector3.zero, stopSmooth * dt);
                    
                    // Apply remaining velocity.
                    current += state.velocity * dt;
                    tr.position = current;

                    _active[tr] = state;
                    continue;
                }

                if (mouseSpeed > mouseStopSpeed)
                {
                    // Mouse is too fast: smoothly reduce velocity to zero (item eases to a stop).
                    state.velocity = Vector3.Lerp(state.velocity, Vector3.zero, stopSmooth * dt);
                }
                else
                {
                    // Normal follow: move towards desiredPos, limited by maxMoveSpeed.
                    Vector3 toTarget = desiredPos - current;

                    // Desired velocity to reach the target in this frame (clamped).
                    Vector3 desiredVel = Vector3.zero;
                    if (dt > 0f)
                    {
                        desiredVel = toTarget / dt;
                        if (desiredVel.magnitude > maxMoveSpeed)
                        {
                            desiredVel = desiredVel.normalized * maxMoveSpeed;
                        }
                    }

                    // Smoothly blend towards desired velocity.
                    state.velocity = Vector3.Lerp(state.velocity, desiredVel, 10f * dt);
                }

                // Apply velocity.
                current += state.velocity * dt;
                
                // Clamp position to never go past the mouse (prevent overshooting).
                float distFromStart = Vector2.Distance(state.startPos, current);
                float distStartToMouse = Vector2.Distance(state.startPos, mouseWorld);
                
                // If we've passed the mouse position, clamp to mouse position.
                if (distFromStart > distStartToMouse)
                {
                    current = mouseWorld;
                    state.velocity = Vector3.zero;
                }
                
                tr.position = current;

                // Optional: when it's very close to the mouse, remove from active list.
                float newDistanceToMouse = Vector2.Distance(tr.position, mouseWorld);
                if (newDistanceToMouse < 0.15f)
                {
                    toClear.Add(tr);
                }
                else
                {
                    _active[tr] = state;
                }
            }

            // 3. Cleanup finished / null transforms.
            for (int i = 0; i < toClear.Count; i++)
            {
                _active.Remove(toClear[i]);
                if (toClear[i].TryGetComponent<EItemDrop>(out var item))
                {
                    item.MarkedNotCollectedByManager = true;
                    EItemDropManager.Instance.Claim(item);
                    EItemDropPool.Instance.Release(item);
                }
            }
        }

        /// <summary>
        /// Custom in-back easing (0 → 1).
        /// Similar to standard easeInBack (overshoots negatively then comes back).
        /// </summary>
        private static float InBack01(float t)
        {
            const float s = 1.70158f;
            return t * t * ((s + 1f) * t - s);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.35f);
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}


