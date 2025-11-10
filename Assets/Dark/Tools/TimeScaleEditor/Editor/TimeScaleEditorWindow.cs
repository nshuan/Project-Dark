using UnityEditor;
using UnityEngine;

namespace Dark.Tools.TimeScaleEditor
{
    public class TimeScaleEditorWindow : EditorWindow
    {
        private const float MinTimeScale = 0f;
        private const float MaxTimeScale = 10f;

        private float _timeScale = 1f;

        [MenuItem("Dark/Tools/Time Scale Editor")]
        public static void ShowWindow()
        {
            var window = GetWindow<TimeScaleEditorWindow>("Time Scale");
            window.minSize = new Vector2(260f, 110f);
            window.RefreshFromUnityTime();
        }

        private void OnEnable()
        {
            RefreshFromUnityTime();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Controls", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            var sliderValue = EditorGUILayout.Slider("Time Scale", _timeScale, MinTimeScale, MaxTimeScale);
            if (EditorGUI.EndChangeCheck())
            {
                SetTimeScale(sliderValue);
            }

            if (GUILayout.Button("Reset To 1"))
            {
                SetTimeScale(1f);
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField($"Current Unity Time Scale: {Time.timeScale:0.00}", EditorStyles.miniLabel);
        }

        private void Update()
        {
            if (!Mathf.Approximately(_timeScale, Time.timeScale))
            {
                _timeScale = Mathf.Clamp(Time.timeScale, MinTimeScale, MaxTimeScale);
                Repaint();
            }
        }

        private void SetTimeScale(float value)
        {
            _timeScale = Mathf.Clamp(value, MinTimeScale, MaxTimeScale);
            Time.timeScale = _timeScale;
        }

        private void RefreshFromUnityTime()
        {
            _timeScale = Mathf.Clamp(Time.timeScale, MinTimeScale, MaxTimeScale);
        }
    }
}

