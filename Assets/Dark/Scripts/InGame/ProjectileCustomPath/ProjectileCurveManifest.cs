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
            var instance = Resources.Load<ProjectileCurveManifest>(Path);
            if (instance.trajectoryCurvesMap == null || instance.trajectoryCurvesMap.Count == 0) return null;
            var result = instance.trajectoryCurvesMap.Values.ToArray()[RandomUtil.Range(0, instance.trajectoryCurvesMap.Count)];
            Resources.UnloadAsset(instance);
            return result;
        }

        public static AnimationCurve GetTrajectoryCurve(int id)
        {
            var instance = Resources.Load<ProjectileCurveManifest>(Path);
            if (instance.trajectoryCurvesMap == null || instance.trajectoryCurvesMap.Count == 0) return null;
            var result = instance.trajectoryCurvesMap.GetValueOrDefault(id);
            Resources.UnloadAsset(instance);
            return result;
        }

        public static AnimationCurve GetAxisCorrectionCurve(int id)
        {
            var instance = Resources.Load<ProjectileCurveManifest>(Path);
            if (instance.axisCorrectionCurveMap == null || instance.axisCorrectionCurveMap.Count == 0) return null;
            var result = instance.axisCorrectionCurveMap.GetValueOrDefault(id);
            Resources.UnloadAsset(instance);
            return result;
        }

        public static AnimationCurve GetProjectileSpeedCurve(int id)
        {
            var instance = Resources.Load<ProjectileCurveManifest>(Path);
            if (instance.projectileSpeedCurveMap == null || instance.projectileSpeedCurveMap.Count == 0) return null;
            var result = instance.projectileSpeedCurveMap.GetValueOrDefault(id);
            Resources.UnloadAsset(instance);
            return result;
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