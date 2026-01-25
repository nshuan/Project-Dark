using Sirenix.OdinInspector;
using UnityEngine;
using Gradient = UnityEngine.UI.Extensions.Gradient;

namespace InGame.UI.Waves
{
    public class UIWaveNodeGradientColor : Gradient
    {
        public Color startColor;
        public Color endColor;
        [Range(0f, 1f)] public float nodePosition;
        public float padding;

        [Button]
        protected override void OnValidate()
        {
            base.OnValidate();

            var nodeStart = nodePosition - padding;
            var nodeEnd = nodePosition + padding;
            Vertex1 = startColor + (endColor - startColor) * nodeStart;
            Vertex2 = startColor + (endColor - startColor) * nodeEnd;
        }
    }
}