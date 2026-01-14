using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace InGame.GateEditorV2
{
    public class LevelGateSpawnPositionItemV2 : MonoBehaviour, IPointerClickHandler, IEndDragHandler, IDragHandler
    {
        public Image imgSelected;
        public LevelGateSpawnPositionEditorV2 manager;
        
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
            manager?.Select(this);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            manager?.UpdateSpawnPositions();
        }

        public void OnDrag(PointerEventData eventData)
        {
            transform.position += new Vector3(eventData.delta.x, eventData.delta.y, 0);
        }
    }
}