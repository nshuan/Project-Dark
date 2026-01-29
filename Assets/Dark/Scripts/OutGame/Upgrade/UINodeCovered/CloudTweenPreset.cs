using UnityEngine;

[CreateAssetMenu(menuName = "UI/Cloud Tween Preset")]
public class CloudTweenPreset : ScriptableObject
{
    [Header("Position Float")]
    public Vector2 moveRange = new Vector2(4f, 10f); // pixels
    public Vector2 moveDuration = new Vector2(4f, 8f);

    [Header("Scale Breathing")]
    public Vector2 scaleRange = new Vector2(0.01f, 0.03f);
    public Vector2 scaleDuration = new Vector2(6f, 12f);

    [Header("General")]
    public Vector2 startDelay = new Vector2(0f, 3f);

    #region Singleton

    private static CloudTweenPreset _instance;

    public static CloudTweenPreset Instance
    {
        get
        {
            if (!_instance) _instance = Resources.Load<CloudTweenPreset>("UICloudTweenPreset");
            return _instance;
        }
    }

    #endregion
}