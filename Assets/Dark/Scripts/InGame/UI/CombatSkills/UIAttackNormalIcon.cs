using System;
using InGame.UI.InGameToast;
using UnityEngine;

namespace InGame.UI.CombatSkills
{
    public class UIAttackNormalIcon : UIInGameSkillIcon
    {
        [Space] [Header("Toast")]
        [SerializeField] private Sprite toastIcon;
        
        private void Start()
        {
            CombatActions.OnAttackNormal += OnSkillUsed;
        }

        private void OnDestroy()
        {
            CombatActions.OnAttackNormal -= OnSkillUsed;
        }

        public override void CheckShowSkill(Action callbackShow, Action callbackHide)
        {
            callbackShow?.Invoke();
        }

        protected override void ShowToast()
        {
            return;
            ToastInGameManager.Instance.Register(
                message: "Attack is ready!",
                icon: toastIcon);
        }
    }
}