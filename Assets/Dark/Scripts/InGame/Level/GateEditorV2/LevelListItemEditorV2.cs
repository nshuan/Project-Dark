using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InGame.GateEditorV2
{
    public class LevelListItemEditorV2 : MonoBehaviour
    {
#if UNITY_EDITOR
        public TextMeshProUGUI txtLevel;
        public Button btnClick;
        
        public LevelConfig Config { get; set; }
        public bool Selecting { get; set; }
        
        public void UpdateUI(LevelConfig level)
        {
            Config = level;
            txtLevel.SetText($"Level {level.level}");
            btnClick.onClick.RemoveAllListeners();
            btnClick.onClick.AddListener(() =>
            {
                LevelGateEditorV2.Instance.LoadLevel(level.level);
            });
        }
#endif
    }
}