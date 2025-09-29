using System;
using UnityEngine;

namespace InGame.UI.CombatSkills
{
    public class UISkillIconManager : MonoBehaviour
    {
        [SerializeField] private UIInGameSkillIcon[] icons;

        private void Awake()
        {
            foreach (var icon in icons)
            {
                var ic = icon;
                icon.CheckShowSkill(() =>
                {
                    ic.gameObject.SetActive(true);
                }, () =>
                {
                    ic.gameObject.SetActive(false);
                });
            }
        }
    }
}