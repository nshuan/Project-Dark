using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace InGame.GateEditorV2
{
    public class LevelGateSpawnPositionItemV2 : MonoBehaviour, IPointerClickHandler, IEndDragHandler, IDragHandler
    {
        public Image imgSelected;
        public LevelGateSpawnPositionEditorV2 manager;
        public Guid guid;
        public List<LevelGateAttackPositionItemV2> attackPositions;
        public LevelGateAttackPositionItemV2 attackPosPrefab;
        
        public bool Selecting { get; set; }

        public void Select()
        {
            Selecting = true;
            imgSelected.gameObject.SetActive(true);
            if (attackPositions != null)
                foreach (var position in attackPositions)
                {
                    position.gameObject.SetActive(true);
                }
        }

        public void Deselect()
        {
            Selecting = false;
            imgSelected.gameObject.SetActive(false);
            if (attackPositions != null)
                foreach (var position in attackPositions)
                {
                    position.Deselect();
                    position.gameObject.SetActive(false);
                }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            // Dùng chuột trái
            if (eventData.button != PointerEventData.InputButton.Left) return;
            
            manager?.Select(this);
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

        public void SetAttackPositions(Vector2[] listAttackPosition)
        {
            attackPositions = new List<LevelGateAttackPositionItemV2>();
            if (listAttackPosition == null) return;
            foreach (var position in listAttackPosition)
            {
                var item = Instantiate(attackPosPrefab, manager.atkPositionParent);
                item.manager = manager;
                item.spawnPosition = this;
                item.transform.position = manager.cacheGate.camera.WorldToScreenPoint(position);
                item.Deselect();
                attackPositions.Add(item);
            }
        }

        public void SelectAttackPosition(LevelGateAttackPositionItemV2 atkPosition, bool forceSelectOne)
        {
            if (Input.GetKey(KeyCode.LeftShift) && !forceSelectOne)
            {
                if (atkPosition.Selecting) atkPosition.Deselect();
                else atkPosition.Select();
            }
            else if (attackPositions != null)
            {
                foreach (var child in attackPositions)
                {
                    child.Deselect();
                }
                
                atkPosition.Select();
            }
        }
    }
}