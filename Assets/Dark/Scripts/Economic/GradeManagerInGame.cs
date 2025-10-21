using System;
using System.Collections.Generic;
using UnityEngine;

namespace Economic
{
    public class GradeManagerInGame : MonoBehaviour
    {
        private List<int> GradeRequirement { get; set; }
        private int currentGrade;
        
        private void Start()
        {
            GradeRequirement = GradeConfig.Instance.RequireMapByTarget;
            currentGrade = WealthManager.Instance.Grade;
            
            WealthManager.Instance.OnExpChanged += OnExpChanged;
        }

        private void OnExpChanged(int before, int after)
        {
            if (currentGrade >= GradeRequirement.Count) return;
            
            // Next grade requirement = GradeRequirement[currentGrade]
            if (after < GradeRequirement[currentGrade]) return;
            currentGrade += 1;
            WealthManager.Instance.UpGrade();
        }
    }
}