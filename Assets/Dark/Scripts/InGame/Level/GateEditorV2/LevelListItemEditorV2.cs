using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InGame.GateEditorV2
{
    public class LevelListItemEditorV2 : MonoBehaviour
    {
        public TextMeshProUGUI txtLevel;
        public Button btnClick;
        
        public void UpdateUI(LevelConfig level)
        {
            txtLevel.SetText($"Level {level.level}");
            btnClick.onClick.RemoveAllListeners();
            btnClick.onClick.AddListener(() =>
            {
                LevelGateEditorV2.Instance.LoadLevel(level.level);
            });
        }
    }
}