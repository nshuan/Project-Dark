#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using System;
using System.Reflection;
using UnityEngine.UIElements;

namespace Dark.Tools.QuickSceneLoadEditor.Editor
{
    [InitializeOnLoad]
    public static class EditorButtonHome
    {
        static EditorButtonHome()
        {
            ToolbarCallback.OnToolbarGUI += OnToolbarGUI;
        }

        private static void OnToolbarGUI()
        { 
            GUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Home", GUILayout.Width(60)))
            {
                if (!EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                    {
                        EditorSceneManager.OpenScene($"Assets/Dark/Scenes/Release/Home.unity");
                    }
                }

            }
            
            if (GUILayout.Button("InGame", GUILayout.Width(60)))
            {
                if (!EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                    {
                        EditorSceneManager.OpenScene($"Assets/Dark/Scenes/Release/BaseLevel.unity"); 
                    }
                }
            }
            
            if (GUILayout.Button("Upgrade", GUILayout.Width(60)))
            {
                if (!EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                    {
                        EditorSceneManager.OpenScene($"Assets/Dark/Scenes/Release/Upgrade.unity"); 
                    }
                }
            }
            
            if (GUILayout.Button("SaveSlot", GUILayout.Width(60)))
            {
                if (!EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                    {
                        EditorSceneManager.OpenScene($"Assets/Dark/Scenes/Release/SaveSlot.unity"); 
                    }
                }
            }
            
            GUILayout.EndHorizontal();
        }
    }

    // Toolbar reflection hook
    [InitializeOnLoad]
    public static class ToolbarCallback
    {
        static Type toolbarType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.Toolbar");
        static object currentToolbar;
        static UnityEngine.UIElements.VisualElement container;

        static ToolbarCallback()
        {
            EditorApplication.update += OnUpdate;
        }

        static void OnUpdate()
        {
            if (currentToolbar != null) return;

            var toolbars = Resources.FindObjectsOfTypeAll(toolbarType);
            if (toolbars.Length == 0) return;

            currentToolbar = toolbars[0];

            var root = currentToolbar.GetType().GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.GetValue(currentToolbar);
            var visualTree = root as UnityEngine.UIElements.VisualElement;

            if (visualTree == null) return;

            visualTree.RegisterCallback<UnityEngine.UIElements.GeometryChangedEvent>(evt =>
            {
                ToolbarGUI();
            });
            
            ToolbarGUI();
        }

        public static event Action OnToolbarGUI = delegate { };

        static void ToolbarGUI()
        {
            // Prevent duplicate containers
            if (container is { parent: not null }) return;
            
            container = new UnityEngine.UIElements.VisualElement();
            container.style.flexDirection = UnityEngine.UIElements.FlexDirection.Row;
            container.Add(new UnityEngine.UIElements.IMGUIContainer(() =>
            {
                OnToolbarGUI.Invoke();
            }));

            var root = currentToolbar.GetType().GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.GetValue(currentToolbar);
            var visualTree = root as UnityEngine.UIElements.VisualElement;
            visualTree.Q("ToolbarZoneRightAlign").Add(container);
        }
    }
}

#endif
