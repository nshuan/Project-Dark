using System.Collections.Generic;
using System.Linq;
using InGame.Upgrade;
using UnityEditor;
using UnityEngine;

namespace DefaultNamespace.Editor.UpgradeTreeEditor
{
    public class UpgradeTreeWindow : EditorWindow
    {
// #if UNITY_EDITOR
        private Vector2 canvasScrollPos;
        private Rect canvasRect = new Rect(0, 0, 1000, 800);
        private float rightPanelWidth = 250;

        private List<UpgradeNodeWrapper> nodes = new List<UpgradeNodeWrapper>();
        private UpgradeNodeWrapper selectedNode;
        private UnityEditor.Editor nodeInspector;
        private Vector2 inspectorScroll;

        private UpgradeTreeConfig treeAsset;
        private static List<System.Type> nodeTypes;
        
        private bool isDraggingLink = false;
        private Vector2 linkDragStartPos;
        private UpgradeNodeWrapper linkSourceNode = null;

        // Splitter, for resize panel
        private bool resizing = false;
        private Rect splitterRect;
        private const float splitterWidth = 5f;
        
        [MenuItem("Tools/Upgrade Tree Editor")]
        public static void ShowWindow()
        {
            GetWindow<UpgradeTreeWindow>("Upgrade Tree");
            
            nodeTypes = new List<System.Type>();
            foreach (var type in TypeCache.GetTypesDerivedFrom<UpgradeNodeConfig>())
            {
                if (!type.IsAbstract && type.IsSubclassOf(typeof(UpgradeNodeConfig)))
                {
                    nodeTypes.Add(type);
                }
            }
        }

        private void OnGUI()
        {
            Event e = Event.current;

            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Delete)
            {
                TryDeleteSelectedNode();
                e.Use(); // consume the event
            }
            
            float canvasWidth = position.width - rightPanelWidth - splitterWidth;
            canvasScrollPos = GUI.BeginScrollView(new Rect(0, 0, canvasWidth, position.height), canvasScrollPos, canvasRect);
            DrawCanvas(); // Your canvas code here
            GUI.EndScrollView();

            // Splitter handle between canvas and right panel
            splitterRect = new Rect(canvasWidth, 0, splitterWidth, position.height);
            EditorGUIUtility.AddCursorRect(splitterRect, MouseCursor.ResizeHorizontal);
            GUI.Box(splitterRect, GUIContent.none, EditorStyles.toolbarButton);

            // Handle drag logic
            HandleResizeEvents(splitterRect);

            // Right panel
            GUILayout.BeginArea(new Rect(canvasWidth + splitterWidth, 0, rightPanelWidth, position.height), EditorStyles.helpBox);
            DrawRightPanel(); // Your right panel code here
            GUILayout.EndArea();

            Repaint();
        }
        
        private void LoadTree(UpgradeTreeConfig loadedTree)
        {
            treeAsset = loadedTree;
            nodes.Clear();

            if (treeAsset.upgradeNodes != null)
            {
                foreach (var node in treeAsset.upgradeNodes)
                {
                    // Avoid duplicates
                    if (node == null || nodes.Exists(n => n.nodeConfig == node)) continue;

                    nodes.Add(new UpgradeNodeWrapper(node, new Rect(50, 50 + nodes.Count * 100, 150, 80)));
                }
            }
        }

        private void HandleResizeEvents(Rect splitterRect)
        {
            Event e = Event.current;

            switch (e.type)
            {
                case EventType.MouseDown:
                    if (splitterRect.Contains(e.mousePosition))
                    {
                        resizing = true;
                        e.Use();
                    }
                    break;

                case EventType.MouseDrag:
                    if (resizing)
                    {
                        rightPanelWidth = Mathf.Clamp(position.width - e.mousePosition.x, 100, position.width - 100);
                        Repaint();
                        e.Use();
                    }
                    break;

                case EventType.MouseUp:
                    if (resizing)
                    {
                        resizing = false;
                        e.Use();
                    }
                    break;
            }
        }
        
        private void DrawCanvas()
        {
            GUI.Box(canvasRect, GUIContent.none);

            BeginWindows();
            // for (int i = 0; i < nodes.Count; i++)
            // {
            //     var node = nodes[i];
            //     node.rect = GUI.Window(i, node.rect, id =>
            //     {
            //         if (GUILayout.Button("Select"))
            //             selectedNode = node;
            //
            //         GUILayout.Label(node.nodeConfig.nodeName ?? $"Node {i}");
            //         GUI.DragWindow();
            //     }, $"Node {i}");
            // }

            for (int i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];
                node.rect = GUI.Window(i, node.rect, id => DrawNodeWindow(id, node), node.nodeConfig.name);
            }
            
            // Draw connections between nodes
            Handles.BeginGUI();
            foreach (var node in nodes)
            {
                if (node.nodeConfig.preRequire == null) continue;

                foreach (var dependency in node.nodeConfig.preRequire)
                {
                    var fromNode = nodes.Find(n => n.nodeConfig == dependency);
                    if (fromNode == null) continue;

                    Vector3 startPos = new Vector3(fromNode.rect.xMax, fromNode.rect.center.y, 0);
                    Vector3 endPos = new Vector3(node.rect.xMin, node.rect.center.y, 0);

                    Vector3 startTangent = startPos + Vector3.right * 50;
                    Vector3 endTangent = endPos + Vector3.left * 50;

                    Handles.DrawBezier(startPos, endPos, startTangent, endTangent, Color.cyan, null, 3);
                }
            }
            Handles.EndGUI();
            EndWindows();
            
            Handles.BeginGUI();

            foreach (var node in nodes)
            {
                // Position the socket on the right-middle of the node
                Vector2 center = new Vector2(node.rect.xMax + 5, node.rect.center.y);
                node.socketCenter = center;

                Handles.color = Color.green;
                Handles.DrawSolidDisc(center, Vector3.forward, 5f);

                // Detect mouse down
                Rect hitbox = new Rect(center.x - 6, center.y - 6, 12, 12);
                if (Event.current.type == EventType.MouseDown && hitbox.Contains(Event.current.mousePosition))
                {
                    isDraggingLink = true;
                    linkDragStartPos = center;
                    linkSourceNode = node;
                    Event.current.Use();
                }
            }

            Handles.EndGUI();
            
            // ---- Draw existing connections
            foreach (var node in nodes)
            {
                if (node.nodeConfig.preRequire == null) continue;

                foreach (var dep in node.nodeConfig.preRequire)
                {
                    var from = nodes.Find(n => n.nodeConfig == dep);
                    if (from == null) continue;

                    Vector3 start = new Vector3(from.rect.xMax, from.rect.center.y);
                    Vector3 end = new Vector3(node.rect.xMin, node.rect.center.y);
                    Handles.DrawBezier(start, end, start + Vector3.right * 50, end + Vector3.left * 50, Color.cyan, null, 2);
                }
            }
            
            // ---- Draw drag line
            if (isDraggingLink && linkSourceNode != null)
            {
                Handles.DrawBezier(
                    linkDragStartPos,
                    Event.current.mousePosition,
                    linkDragStartPos + Vector2.right * 50,
                    Event.current.mousePosition + Vector2.left * 50,
                    Color.yellow, null, 2
                );

                Repaint();
            }
            Handles.EndGUI();

            if (isDraggingLink && Event.current.type == EventType.MouseUp)
            {
                isDraggingLink = false;

                foreach (var targetNode in nodes)
                {
                    if (targetNode == linkSourceNode) continue;
                    if (linkSourceNode.nodeConfig.preRequire.Contains(targetNode.nodeConfig)) continue;

                    if (targetNode.rect.Contains(Event.current.mousePosition))
                    {
                        // Add dependency
                        var deps = new List<UpgradeNodeConfig>(targetNode.nodeConfig.preRequire ?? new UpgradeNodeConfig[0]);
                        if (!deps.Contains(linkSourceNode.nodeConfig))
                        {
                            deps.Add(linkSourceNode.nodeConfig);
                            targetNode.nodeConfig.preRequire = deps.ToArray();
                            EditorUtility.SetDirty(targetNode.nodeConfig);
                        }
                        
                        linkSourceNode = null;
                        Event.current.Use();
                        return;
                    }
                }
                
                // 🆕 Not dropped on any node → show node type menu
                Vector2 dropPos = Event.current.mousePosition;
                ShowAddNodeMenuAt(dropPos, linkSourceNode);
                linkSourceNode = null;
                Event.current.Use();
            }
        }

        private void DrawRightPanel()
        {
            GUILayout.Label("Upgrade Tree Editor", EditorStyles.boldLabel);

            if (GUILayout.Button("Create Tree Asset"))
            {
                string path = EditorUtility.SaveFilePanelInProject("Create Upgrade Tree", "NewUpgradeTree", "asset", "Create a new upgrade tree asset");
                if (!string.IsNullOrEmpty(path))
                {
                    treeAsset = CreateInstance<UpgradeTreeConfig>();
                    AssetDatabase.CreateAsset(treeAsset, path);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    nodes.Clear();
                }
            }
            
            if (GUILayout.Button("Open Tree Asset"))
            {
                string path = EditorUtility.OpenFilePanel("Open Upgrade Tree", "Assets", "asset");
                if (!string.IsNullOrEmpty(path))
                {
                    path = FileUtil.GetProjectRelativePath(path);
                    var loadedTree = AssetDatabase.LoadAssetAtPath<UpgradeTreeConfig>(path);
                    if (loadedTree != null)
                    {
                        LoadTree(loadedTree);
                    }
                    else
                    {
                        Debug.LogError("Selected file is not a valid UpgradeTreeConfig.");
                    }
                }
            }

            if (GUILayout.Button("Add Node"))
            {
                if (treeAsset == null)
                {
                    Debug.LogWarning("Create or open a tree asset first.");
                    return;
                }

                GenericMenu menu = new GenericMenu();
                foreach (var type in nodeTypes)
                {
                    menu.AddItem(new GUIContent(type.Name), false, () =>
                    {
                        UpgradeNodeConfig newNode = (UpgradeNodeConfig)ScriptableObject.CreateInstance(type);
                        newNode.nodeName = type.Name;
                        newNode.levelInTree = 0;

                        string nodePath = EditorUtility.SaveFilePanelInProject("Save Node", newNode.nodeName, "asset", "Save new node asset");
                        if (!string.IsNullOrEmpty(nodePath))
                        {
                            AssetDatabase.CreateAsset(newNode, nodePath);
                            AssetDatabase.SaveAssets();
                            AssetDatabase.Refresh();

                            // Add to tree only if not already present
                            if (!treeAsset.upgradeNodes.Contains(newNode))
                                treeAsset.upgradeNodes.Add(newNode);

                            // Only add to canvas once
                            if (!nodes.Exists(n => n.nodeConfig == newNode))
                                nodes.Add(new UpgradeNodeWrapper(newNode, new Rect(50 + nodes.Count * 20, 50, 150, 80)));
                        }
                    });
                }
                menu.ShowAsContext();
            }

            if (GUILayout.Button("Save Tree Asset"))
            {
                if (treeAsset != null)
                {
                    treeAsset.upgradeNodes = new List<UpgradeNodeConfig>();
                    foreach (var wrapper in nodes)
                    {
                        treeAsset.upgradeNodes.Add(wrapper.nodeConfig);
                    }

                    string path = EditorUtility.SaveFilePanelInProject("Save Upgrade Tree", "UpgradeTree", "asset", "Save your upgrade tree");
                    if (!string.IsNullOrEmpty(path))
                    {
                        AssetDatabase.CreateAsset(treeAsset, path);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                        Debug.Log("Tree saved at: " + path);
                    }
                }
            }

            if (selectedNode != null)
            {
                GUILayout.Space(10);
                GUILayout.Label("Selected Node", EditorStyles.boldLabel);
                inspectorScroll = EditorGUILayout.BeginScrollView(inspectorScroll);

                // selectedNode.nodeConfig.nodeName = EditorGUILayout.TextField("Node Name", selectedNode.nodeConfig.nodeName);
                // selectedNode.nodeConfig.description = EditorGUILayout.TextField("Description", selectedNode.nodeConfig.description);
                // selectedNode.nodeConfig.levelInTree = EditorGUILayout.IntField("Level In Tree", selectedNode.nodeConfig.levelInTree);
                // selectedNode.nodeConfig.costType = EditorGUILayout.IntField("Cost Type", selectedNode.nodeConfig.costType);
                // selectedNode.nodeConfig.costValue = EditorGUILayout.IntField("Cost Value", selectedNode.nodeConfig.costValue);
                
                var nodeEditor = UnityEditor.Editor.CreateEditor(selectedNode.nodeConfig);
                if (nodeEditor != null)
                {
                    if (nodeInspector != null) DestroyImmediate(nodeInspector);
                    nodeInspector = UnityEditor.Editor.CreateEditor(selectedNode.nodeConfig);
                    EditorGUI.BeginChangeCheck();
                    nodeInspector.OnInspectorGUI();
                    if (EditorGUI.EndChangeCheck())
                    {
                        EditorUtility.SetDirty(selectedNode.nodeConfig);
                    }
                }
                EditorGUILayout.EndScrollView();
                
                // // ... existing fields
                // GUILayout.Space(10);
                // GUILayout.Label("Dependencies");
                // foreach (var node in nodes)
                // {
                //     if (node == selectedNode) continue;
                //     bool hasDep = selectedNode.nodeConfig.preRequire != null &&
                //                   System.Array.Exists(selectedNode.nodeConfig.preRequire, d => d == node.nodeConfig);
                //
                //     bool shouldHave = GUILayout.Toggle(hasDep, $"Depends on {node.nodeConfig.nodeName}");
                //     if (shouldHave && !hasDep)
                //     {
                //         var deps = new List<UpgradeNodeConfig>(selectedNode.nodeConfig.preRequire ?? new UpgradeNodeConfig[0]);
                //         deps.Add(node.nodeConfig);
                //         selectedNode.nodeConfig.preRequire = deps.ToArray();
                //     }
                //     else if (!shouldHave && hasDep)
                //     {
                //         var deps = new List<UpgradeNodeConfig>(selectedNode.nodeConfig.preRequire);
                //         deps.Remove(node.nodeConfig);
                //         selectedNode.nodeConfig.preRequire = deps.ToArray();
                //     }
                // }
            }
        }
        
        private void DrawNodeWindow(int id, UpgradeNodeWrapper node)
        {
            GUILayout.BeginVertical();

            GUILayout.Label(node.nodeConfig.nodeName ?? $"Node {id}", EditorStyles.boldLabel);

            if (GUILayout.Button("Select"))
            {
                selectedNode = node;

                if (nodeInspector != null) DestroyImmediate(nodeInspector);
                nodeInspector = UnityEditor.Editor.CreateEditor(node.nodeConfig);
            }

            GUILayout.EndVertical();

            GUI.DragWindow();
        }
        
        private void TryDeleteSelectedNode()
        {
            if (selectedNode == null) return;
            if (!EditorUtility.DisplayDialog("Delete Node", $"Are you sure you want to delete '{selectedNode.nodeConfig.nodeName}'?", "Yes", "Cancel"))
                return;

            // Confirm removal from tree
            if (treeAsset.upgradeNodes.Contains(selectedNode.nodeConfig))
            {
                treeAsset.upgradeNodes.Remove(selectedNode.nodeConfig);
                EditorUtility.SetDirty(treeAsset);
            }

            // Remove from other node's preRequires
            foreach (var node in nodes)
            {
                if (node.nodeConfig.preRequire == null) continue;

                var deps = new List<UpgradeNodeConfig>(node.nodeConfig.preRequire);
                if (deps.Remove(selectedNode.nodeConfig))
                {
                    node.nodeConfig.preRequire = deps.ToArray();
                    EditorUtility.SetDirty(node.nodeConfig);
                }
            }

            // Remove from visual list
            nodes.Remove(selectedNode);

            // Delete asset from disk
            string path = AssetDatabase.GetAssetPath(selectedNode.nodeConfig);
            if (!string.IsNullOrEmpty(path))
            {
                bool fileDeleted = AssetDatabase.DeleteAsset(path);
                if (!fileDeleted)
                {
                    Debug.LogWarning($"Failed to delete asset at {path}");
                }
            }
            
            selectedNode = null;
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        
        private void ShowAddNodeMenuAt(Vector2 position, UpgradeNodeWrapper prereqNode)
        {
            GenericMenu menu = new GenericMenu();
            foreach (var type in nodeTypes)
            {
                menu.AddItem(new GUIContent(type.Name), false, () =>
                {
                    var newNode = (UpgradeNodeConfig)ScriptableObject.CreateInstance(type);
                    newNode.nodeName = type.Name;
                    newNode.levelInTree = prereqNode?.nodeConfig.levelInTree + 1 ?? 0;

                    string nodePath = EditorUtility.SaveFilePanelInProject("Save Node", newNode.nodeName, "asset", "Save new node asset");
                    if (!string.IsNullOrEmpty(nodePath))
                    {
                        AssetDatabase.CreateAsset(newNode, nodePath);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        // Set preRequire to dragged-from node
                        if (prereqNode != null)
                            newNode.preRequire = new[] { prereqNode.nodeConfig };

                        var newWrapper = new UpgradeNodeWrapper(newNode, new Rect(position.x, position.y, 150, 80));
                        nodes.Add(newWrapper);
                        treeAsset.upgradeNodes.Add(newNode);
                        EditorUtility.SetDirty(treeAsset);
                    }
                });
            }
            menu.ShowAsContext();
        }

        // Helper wrapper for UI state
        private class UpgradeNodeWrapper
        {
            public Rect rect;
            public UpgradeNodeConfig nodeConfig;

            // Cached socket center (for drawing)
            public Vector2 socketCenter;

            public UpgradeNodeWrapper(UpgradeNodeConfig config, Rect r)
            {
                rect = r;
                nodeConfig = config;
                socketCenter = Vector2.zero;
            }
        }
// #endif
    }
}