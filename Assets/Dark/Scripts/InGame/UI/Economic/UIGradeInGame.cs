using System;
using Economic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InGame.UI.Economic
{
    public class UIGradeInGame : MonoBehaviour
    {
        public Image fillExp;
        public TextMeshProUGUI txtGrade;
        
        private int grade;
        private int expNeededToLevelUp;
        private int currentExpProgress;
        
        private void Start()
        {
            OnUpGrade(WealthManager.Instance.Grade);
            
            WealthManager.Instance.OnUpGrade += OnUpGrade;
            WealthManager.Instance.OnExpChanged += OnExpChanged;
        }

        private void OnDestroy()
        {
            WealthManager.Instance.OnUpGrade -= OnUpGrade;
            WealthManager.Instance.OnExpChanged -= OnExpChanged;
        }

        private void OnUpGrade(int newGrade)
        {
            grade = newGrade;
            currentExpProgress = WealthManager.Instance.Exp - expNeededToLevelUp;
            expNeededToLevelUp = GradeConfig.Instance.GetRequirement(grade + 1);
            UpdateUIGrade();
            UpdateUIExp();
        }
        
        private void OnExpChanged(int before, int after)
        {
            currentExpProgress += after - before;
            UpdateUIExp();    
        }
        
        private void UpdateUIGrade()
        {
            txtGrade.SetText($"{grade}");
        }

        private void UpdateUIExp()
        {
            fillExp.fillAmount = (float)currentExpProgress / expNeededToLevelUp;
        }
    }
}