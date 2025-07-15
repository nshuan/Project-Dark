using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Economic
{
    [CreateAssetMenu(menuName = "Economic/Grade Config", fileName = "GradeConfig")]
    public class GradeConfig : SerializedScriptableObject
    {
        [NonSerialized, OdinSerialize] private int[] gradeRequireMap;

        public int GetRequirement(int grade)
        {
            if (grade <= 0) return 0;
            return grade > gradeRequireMap.Length ? int.MaxValue : gradeRequireMap[grade - 1];
        }

        public int[] GetAllRequirement()
        {
            return gradeRequireMap;
        }
        
        #region SINGLETON

        private static GradeConfig instance;

        public static GradeConfig Instance
        {
            get
            {
                if (instance == null)
                    instance = Resources.Load<GradeConfig>("GradeConfig");

                return instance;
            }
        }
        #endregion
    }
}