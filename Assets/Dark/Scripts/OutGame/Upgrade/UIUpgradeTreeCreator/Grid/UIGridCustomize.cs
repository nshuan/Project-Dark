using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Dark.Scripts.OutGame.Upgrade.UIUpgradeTreeCreator.Grid
{
    public class UIGridCustomize : MonoBehaviour
    {
        [SerializeField] private Toggle toggleGrid;
        [SerializeField] private Slider lineAlphaSlider;
        [SerializeField] private Slider lineThicknessSlider;
        [SerializeField] private TextMeshProUGUI txtAlphaValue;
        [SerializeField] private TextMeshProUGUI txtThicknessValue;

        private GridConfig config;
        
        private void Awake()
        {
            config = GridConfig.Instance;

            toggleGrid.isOn = config.enableGrid;
            lineAlphaSlider.value = config.lineColor.a;
            lineThicknessSlider.value = config.thickness;
            txtAlphaValue.SetText(config.lineColor.a.ToString("F"));
            txtThicknessValue.SetText(config.thickness.ToString("F"));
            
            toggleGrid.onValueChanged.AddListener((enable) =>
            {
                config.enableGrid = enable;
#if UNITY_EDITOR
                EditorUtility.SetDirty(config);
                AssetDatabase.SaveAssets();
#endif
                UIAlignManager.Instance.EnableGrid(enable);
            });
            lineAlphaSlider.onValueChanged.AddListener((value) =>
            {
                txtAlphaValue.SetText(value.ToString("F"));
                config.lineColor = new Color(config.lineColor.r, config.lineColor.g, config.lineColor.b, value);
                UIAlignManager.Instance.UpdateGrid();
            });
            lineThicknessSlider.onValueChanged.AddListener((value) =>
            {
                txtThicknessValue.SetText(value.ToString("F"));
                config.thickness = value;
                UIAlignManager.Instance.UpdateGrid();
            });
        }

        private void OnDestroy()
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();
#endif
        }
    }
}