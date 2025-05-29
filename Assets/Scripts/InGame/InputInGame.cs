using System;
using InGame.MouseInput;
using UnityEngine;
using UnityEngine.EventSystems;

namespace InGame
{
    public class InputInGame : MonoBehaviour, IPointerClickHandler
    {
        private Camera cam;
        [SerializeField] private Canvas canvas;
        private IMouseInput mouseInput;

        private void Awake()
        {
            cam = Camera.main;
        }

        private void Start()
        {
            var cursor = Instantiate(ShotCursorManager.Instance.GetPrefab(ShotCursorType.SINGLE), canvas.transform);
            mouseInput = new MoveSingleShot(cam, cursor, 2f);
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