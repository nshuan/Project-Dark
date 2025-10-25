namespace Dark.Scripts.Utils.Skeleton
{
    using UnityEngine;

    public class SpineAnimationNameAttribute : PropertyAttribute
    {
        public string skeletonFieldName;

        public SpineAnimationNameAttribute(string skeletonFieldName)
        {
            this.skeletonFieldName = skeletonFieldName;
        }
    }

}