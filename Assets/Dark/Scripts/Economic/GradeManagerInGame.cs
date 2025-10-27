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
            WealthManager.Instance.OnUpGrade += OnUpgrade;
        }

        private void OnDestroy()
        {
            WealthManager.Instance.OnExpChanged -= OnExpChanged;
            WealthManager.Instance.OnUpGrade -= OnUpgrade;
        }

        private void OnExpChanged(int before, int after)
        {
            if (currentGrade >= GradeRequirement.Count) return;
            
            // Next grade requirement = GradeRequirement[currentGrade]
            if (after < GradeRequirement[currentGrade]) return;
            WealthManager.Instance.UpGrade();
        }

        private void OnUpgrade(int newGrade)
        {
            currentGrade = newGrade;
        }
    }
}