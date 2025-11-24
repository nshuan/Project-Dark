#if (UNITY_2017_4 || UNITY_2018_1_OR_NEWER)
#define SPINE_UNITY_2018_PREVIEW_API
#endif

using UnityEditor;
using UnityEngine;
using Spine;
using Spine.Unity;
using Spine.Unity.Editor;
using System;

namespace Dark.Tools.SkeletonPreviewEditor
{
    public class SkeletonPreviewEditorWindow : EditorWindow
    {
        private SkeletonDataAsset _skeletonDataAsset;
        
        private string[] _animationNames;
        private string[] _skinNames;
        private int _selectedAnimationIndex = 0;
        private int _selectedSkinIndex = 0;
        
        private float _currentTime = 0f;
        private float _animationDuration = 0f;
        private bool _isPlaying = false;
        private double _lastUpdateTime;
        private float _animationLastTime;
        
        // Preview rendering
        private PreviewRenderUtility _previewRenderUtility;
        private GameObject _previewGameObject;
        private SkeletonAnimation _previewSkeletonAnimation;
        private Texture _previewTexture;
        private bool _requiresRefresh = true;
        private float _cameraOrthoSize = 1f;
        private float _cameraManualOrthoSize = 1f;
        private Vector3 _cameraPosition = new Vector3(0, 0, -10f);
        
        private const float FrameRate = 30f;
        private const float MinWindowWidth = 350f;
        private const float MinWindowHeight = 500f;
        private const float PreviewHeight = 300f;
        private const int PreviewLayer = 30;
        private const int PreviewCameraCullingMask = 1 << PreviewLayer;

        [MenuItem("Dark/Tools/Skeleton Preview Editor")]
        public static void ShowWindow()
        {
            var window = GetWindow<SkeletonPreviewEditorWindow>("Skeleton Preview");
            window.minSize = new Vector2(MinWindowWidth, MinWindowHeight);
        }

        private void OnEnable()
        {
            _lastUpdateTime = EditorApplication.timeSinceStartup;
            _animationLastTime = (float)EditorApplication.timeSinceStartup;
            EditorApplication.update += OnUpdate;
            InitializePreviewRenderUtility();
        }

        private void OnDisable()
        {
            EditorApplication.update -= OnUpdate;
            CleanupPreview();
        }

        private void OnDestroy()
        {
            CleanupPreview();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Skeleton Preview", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // Skeleton Data Asset field
            EditorGUI.BeginChangeCheck();
            _skeletonDataAsset = (SkeletonDataAsset)EditorGUILayout.ObjectField(
                "Skeleton Data Asset", 
                _skeletonDataAsset, 
                typeof(SkeletonDataAsset), 
                false
            );
            
            if (EditorGUI.EndChangeCheck())
            {
                OnSkeletonDataAssetChanged();
            }

            EditorGUILayout.Space();

            // Get the active skeleton data
            SkeletonData skeletonData = GetSkeletonData();
            
            if (skeletonData == null)
            {
                EditorGUILayout.HelpBox("Please assign a Skeleton Data Asset.", MessageType.Info);
                
                // Draw empty preview area
                Rect previewRect = GUILayoutUtility.GetRect(0, PreviewHeight, GUILayout.ExpandWidth(true));
                EditorGUI.DrawRect(previewRect, new Color(0.3f, 0.3f, 0.3f, 1f));
                return;
            }

            // Ensure preview instance exists
            if (_previewGameObject == null && skeletonData != null)
            {
                CreatePreviewInstance();
            }

            // Draw preview area
            Rect previewRect2 = GUILayoutUtility.GetRect(0, PreviewHeight, GUILayout.ExpandWidth(true));
            DrawPreview(previewRect2);

            EditorGUILayout.Space();

            // Animation dropdown
            if (_animationNames != null && _animationNames.Length > 0)
            {
                EditorGUI.BeginChangeCheck();
                _selectedAnimationIndex = EditorGUILayout.Popup("Animation", _selectedAnimationIndex, _animationNames);
                if (EditorGUI.EndChangeCheck())
                {
                    OnAnimationChanged();
                }
            }
            else
            {
                EditorGUILayout.LabelField("Animation", "No animations available");
            }

            // Skin dropdown
            if (_skinNames != null && _skinNames.Length > 0)
            {
                EditorGUI.BeginChangeCheck();
                _selectedSkinIndex = EditorGUILayout.Popup("Skin", _selectedSkinIndex, _skinNames);
                if (EditorGUI.EndChangeCheck())
                {
                    OnSkinChanged();
                }
            }
            else
            {
                EditorGUILayout.LabelField("Skin", "No skins available");
            }

            EditorGUILayout.Space();

            // Frame/Time slider
            if (_animationDuration > 0f)
            {
                EditorGUI.BeginChangeCheck();
                _currentTime = EditorGUILayout.Slider("Time", _currentTime, 0f, _animationDuration);
                if (EditorGUI.EndChangeCheck())
                {
                    OnTimeChanged();
                }

                EditorGUILayout.LabelField($"Frame: {Mathf.FloorToInt(_currentTime * FrameRate)} / {Mathf.FloorToInt(_animationDuration * FrameRate)}", EditorStyles.miniLabel);
            }
            else
            {
                EditorGUILayout.LabelField("Time", "No animation selected");
            }
            
            EditorGUI.BeginChangeCheck();
            _cameraManualOrthoSize = EditorGUILayout.Slider("Zoom", _cameraManualOrthoSize, 0.2f, 5f);
            if (EditorGUI.EndChangeCheck())
            {
                Camera cam = GetPreviewCamera();

                if (cam)
                {
                    cam.orthographicSize = _cameraManualOrthoSize;
                    _requiresRefresh = true;
                }
            }

            EditorGUILayout.Space();

            // Play/Pause button
            EditorGUILayout.BeginHorizontal();
            
            string buttonText = _isPlaying ? "Pause" : "Play";
            if (GUILayout.Button(buttonText))
            {
                _isPlaying = !_isPlaying;
                if (_isPlaying)
                {
                    _lastUpdateTime = EditorApplication.timeSinceStartup;
                    _animationLastTime = (float)EditorApplication.timeSinceStartup;
                }
            }

            if (GUILayout.Button("Reset"))
            {
                _currentTime = 0f;
                _isPlaying = false;
                OnTimeChanged();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            
            // Info
            if (_animationDuration > 0f)
            {
                EditorGUILayout.LabelField($"Duration: {_animationDuration:F2}s", EditorStyles.miniLabel);
            }
        }

        private void DrawPreview(Rect rect)
        {
            // Always handle interaction
            HandlePreviewInteraction(rect);
            
            if (UnityEngine.Event.current.type == EventType.Repaint)
            {
                // Always try to render if we have a valid preview instance
                bool shouldRender = _requiresRefresh || _previewTexture == null || _isPlaying;
                
                // Also render if we have a valid skeleton but no texture yet
                if (!shouldRender && _previewSkeletonAnimation != null && _previewSkeletonAnimation.valid && _previewTexture == null)
                {
                    shouldRender = true;
                }
                
                if (shouldRender)
                {
                    RenderPreview(rect);
                }
                
                if (_previewTexture != null)
                {
                    GUI.DrawTexture(rect, _previewTexture, ScaleMode.StretchToFill, false);
                }
                else
                {
                    // Draw placeholder if no texture
                    EditorGUI.DrawRect(rect, new Color(0.3f, 0.3f, 0.3f, 1f));
                    
                    // Show debug info
                    if (_previewGameObject == null)
                    {
                        EditorGUI.LabelField(rect, "No preview instance", EditorStyles.centeredGreyMiniLabel);
                    }
                    else if (_previewSkeletonAnimation != null && !_previewSkeletonAnimation.valid)
                    {
                        EditorGUI.LabelField(rect, "Skeleton not valid", EditorStyles.centeredGreyMiniLabel);
                    }
                }
            }
        }

        private void HandlePreviewInteraction(Rect rect)
        {
            var current = UnityEngine.Event.current;
            
            // Handle scroll wheel for zoom
            if (rect.Contains(current.mousePosition) && current.type == EventType.ScrollWheel)
            {
                _cameraOrthoSize += current.delta.y * 0.06f;
                _cameraOrthoSize = Mathf.Max(0.01f, _cameraOrthoSize);
                _requiresRefresh = true;
                current.Use();
                Repaint();
            }
        }

        private void RenderPreview(Rect rect)
        {
            if (_previewRenderUtility == null || _previewGameObject == null || _previewSkeletonAnimation == null)
            {
                EditorGUI.DrawRect(rect, new Color(0.3f, 0.3f, 0.3f, 1f));
                return;
            }

            Camera cam = GetPreviewCamera();
            if (cam == null)
            {
                EditorGUI.DrawRect(rect, new Color(0.3f, 0.3f, 0.3f, 1f));
                return;
            }

            // Update camera position before BeginPreview
            AdjustCamera(true);
            // cam.orthographicSize = _cameraOrthoSize;
            // cam.transform.position = _cameraPosition;
            cam.transform.rotation = Quaternion.identity;

            // Render - BeginPreview sets up the camera textures
            try
            {
                _previewRenderUtility.BeginPreview(rect, GUIStyle.none);
                
                // Now check if camera textures are ready (after BeginPreview)
                if (cam.activeTexture == null || cam.targetTexture == null)
                {
                    _previewRenderUtility.EndPreview();
                    EditorGUI.DrawRect(rect, new Color(0.3f, 0.3f, 0.3f, 1f));
                    return;
                }
                
                DoRenderPreview();
                _previewTexture = _previewRenderUtility.EndPreview();
                _requiresRefresh = false;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Preview render error: {e.Message}\n{e.StackTrace}");
                EditorGUI.DrawRect(rect, new Color(0.3f, 0.3f, 0.3f, 1f));
            }
        }

        private void DoRenderPreview()
        {
            if (_previewGameObject == null || _previewSkeletonAnimation == null || !_previewSkeletonAnimation.valid)
                return;

            Renderer renderer = _previewGameObject.GetComponent<Renderer>();
            if (renderer == null)
                return;

            // Enable renderer for rendering
            renderer.enabled = true;

            // Update animation (like Spine does - inside the render method)
            if (_previewSkeletonAnimation.AnimationState != null)
            {
                var track = _previewSkeletonAnimation.AnimationState.GetCurrent(0);
                if (track != null)
                {
                    // Set the track time to match our current time control
                    track.TrackTime = _currentTime;
                    
                    if (_isPlaying && _animationDuration > 0f)
                    {
                        // When playing, update with delta time
                        float currentTime = (float)EditorApplication.timeSinceStartup;
                        float deltaTime = currentTime - _animationLastTime;
                        _animationLastTime = currentTime;
                        _previewSkeletonAnimation.Update(deltaTime);
                    }
                    else
                    {
                        // When paused/scrubbing, update without advancing time
                        _previewSkeletonAnimation.Update(0f);
                    }
                    
                    _previewSkeletonAnimation.LateUpdate();
                }
            }

            // Render the camera
            Camera cam = GetPreviewCamera();
            if (cam != null)
            {
                cam.Render();
            }

            // Disable renderer after rendering
            renderer.enabled = false;
        }

        private void AdjustCamera(bool updateSizeAndPosition)
        {
            if (_previewGameObject == null || _previewSkeletonAnimation == null || !_previewSkeletonAnimation.valid)
                return;

            Renderer renderer = _previewGameObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                // Make sure the renderer has valid bounds
                if (renderer.bounds.size.magnitude > 0.001f)
                {
                    Bounds bounds = renderer.bounds;
                    if (updateSizeAndPosition)
                    {
                        _cameraOrthoSize = Mathf.Max(bounds.size.y, bounds.size.x) * 0.6f;
                        _cameraPosition = bounds.center + new Vector3(0, 0, -10f);
                    }
                }
                else
                {
                    // Use skeleton GetBounds method if renderer bounds are not available yet
                    Skeleton skeleton = _previewSkeletonAnimation.skeleton;
                    if (skeleton != null)
                    {
                        float[] vertexBuffer = null;
                        skeleton.GetBounds(out float x, out float y, out float width, out float height, ref vertexBuffer);
                        
                        if (width > 0.001f && height > 0.001f)
                        {
                            if (updateSizeAndPosition)
                            {
                                _cameraOrthoSize = Mathf.Max(height, width) * 0.6f;
                                _cameraPosition = new Vector3(x + width * 0.5f, y + height * 0.5f, -10f);
                            }
                        }
                    }
                }
            }
        }

        private void OnUpdate()
        {
            if (_isPlaying && _animationDuration > 0f)
            {
                double currentTime = EditorApplication.timeSinceStartup;
                float deltaTime = (float)(currentTime - _lastUpdateTime);
                _lastUpdateTime = currentTime;

                _currentTime += deltaTime;
                
                if (_currentTime >= _animationDuration)
                {
                    _currentTime = 0f; // Loop
                }

                _requiresRefresh = true;
                Repaint();
            }
            else if (_requiresRefresh)
            {
                Repaint();
            }
        }

        private void InitializePreviewRenderUtility()
        {
            if (_previewRenderUtility != null)
                return;

            _previewRenderUtility = new PreviewRenderUtility(true);
            Camera cam = GetPreviewCamera();
            if (cam != null)
            {
                cam.orthographic = true;
                cam.cullingMask = PreviewCameraCullingMask;
                cam.nearClipPlane = 0.01f;
                cam.farClipPlane = 1000f;
                cam.orthographicSize = _cameraOrthoSize;
                cam.transform.position = _cameraPosition;
            }
        }

        private Camera GetPreviewCamera()
        {
            if (_previewRenderUtility == null)
                return null;

#if UNITY_2017_1_OR_NEWER
            return _previewRenderUtility.camera;
#else
            return _previewRenderUtility.m_Camera;
#endif
        }

        private SkeletonData GetSkeletonData()
        {
            if (_skeletonDataAsset != null)
            {
                return _skeletonDataAsset.GetSkeletonData(true);
            }

            return null;
        }

        private void CreatePreviewInstance()
        {
            if (_skeletonDataAsset == null)
                return;

            SkeletonData skeletonData = _skeletonDataAsset.GetSkeletonData(false);
            if (skeletonData == null)
                return;

            DestroyPreviewInstance();

            try
            {
                // Use Spine's EditorInstantiation to create the skeleton properly
                string skinName = _skinNames != null && _selectedSkinIndex >= 0 && _selectedSkinIndex < _skinNames.Length
                    ? _skinNames[_selectedSkinIndex]
                    : "";

                var skeletonComponent = EditorInstantiation.InstantiateSkeletonAnimation(
                    _skeletonDataAsset, 
                    skinName, 
                    destroyInvalid: false, 
                    useObjectFactory: false
                );

                if (skeletonComponent == null)
                {
                    Debug.LogError("Failed to create skeleton preview instance");
                    return;
                }

                _previewGameObject = skeletonComponent.gameObject;
                _previewGameObject.hideFlags = HideFlags.HideAndDontSave;
                _previewGameObject.layer = PreviewLayer;
                _previewSkeletonAnimation = skeletonComponent;

                // Ensure skeleton is initialized
                if (!_previewSkeletonAnimation.valid)
                {
                    _previewSkeletonAnimation.Initialize(false);
                }

                // Update once to ensure mesh is generated
                _previewSkeletonAnimation.LateUpdate();

                Renderer renderer = _previewGameObject.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.enabled = false;
                }

                // Add to preview utility - this ensures the GameObject is in the preview scene
                // This is critical for the preview to work!
                if (_previewRenderUtility != null)
                {
#if SPINE_UNITY_2018_PREVIEW_API
                    _previewRenderUtility.AddSingleGO(_previewGameObject);
                    Debug.Log($"Added GameObject to preview utility. Layer: {_previewGameObject.layer}, Valid: {_previewSkeletonAnimation.valid}");
#else
                    Debug.LogWarning("SPINE_UNITY_2018_PREVIEW_API not defined - preview may not work on this Unity version");
#endif
                }
                else
                {
                    Debug.LogError("PreviewRenderUtility is null when trying to add GameObject!");
                }

                // Force initial camera adjustment and refresh
                EditorApplication.delayCall += () =>
                {
                    if (_previewGameObject != null && _previewSkeletonAnimation != null && _previewSkeletonAnimation.valid)
                    {
                        AdjustCamera(true);
                        _requiresRefresh = true;
                        Repaint();
                    }
                };
                
                _requiresRefresh = true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error creating preview instance: {e.Message}\n{e.StackTrace}");
                DestroyPreviewInstance();
            }
        }

        private void DestroyPreviewInstance()
        {
            if (_previewGameObject != null)
            {
                DestroyImmediate(_previewGameObject);
                _previewGameObject = null;
                _previewSkeletonAnimation = null;
            }
        }

        private void OnSkeletonDataAssetChanged()
        {
            Debug.Log("SkeletonDataAsset changed, refreshing data...");
            RefreshData();
        }

        private void RefreshData()
        {
            SkeletonData skeletonData = GetSkeletonData();
            
            if (skeletonData == null)
            {
                _animationNames = null;
                _skinNames = null;
                DestroyPreviewInstance();
                _requiresRefresh = true;
                return;
            }

            // Get animations
            var animations = skeletonData.Animations;
            _animationNames = new string[animations.Count];
            for (int i = 0; i < animations.Count; i++)
            {
                _animationNames[i] = animations.Items[i].Name;
            }

            // Get skins
            var skins = skeletonData.Skins;
            _skinNames = new string[skins.Count];
            for (int i = 0; i < skins.Count; i++)
            {
                _skinNames[i] = skins.Items[i].Name;
            }

            // Reset selections
            _selectedAnimationIndex = 0;
            _selectedSkinIndex = 0;
            _currentTime = 0f;
            _animationDuration = 0f;
            _isPlaying = false;

            Debug.Log($"RefreshData: Found {_animationNames?.Length ?? 0} animations, {_skinNames?.Length ?? 0} skins");
            
            CreatePreviewInstance();
            
            // Wait a frame for initialization, then set animation and skin
            EditorApplication.delayCall += () =>
            {
                if (_previewSkeletonAnimation != null && _previewSkeletonAnimation.valid)
                {
                    OnAnimationChanged();
                    OnSkinChanged();
                }
            };
        }

        private void OnAnimationChanged()
        {
            SkeletonData skeletonData = GetSkeletonData();
            if (skeletonData == null || _animationNames == null || _selectedAnimationIndex < 0 || _selectedAnimationIndex >= _animationNames.Length)
            {
                _animationDuration = 0f;
                return;
            }

            string animationName = _animationNames[_selectedAnimationIndex];
            var animation = skeletonData.FindAnimation(animationName);
            
            if (animation != null)
            {
                _animationDuration = animation.Duration;
                _currentTime = 0f;
                
                if (_previewSkeletonAnimation != null && _previewSkeletonAnimation.valid && _previewSkeletonAnimation.AnimationState != null)
                {
                    _previewSkeletonAnimation.AnimationState.SetAnimation(0, animation, true);
                    var track = _previewSkeletonAnimation.AnimationState.GetCurrent(0);
                    if (track != null)
                    {
                        track.TrackTime = _currentTime;
                    }
                    _previewSkeletonAnimation.Update(0f);
                    _previewSkeletonAnimation.LateUpdate();
                }
            }
            else
            {
                _animationDuration = 0f;
            }

            _requiresRefresh = true;
        }

        private void OnSkinChanged()
        {
            SkeletonData skeletonData = GetSkeletonData();
            if (skeletonData == null || _skinNames == null || _selectedSkinIndex < 0 || _selectedSkinIndex >= _skinNames.Length)
                return;

            string skinName = _skinNames[_selectedSkinIndex];
            Skin skin = skeletonData.FindSkin(skinName);
            
            if (skin != null && _previewSkeletonAnimation != null && _previewSkeletonAnimation.valid && _previewSkeletonAnimation.skeleton != null)
            {
                _previewSkeletonAnimation.skeleton.SetSkin(skin);
                _previewSkeletonAnimation.skeleton.SetSlotsToSetupPose();
                _previewSkeletonAnimation.LateUpdate();
                _requiresRefresh = true;
            }
        }

        private void OnTimeChanged()
        {
            if (_previewSkeletonAnimation != null && _previewSkeletonAnimation.valid && _previewSkeletonAnimation.AnimationState != null)
            {
                var track = _previewSkeletonAnimation.AnimationState.GetCurrent(0);
                if (track != null)
                {
                    track.TrackTime = _currentTime;
                    _previewSkeletonAnimation.Update(0f);
                    _previewSkeletonAnimation.LateUpdate();
                }
            }

            _requiresRefresh = true;
        }

        private void CleanupPreview()
        {
            DestroyPreviewInstance();
            
            if (_previewRenderUtility != null)
            {
                _previewRenderUtility.Cleanup();
                _previewRenderUtility = null;
            }

            _previewTexture = null;
        }
    }
}

