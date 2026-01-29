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
        [SerializeField] private EItemDropMouseTrigger trigger;
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
        
        [Header("Performance")]
        [SerializeField] private int maxActiveItems = 24;
        [SerializeField] private int maxActiveAbsorbing = 24;

        const float CLAIM_DISTANCE = 0.25f;
        private readonly RaycastHit2D[] _hits = new RaycastHit2D[64];
        private readonly List<int> _toClear = new List<int>(64);

        private struct AbsorbState
        {
            public Transform transform;
            public EItemDrop targetItem;
            public Vector3 originalPos;
            public Vector3 pushTargetPos;
            public float timeSinceDetected;
            public bool hasPushed;
            public bool isAbsorbing;
        }

        // We track the Transform directly; no Rigidbody is required.
        private readonly Dictionary<int, AbsorbState> _active = new Dictionary<int, AbsorbState>();
        // Temporary buffer to safely iterate active items without modifying the collection during enumeration.
        private readonly List<int> _updateBuffer = new List<int>();
        [SerializeField] private Camera _cam;
        [SerializeField] private Transform mouseFollower;
        
        private MonoCursor cursor;
        private float maxDelayHideCursor = 0.5f;
        private float delayHideCursorCounter;
        private bool chargeBlockCollect;
        private int tempItemCollectedCount;
        
        public bool IsChargeBlockCollect => chargeBlockCollect;

        private void Awake()
        {
            trigger.Init(this);
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
            trigger.collider.radius = radius + absorbMargin;
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

            UpdateActiveOnly(mouseWorld, dt);
        }

        public void TryRegisterItem(Collider2D col)
        {
            if (!col) return;
            if (!col.CompareTag("Collectible")) return;
            if (_active.ContainsKey(col.transform.GetInstanceID())) return;
            if (col.transform.TryGetComponent<EItemDrop>(out var targetItem) && !targetItem.Collectible) return;

            Transform target = col.transform;
            var instanceId = target.GetInstanceID();

            // Calculate push direction (slight push away from mouse)
            Vector3 mouseWorld = mouseFollower.position;
            Vector3 toItem = target.position - mouseWorld;
            Vector3 pushDir = toItem.normalized;
            if (pushDir.magnitude < 0.01f)
            {
                // If item is exactly at mouse, push in a random direction
                float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
                pushDir = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);
            }
            Vector3 pushTarget = target.position + pushDir * pushDistance;

            _active[instanceId] = new AbsorbState
            {
                transform = target,
                targetItem = targetItem,
                originalPos = target.position,
                pushTargetPos = pushTarget,
                timeSinceDetected = 0f,
                hasPushed = false
            };
        }
        
        public void TryUnregisterItem(Collider2D col)
        {
            int id = col.transform.GetInstanceID();

            if (_active.ContainsKey(id))
            {
                _toClear.Add(id);
            }
        }
        
        private void UpdateActiveOnly(Vector3 mouseWorld, float dt)
        {
            // 2. Update movement of all actively absorbed items.
            _toClear.Clear();

            // Copy keys to buffer so we can modify _active safely while iterating.
            _updateBuffer.Clear();
            foreach (var kvp in _active)
            {
                _updateBuffer.Add(kvp.Key);
            }

            int absorbingCount = 0;
            foreach (var s in _active.Values)
                if (s.isAbsorbing) absorbingCount++;

            if (absorbingCount < maxActiveAbsorbing)
            {
                foreach (var id in _updateBuffer)
                {
                    if (absorbingCount >= maxActiveAbsorbing)
                        break;

                    var state = _active[id];
                    if (!state.isAbsorbing)
                    {
                        state.isAbsorbing = true;
                        _active[id] = state;
                        absorbingCount++;
                    }
                }
            }
            
            var isCollecting = false;
            foreach (var id in _updateBuffer)
            {
                if (id == 0)
                {
                    _toClear.Add(id);
                    continue;
                }
                
                if (!_active.TryGetValue(id, out var state))
                {
                    continue;
                }
                if (!state.isAbsorbing) continue; // queued, do nothing this frame
                if (!state.targetItem.Collectible) continue;

                state.timeSinceDetected += dt;
                Vector3 current = state.transform.position;

                // Phase 1: Push effect (immediate, slight movement)
                if (!state.hasPushed)
                {
                    // Apply push immediately
                    float pushProgress = Mathf.Clamp01(state.timeSinceDetected / pushDuration);
                    current = Vector3.Lerp(state.originalPos, state.pushTargetPos, pushProgress);
                    state.transform.position = current;
                    
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
                        state.transform.position = current;
                        isCollecting = true;
                    }
                    else
                    {
                        // Very close to mouse, snap to it and mark for cleanup
                        state.transform.position = mouseWorld;
                        _toClear.Add(id);
                        continue;
                    }
                }
                // Phase 2 delay: Just wait (push is done, waiting for moveDelay)
                // Keep item at pushed position during delay

                // Update state
                _active[id] = state;

                // Safety check: if item is very close to mouse, mark for cleanup
                float finalDistanceToMouse = Vector2.Distance(state.transform.position, mouseWorld);
                if (finalDistanceToMouse < CLAIM_DISTANCE)
                {
                    _toClear.Add(id);
                }
            }

            UpdateCursor(isCollecting);
            CleanupCollected();
        }

        private void CleanupCollected()
        {
            // 3. Cleanup finished / null transforms.
            var isClaim = false;
            for (int i = 0; i < _toClear.Count; i++)
            {
                if (_active.TryGetValue(_toClear[i], out var state))
                {
                    if (state.targetItem)
                    {
                        state.targetItem.MarkedNotCollectedByManager = true;
                        state.targetItem.DoClaimedVisual(mouseFollower);
                        EItemDropManager.Instance.Claim(state.targetItem);
                        EItemDropPool.Instance.Release(state.targetItem);
                        isClaim = true;
                    }
                    _active.Remove(_toClear[i]);
                }
            }
            if (isClaim)
                cursor?.PunchCollectCursor();
        }

        private void UpdateCursor(bool collecting)
        {
            // Update cursor
            if (collecting)
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


