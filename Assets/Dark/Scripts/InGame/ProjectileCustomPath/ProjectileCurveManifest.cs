using System.Collections.Generic;
using System.Linq;
using Dark.Tools.Utils;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace InGame.ProjectileCustomPath
{
    public class ProjectileCurveManifest  : SerializedScriptableObject
    {
        public static string Path = "ProjectileCurveManifest";
        private static string FilePath = "Assets/Dark/Scripts/InGame/ProjectileCustomPath/Resources/ProjectileCurveManifest.asset";

        public Dictionary<int, AnimationCurve> trajectoryCurvesMap;
        public Dictionary<int, AnimationCurve> axisCorrectionCurveMap;
        public Dictionary<int, AnimationCurve> projectileSpeedCurveMap;
        
        public static AnimationCurve GetRandomTrajectoryCurve()
        {
            if (Instance.trajectoryCurvesMap == null || Instance.trajectoryCurvesMap.Count == 0) return null;
            return Instance.trajectoryCurvesMap.Values.ToArray()[RandomUtil.Range(0, instance.trajectoryCurvesMap.Count)];
        }

        public static AnimationCurve GetTrajectoryCurve(int id)
        {
            if (Instance.trajectoryCurvesMap == null || Instance.trajectoryCurvesMap.Count == 0) return null;
            return Instance.trajectoryCurvesMap.GetValueOrDefault(id);
        }

        public static AnimationCurve GetAxisCorrectionCurve(int id)
        {
            if (Instance.axisCorrectionCurveMap == null || Instance.axisCorrectionCurveMap.Count == 0) return null;
            return Instance.axisCorrectionCurveMap.GetValueOrDefault(id);
        }

        public static AnimationCurve GetProjectileSpeedCurve(int id)
        {
            if (Instance.projectileSpeedCurveMap == null || Instance.projectileSpeedCurveMap.Count == 0) return null;
            return Instance.projectileSpeedCurveMap.GetValueOrDefault(id);
        }

        private static ProjectileCurveManifest instance;
        public static ProjectileCurveManifest Instance
        {
            get
            {
                if (!instance) instance = Resources.Load<ProjectileCurveManifest>(Path);
                return instance;
            }
        }
        
#if UNITY_EDITOR
        [MenuItem("Dark/Manifest/Generate Projectile Curve Manifest")]
        public static void CreateInstance()
        {
            AssetDatabaseUtils.CreateSOInstance<ProjectileCurveManifest>(FilePath);
        }
#endif
    }
}