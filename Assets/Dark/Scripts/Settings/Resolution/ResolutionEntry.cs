using System;

namespace Dark.Scripts.Settings.Resolution
{
    [Serializable]
    public struct ResolutionEntry
    {
        public int width;
        public int height;
        public string label;

        public ResolutionEntry(int width, int height, string label = null)
        {
            this.width = width;
            this.height = height;
            this.label = label;
        }

        public string ToDisplayString()
        {
            if (!string.IsNullOrWhiteSpace(label)) return label;
            return $"{width} x {height}";
        }
    }
}

