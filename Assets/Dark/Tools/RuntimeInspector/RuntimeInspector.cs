// RuntimeInspector.cs (generic version)
// This version can inspect any serializable class or object, not only GameObjects.
// You can assign 'selected' in the Inspector or by code to any object.
// If it's a UnityEngine.Object (like a GameObject or Component), it will still show its components.

using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class RuntimeInspector : MonoBehaviour
{
    [SerializeReference]
    public object selected;

    public KeyCode toggleKey = KeyCode.I;
    bool visible = true;
    bool picking = false;
    Vector2 scroll;
    Rect toggleRect = new Rect(8, 8, 80, 28);

    void Update()
    {
        if (Input.GetKeyDown(toggleKey)) visible = !visible;

        if (picking && Input.GetMouseButtonDown(0))
        {
            var cam = Camera.main;
            if (cam != null)
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    selected = hit.collider.gameObject;
                }
            }
            picking = false;
        }
    }

    void OnGUI()
    {
        // --- Toggle button ---
        GUI.Box(toggleRect, "");
        if (GUI.Button(toggleRect, visible ? "Hide UI" : "Show UI"))
            visible = !visible;

        if (!visible) return;

        var pad = 8;
        var w = 420;
        var h = Screen.height - pad * 2;
        GUILayout.BeginArea(new Rect(pad, toggleRect.yMax + 8, w, h - 40), GUI.skin.box);
        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Pick (click object)", GUILayout.Width(160))) picking = true;
        if (GUILayout.Button("Clear", GUILayout.Width(80))) selected = null;
        GUILayout.EndHorizontal();

        GUILayout.Space(6);

        GUILayout.Label("Selected:");
        if (selected == null)
        {
            GUILayout.Label("<none>");
            GUILayout.EndVertical();
            GUILayout.EndArea();
            return;
        }

        GUILayout.Label(selected.GetType().Name, EditorStylesBoldLike());
        GUILayout.Space(6);

        scroll = GUILayout.BeginScrollView(scroll);

        if (selected is GameObject go)
        {
            GUILayout.Label($"GameObject: {go.name}", EditorStylesBoldLike());
            var comps = go.GetComponents<Component>();
            foreach (var comp in comps)
            {
                if (comp == null) continue;
                var type = comp.GetType();
                bool fold = FoldoutHeader(type.Name);
                if (!fold) continue;
                GUILayout.BeginVertical(GUI.skin.box);
                DrawObjectFields(comp);
                GUILayout.EndVertical();
            }
        }
        else if (selected is Component c)
        {
            GUILayout.Label($"Component: {c.GetType().Name}", EditorStylesBoldLike());
            DrawObjectFields(c);
        }
        else
        {
            GUILayout.Label($"Object: {selected.GetType().FullName}", EditorStylesBoldLike());
            DrawObjectFields(selected);
        }

        GUILayout.EndScrollView();
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    GUIStyle labelBold;
    GUIStyle EditorStylesBoldLike()
    {
        if (labelBold == null) labelBold = new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold };
        return labelBold;
    }

    GUIStyle headerStyle;
    bool FoldoutHeader(string title)
    {
        if (headerStyle == null)
            headerStyle = new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleLeft };

        GUILayout.BeginHorizontal();
        bool show = EditorPrefsGetBool("fold_" + title, true);
        if (GUILayout.Button((show ? "▾ " : "▸ ") + title, headerStyle))
        {
            show = !show;
            EditorPrefsSetBool("fold_" + title, show);
        }
        GUILayout.EndHorizontal();
        return show;
    }

    void DrawObjectFields(object obj)
    {
        if (obj == null) return;

        var type = obj.GetType();
        var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(f => !f.IsDefined(typeof(NonSerializedAttribute), true))
            .ToArray();

        foreach (var f in fields)
        {
            bool isSerialized = f.IsPublic || f.IsDefined(typeof(SerializeField), true);
            GUILayout.BeginHorizontal();
            GUILayout.Label(f.Name, GUILayout.Width(140));
            object val = null;
            try { val = f.GetValue(obj); } catch { }
            bool editable = isSerialized && !f.IsInitOnly;
            DrawValueEditor(obj, f, val, editable);
            GUILayout.EndHorizontal();
        }
    }

    void DrawValueEditor(object owner, MemberInfo member, object value, bool editable)
    {
        Type t = null;
        if (member is FieldInfo f) t = f.FieldType;
        else if (member is PropertyInfo p) t = p.PropertyType;
        else return;

        if (t == typeof(float))
        {
            float v = value != null ? (float)value : 0f;
            string s = GUILayout.TextField(v.ToString("G4"), GUILayout.Width(80));
            if (editable && float.TryParse(s, out float newV) && Math.Abs(newV - v) > 0.0001f)
                SetMemberValue(owner, member, newV);
        }
        else if (t == typeof(int))
        {
            int v = value != null ? (int)value : 0;
            string s = GUILayout.TextField(v.ToString(), GUILayout.Width(80));
            if (editable && int.TryParse(s, out int newV) && newV != v)
                SetMemberValue(owner, member, newV);
        }
        else if (t == typeof(bool))
        {
            bool v = value != null ? (bool)value : false;
            bool newV = GUILayout.Toggle(v, "");
            if (editable && newV != v)
                SetMemberValue(owner, member, newV);
        }
        else if (t == typeof(string))
        {
            string v = value != null ? (string)value : "";
            string newV = GUILayout.TextField(v);
            if (editable && newV != v)
                SetMemberValue(owner, member, newV);
        }
        else if (t.IsClass && value != null && !(value is UnityEngine.Object))
        {
            GUILayout.EndHorizontal();
            GUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label(t.Name, EditorStylesBoldLike());
            DrawObjectFields(value);
            GUILayout.EndVertical();
            GUILayout.BeginHorizontal();
        }
        else
        {
            GUILayout.Label(value != null ? value.ToString() : "null");
        }
    }

    void SetMemberValue(object owner, MemberInfo member, object newValue)
    {
        try
        {
            if (member is FieldInfo f) f.SetValue(owner, newValue);
            else if (member is PropertyInfo p) p.SetValue(owner, newValue);
        }
        catch (Exception ex) { Debug.LogException(ex); }
    }

    bool EditorPrefsGetBool(string key, bool def)
    {
        return PlayerPrefs.GetInt(key.GetHashCode().ToString(), def ? 1 : 0) == 1;
    }
    void EditorPrefsSetBool(string key, bool val)
    {
        PlayerPrefs.SetInt(key.GetHashCode().ToString(), val ? 1 : 0);
    }
}
