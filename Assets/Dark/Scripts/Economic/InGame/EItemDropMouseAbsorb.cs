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
    /// When the mouse moves to an item, it gets a slight push, then after a short delay moves to the mouse.
    /// </summary>
    public class EItemDropMouseAbsorb : MonoBehaviour
    {
        [Header("Detection")]
        [SerializeField] private float radius = 2f;
        [SerializeField] private LayerMask itemLayer;

        [Header("Movement")] 
        [Tooltip("Duration for the item to reach the pushed position")] 
        [SerializeField] private float pushDuration = 0.5f;
        
        [Tooltip("Delay before the item starts moving to the mouse (in seconds).")]
        [SerializeField] private float moveDelay = 0.1f;

        [Tooltip("How long (in seconds) it takes an item to reach the mouse position after the delay.")]
        [SerializeField] private float absorbDuration = 0.35f;

        [Tooltip("Maximum movement speed of the item.")]
        [SerializeField] private float maxMoveSpeed = 20f;

        [Tooltip("Distance of the push effect when mouse first approaches the item.")]
        [SerializeField] private float pushDistance = 0.15f;

        [Tooltip("Optional extra distance at which items start being absorbed (0 = exactly at radius).")]
        [SerializeField] private float absorbMargin = 0.05f;

        private readonly RaycastHit2D[] _hits = new RaycastHit2D[64];

        private struct AbsorbState
        {
            public EItemDrop targetItem;
            public Vector3 originalPos;
            public Vector3 pushTargetPos;
            public float timeSinceDetected;
            public bool hasPushed;
        }

        // We track the Transform directly; no Rigidbody is required.
        private readonly Dictionary<Transform, AbsorbState> _active = new Dictionary<Transform, AbsorbState>();
        // Temporary buffer to safely iterate active items without modifying the collection during enumeration.
        private readonly List<Transform> _updateBuffer = new List<Transform>();
        [SerializeField] private Camera _cam;
        [SerializeField] private Transform mouseFollower;

        private MonoCursor cursor;
        private float maxDelayHideCursor = 0.5f;
        private float delayHideCursorCounter;
        private bool chargeBlockCollect;

        private void Awake()
        {
            CombatActions.OnInitInGameCursor += OnInitCursor;
            CombatActions.OnChargeStarted += () => chargeBlockCollect = true;
            CombatActions.OnChargeEnded += () => chargeBlockCollect = false;
        }

        private void Start()
        {
            if (DemoConfig.CollectLogicType == 2 && !_cam)
                _cam = Camera.main;

            LevelManager.Instance.OnLevelPreLoaded += OnLevelPreLoaded;
        }

        private void OnLevelPreLoaded(LevelConfig level)
        {
            radius = LevelUtilityV2.GetVestigeCollectSize();
        }

        private void OnInitCursor(MonoCursor value)
        {
            cursor = value;
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
            mouseFollower.position = mouseWorld;

            float dt = Time.deltaTime;

            // 1. Discover nearby items under the mouse radius.
            var count = 0;
            if (chargeBlockCollect == false)
            {
                count = Physics2D.CircleCastNonAlloc(mouseWorld, radius + absorbMargin, Vector2.zero, _hits, 0f, itemLayer);
                for (int i = 0; i < count; i++)
                {
                    var col = _hits[i].collider;
                    if (!col) continue;
                    if (!col.CompareTag("Collectible")) continue;
                    if (col.transform.TryGetComponent<EItemDrop>(out var targetItem) && !targetItem.Collectible) continue;

                    Transform target = col.transform;

                    if (!_active.ContainsKey(target))
                    {
                        // Calculate push direction (slight push away from mouse)
                        Vector3 toItem = target.position - mouseWorld;
                        Vector3 pushDir = toItem.normalized;
                        if (pushDir.magnitude < 0.01f)
                        {
                            // If item is exactly at mouse, push in a random direction
                            float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
                            pushDir = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);
                        }
                        Vector3 pushTarget = target.position + pushDir * pushDistance;

                        _active[target] = new AbsorbState
                        {
                            targetItem = targetItem,
                            originalPos = target.position,
                            pushTargetPos = pushTarget,
                            timeSinceDetected = 0f,
                            hasPushed = false
                        };
                    }
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

            var isCollecting = false;
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
                
                if (!state.targetItem.Collectible) continue;

                state.timeSinceDetected += dt;
                Vector3 current = tr.position;

                // Phase 1: Push effect (immediate, slight movement)
                if (!state.hasPushed)
                {
                    // Apply push immediately
                    float pushProgress = Mathf.Clamp01(state.timeSinceDetected / pushDuration);
                    current = Vector3.Lerp(state.originalPos, state.pushTargetPos, pushProgress);
                    tr.position = current;
                    
                    // Mark as pushed after a brief moment
                    if (state.timeSinceDetected >= pushDuration)
                    {
                        state.hasPushed = true;
                    }
                }
                // Phase 2: Move to mouse (after delay)
                else if (state.timeSinceDetected >= moveDelay)
                {
                    // Move towards mouse with speed limit (always tries to reach mouse, no stopping)
                    Vector3 toMouse = mouseWorld - current;
                    float distanceToMouse = toMouse.magnitude;
                    
                    if (distanceToMouse > 0.01f)
                    {
                        // Move towards mouse, limited by maxMoveSpeed
                        float moveDistance = Mathf.Min(distanceToMouse, maxMoveSpeed * dt);
                        current += toMouse.normalized * moveDistance;
                        tr.position = current;
                        isCollecting = true;
                    }
                    else
                    {
                        // Very close to mouse, snap to it and mark for cleanup
                        tr.position = mouseWorld;
                        toClear.Add(tr);
                        continue;
                    }
                }
                // Phase 2 delay: Just wait (push is done, waiting for moveDelay)
                else
                {
                    // Keep item at pushed position during delay
                    tr.position = current;
                }

                // Update state
                _active[tr] = state;

                // Safety check: if item is very close to mouse, mark for cleanup
                float finalDistanceToMouse = Vector2.Distance(tr.position, mouseWorld);
                if (finalDistanceToMouse < 0.15f)
                {
                    toClear.Add(tr);
                }
            }
            
            // Update cursor
            if (count > 0 || isCollecting)
            {
                cursor?.SetCollectCursor(true);
                delayHideCursorCounter = maxDelayHideCursor;
            }
            else
            {
                delayHideCursorCounter -= Time.deltaTime;
                if (delayHideCursorCounter <= 0f)
                    cursor?.SetCollectCursor(false);
            }

            // 3. Cleanup finished / null transforms.
            var isClaim = false;
            for (int i = 0; i < toClear.Count; i++)
            {
                _active.Remove(toClear[i]);
                if (toClear[i].TryGetComponent<EItemDrop>(out var item))
                {
                    item.MarkedNotCollectedByManager = true;
                    item.DoClaimedVisual(mouseFollower);
                    EItemDropManager.Instance.Claim(item);
                    EItemDropPool.Instance.Release(item);
                    isClaim = true;
                }
            }
            if (isClaim)
                cursor.PunchCollectCursor();
        }
        
        private void OnDrawGizmosSelected()
        {
            if (!_cam) return;
            Vector3 mouseWorld = _cam.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0f;
            
            Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.35f);
            Gizmos.DrawWireSphere(mouseWorld, radius);
        }
    }
}


