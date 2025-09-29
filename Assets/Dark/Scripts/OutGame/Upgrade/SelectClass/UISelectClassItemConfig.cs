using DG.Tweening;
using UnityEngine;

namespace Dark.Scripts.OutGame.Upgrade
{
    public class UISelectClassItemConfig : MonoBehaviour
    {
        public float widthCollapse;
        public float widthExpand;
        public float iconLightAlpha = 0.5f;
        public float expandDuration = 0.5f;
        public Ease expandEasing;
        public float lightOnDuration;
        public Ease lightOnEasing;
        public float lightOffDuration;
        public Ease lightOffEasing;
    }
}