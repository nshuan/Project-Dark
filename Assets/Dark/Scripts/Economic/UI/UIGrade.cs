using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Economic.UI
{
    public class UIGrade : MonoBehaviour
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
            currentExpProgress = WealthManager.Instance.Exp % (expNeededToLevelUp == 0 ? 1 : expNeededToLevelUp);
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
            DOTween.Kill(this);
            fillExp.DOFillAmount((float)currentExpProgress / expNeededToLevelUp, 0.5f).SetTarget(this);
        }
    }
}