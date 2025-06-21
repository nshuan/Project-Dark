using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace InGame
{
    public class InputInGame : MonoBehaviour, IPointerClickHandler
    {
        private Camera cam;
        [SerializeField] private Canvas canvas;
        private IMouseInput mouseInput;
        public PlayerSkillConfig currentSkillConfig;
        public Transform cursorRangeCenter;
        public float cursorRangeRadius;

        private void Awake()
        {
            cam = Camera.main;

            LevelManager.Instance.OnChangeTower += OnTowerChanged;
            LevelManager.Instance.OnChangeSkill += OnSkillChanged;
        }

        private void OnSkillChanged(PlayerSkillConfig skillConfig)
        {
            if (!skillConfig) return;

            currentSkillConfig = skillConfig;
            cursorRangeRadius = skillConfig.range;
            
            if (mouseInput != null)
            {
                mouseInput.Dispose();
                mouseInput = null;
            }
            var cursor = ShotCursorManager.Instance.GetPrefab(skillConfig.shootLogic.cursorType, canvas.transform);
            mouseInput = ShotCursorManager.Instance.GetCursorMoveLogic(skillConfig.shootLogic.cursorType, cam, cursor);
            mouseInput.InputManager = this;
        }

        private void OnTowerChanged(Transform towerTransform)
        {
            cursorRangeCenter = towerTransform;
        }

        private void Update()
        {
            mouseInput?.OnUpdate();
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            mouseInput?.OnMouseClick();
        }

        private void OnDrawGizmos()
        {
            mouseInput?.OnDrawGizmos();
        }
    }
}