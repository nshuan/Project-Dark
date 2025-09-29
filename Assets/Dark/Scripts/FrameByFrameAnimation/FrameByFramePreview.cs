#if UNITY_EDITOR
using UnityEditor;
#endif
using Dark.Scripts.FrameByFrameAnimation;
using UnityEngine;

[ExecuteAlways]
public class FrameByFramePreview : MonoBehaviour
{
    public FrameByFrameAnimation anim;
    public float fps = 12f;
    private SpriteRenderer sr;
    private int currentFrame;
    private double lastTime;

    void OnEnable()
    {
        sr = GetComponent<SpriteRenderer>();
#if UNITY_EDITOR
        lastTime = EditorApplication.timeSinceStartup;
        EditorApplication.update += EditorUpdate;
#endif
    }

#if UNITY_EDITOR
    void OnDisable()
    {
        EditorApplication.update -= EditorUpdate;
    }

    void EditorUpdate()
    {
        double t = EditorApplication.timeSinceStartup;
        if (t - lastTime >= 1f / fps)
        {
            lastTime = t;
            currentFrame = (currentFrame + 1) % anim.frames.Length;
            sr.sprite = anim.frames[currentFrame];
            EditorApplication.QueuePlayerLoopUpdate();
        }
    }
#endif
}