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

        private List<int> requireMapByTarget;

        public List<int> RequireMapByTarget
        {
            get
            {
                if (requireMapByTarget == null || requireMapByTarget.Count == 0) 
                    GetAllRequirement();

                return requireMapByTarget;
            }
        }
        
        public int GetRequirement(int grade)
        {
            if (grade <= 0) return 0;
            return grade > RequireMapByTarget.Count ? int.MaxValue : RequireMapByTarget[grade];
        }

        public void GetAllRequirement()
        {
            requireMapByTarget = new List<int>();
            for (int i = 0; i < gradeRequireMap.Length; i++)
            {
                if (i == 0) requireMapByTarget.Add(gradeRequireMap[i]);
                else requireMapByTarget.Add(requireMapByTarget[i - 1] + gradeRequireMap[i]);
            }
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