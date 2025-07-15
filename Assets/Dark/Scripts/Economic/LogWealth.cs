using System;
using TMPro;
using UnityEngine;

namespace Economic
{
    public class LogWealth : MonoBehaviour
    {
        public TextMeshProUGUI txtGrade;
        public TextMeshProUGUI txtExp;
        public TextMeshProUGUI txtLevelPoint;
        public TextMeshProUGUI txtDark;
        public TextMeshProUGUI txtBossPoint;

        private int grade;
        private int exp;
        private int levelPoint;
        private int dark;
        private int bossPoint;
        
        private void Start()
        {
            grade = WealthManager.Instance.Grade;
            exp = WealthManager.Instance.Exp;
            levelPoint = WealthManager.Instance.LevelPoint;
            dark = WealthManager.Instance.Dark;
            bossPoint = WealthManager.Instance.BossPoint;
            UpdateUI();

            WealthManager.Instance.OnUpGrade += OnUpGrade;
            WealthManager.Instance.OnExpChanged += OnExpChanged;
            WealthManager.Instance.OnLevelPointChanged += OnLevelPointChanged;
            WealthManager.Instance.OnDarkChanged += OnDarkChanged;
            WealthManager.Instance.OnBossPointChanged += OnBossPointChanged;
        }

        private void UpdateUI()
        {
            txtGrade.SetText($"Grade: {grade}");
            txtExp.SetText($"Exp: {exp}");
            txtLevelPoint.SetText($"Level Point: {levelPoint}");
            txtDark.SetText($"Dark: {dark}");
            txtBossPoint.SetText($"Boss Point: {bossPoint}");
        }

        private void OnUpGrade(int newGrade)
        {
            grade = newGrade;
            UpdateUI();
        }
        
        private void OnExpChanged(int before, int after)
        {
            exp = after;
            UpdateUI();    
        }
        
        private void OnLevelPointChanged(int before, int after)
        {
            levelPoint = after;
            UpdateUI();
        }
        
        private void OnDarkChanged(int before, int after)
        {
            dark = after;
            UpdateUI();
        }
        
        private void OnBossPointChanged(int before, int after)
        {
            bossPoint = after;
            UpdateUI();
        }
    }
}