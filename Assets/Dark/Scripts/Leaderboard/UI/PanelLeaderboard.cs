using UnityEngine;
using UnityEngine.UI;

namespace Dark.Scripts.Leaderboard.UI
{
    public class PanelLeaderboard : MonoBehaviour
    {
        [SerializeField] private Button btnTabGeneral;
        [SerializeField] private Button btnTabEndless;
        
        public void ShowGeneral()
        {
            gameObject.SetActive(true);
            btnTabGeneral?.onClick.Invoke();
        }

        public void ShowEndless()
        {
            gameObject.SetActive(true);
            btnTabEndless?.onClick.Invoke();
        }
    }
}