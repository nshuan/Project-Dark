using System;
using UnityEngine;

namespace Economic
{
    public class GradeManagerInGame : MonoBehaviour
    {
        private int[] GradeRequirement { get; set; }
        private int currentGrade;
        
        private void Start()
        {
            GradeRequirement = GradeConfig.Instance.GetAllRequirement();
            currentGrade = WealthManager.Instance.Grade;
            
            WealthManager.Instance.OnExpChanged += OnExpChanged;
        }

        private void OnExpChanged(int before, int after)
        {
            if (currentGrade >= GradeRequirement.Length) return;
            
            // Next grade requirement = GradeRequirement[currentGrade]
            if (after < GradeRequirement[currentGrade]) return;
            currentGrade += 1;
            WealthManager.Instance.UpGrade();
        }
    }
}