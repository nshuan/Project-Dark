using UnityEngine;

namespace Dark.Scripts.Utils.Skeleton
{
    public class SpineGraphicNameAttribute : PropertyAttribute
    {
        public string skeletonFieldName;

        public SpineGraphicNameAttribute(string skeletonFieldName)
        {
            this.skeletonFieldName = skeletonFieldName;
        }
    }
}