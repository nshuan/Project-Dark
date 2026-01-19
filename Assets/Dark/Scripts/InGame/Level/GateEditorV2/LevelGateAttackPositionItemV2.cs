using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace InGame.GateEditorV2
{
    public class LevelGateAttackPositionItemV2 : MonoBehaviour, IPointerClickHandler, IEndDragHandler, IDragHandler
    {
        public Image imgSelected;
        public LevelGateSpawnPositionEditorV2 manager;
        public LevelGateSpawnPositionItemV2 spawnPosition;
        
        public bool Selecting { get; set; }

        public void Select()
        {
            Selecting = true;
            imgSelected.gameObject.SetActive(true);
        }

        public void Deselect()
        {
            Selecting = false;
            imgSelected.gameObject.SetActive(false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            // Dùng chuột trái
            if (eventData.button != PointerEventData.InputButton.Left) return;
            
            spawnPosition?.SelectAttackPosition(this, false);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            manager?.UpdateSpawnPositions();
        }

        public void OnDrag(PointerEventData eventData)
        {
            // Dùng chuột trái
            if (eventData.button != PointerEventData.InputButton.Left) return;
            transform.position += new Vector3(eventData.delta.x, eventData.delta.y, 0);
        }
    }
}