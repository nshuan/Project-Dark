using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace InGame.GateEditorV2
{
    public class LevelGateSpawnPositionEditorV2 : MonoBehaviour, IPointerClickHandler
    {
        public Transform positionParent;
        public Transform atkPositionParent;
        public Button btnClose;
        public LevelGateSpawnPositionItemV2 positionPrefab;
        public Button btnDelete;
        public Button btnDeleteAttackPosition;

        public LevelGatePrefabEditorV2 cacheGate;
        
        public void Setup(LevelGatePrefabEditorV2 gate)
        {
            cacheGate = gate;
            
            foreach (Transform child in positionParent)
            {
                Destroy(child.gameObject);
            }
            
            if (!gate || gate.SpawnPositions == null)
            {
                gameObject.SetActive(false);
                return;
            }

            if (gate.Config == null || gate.Config.spawnLogic is not GateSpawnPositions spawnLogic)
            {
                gameObject.SetActive(false);
                return;
            }


            if (gate.SpawnPositions is { Count: > 0 })
            {
                foreach (var position in gate.SpawnPositions)
                {
                    var item = Instantiate(positionPrefab, positionParent);
                    item.manager = this;
                    item.SetAttackPositions(position.Value.attackPositions);
                    item.Deselect();
                    item.guid = position.Key;
                    item.transform.position = cacheGate.camera.WorldToScreenPoint(position.Value.spawnPosition);
                }
            }
            
            gameObject.SetActive(true);
            btnClose.onClick.RemoveAllListeners();
            btnClose.onClick.AddListener(() =>
            {
                foreach (Transform child in positionParent)
                {
                    Destroy(child.gameObject);
                }

                foreach (Transform child in atkPositionParent)
                {
                    Destroy(child.gameObject);
                }
                
                gameObject.SetActive(false);
            });
            btnDelete.onClick.RemoveAllListeners();
            btnDelete.onClick.AddListener(() =>
            {
                foreach (Transform child in positionParent)
                {
                    if (child.TryGetComponent<LevelGateSpawnPositionItemV2>(out var position))
                    {
                        if (position.Selecting)
                        {
                            if (position.attackPositions != null)
                            {
                                foreach (var atkPos in position.attackPositions)
                                {
                                    Destroy(atkPos.gameObject);
                                }
                            }
                            Destroy(child.gameObject);
                        }
                    }
                }

                UpdateSpawnPositions();
            });
            btnDeleteAttackPosition.onClick.RemoveAllListeners();
            btnDeleteAttackPosition.onClick.AddListener(() =>
            {
                foreach (Transform child in positionParent)
                {
                    if (child.TryGetComponent<LevelGateSpawnPositionItemV2>(out var position))
                    {
                        if (position.Selecting && position.attackPositions != null)
                        {
                            var newAttackPositions = new List<LevelGateAttackPositionItemV2>();
                            foreach (var attackPosition in position.attackPositions)
                            {
                                if (attackPosition.Selecting)
                                {
                                    Destroy(attackPosition.gameObject);
                                }
                                else
                                {
                                    newAttackPositions.Add(attackPosition);
                                }
                            }

                            position.attackPositions = newAttackPositions;
                        }
                    }
                }
            });
        }

        public int currentSelectingId;

        public void Select(LevelGateSpawnPositionItemV2 selected, bool forceSelectOne = false)
        {
            if (Input.GetKey(KeyCode.LeftShift) && !forceSelectOne)
            {
                if (selected.Selecting) selected.Deselect();
                else
                {
                    selected.Select(currentSelectingId);
                    currentSelectingId += 1;
                }
            }
            else
            {
                foreach (Transform child in positionParent)
                {
                    if (child.TryGetComponent<LevelGateSpawnPositionItemV2>(out var position))
                    {
                        position.Deselect();
                    }
                }
                
                selected.Select(0);
                currentSelectingId = 1;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            // Dùng chuột trái để thêm spawn position
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                var item = Instantiate(positionPrefab, positionParent);
                item.attackPositions = new List<LevelGateAttackPositionItemV2>();
                item.manager = this;
                item.guid = Guid.NewGuid();
                item.transform.position = eventData.position;
                Select(item, true);
                cacheGate.SpawnPositions ??= new Dictionary<Guid, GateSpawnPositionInfo>();
                cacheGate.SpawnPositions.Add(item.guid,
                    new GateSpawnPositionInfo()
                        { spawnPosition = cacheGate.camera.ScreenToWorldPoint(eventData.position) });
            }
            // Dùng chuột phải để thêm attack position
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                foreach (Transform child in positionParent)
                {
                    if (child.TryGetComponent<LevelGateSpawnPositionItemV2>(out var position))
                    {
                        if (position.Selecting)
                        {
                            var newAttackPosition = Instantiate(position.attackPosPrefab, atkPositionParent);
                            newAttackPosition.manager = this;
                            newAttackPosition.spawnPosition = position;
                            newAttackPosition.transform.position = eventData.position;
                            position.attackPositions ??= new List<LevelGateAttackPositionItemV2>();
                            position.attackPositions.Add(newAttackPosition);
                            position.SelectAttackPosition(newAttackPosition, true);
                        }
                    }
                }
                
                UpdateSpawnPositions();
            }
        }

        private Coroutine coroutineUpdateSpawnPositions;
        
        public void UpdateSpawnPositions()
        {
            if (coroutineUpdateSpawnPositions != null)
                StopCoroutine(coroutineUpdateSpawnPositions);
            coroutineUpdateSpawnPositions = StartCoroutine(IEUpdateSpawnPositions());
        }

        private IEnumerator IEUpdateSpawnPositions()
        {
            if (!cacheGate) yield break;
            
            yield return new WaitForEndOfFrame();
            cacheGate.SpawnPositions = new Dictionary<Guid, GateSpawnPositionInfo>();
            foreach (Transform child in positionParent)
            {
                if (child.TryGetComponent<LevelGateSpawnPositionItemV2>(out var position))
                {
                    var newInfo = new GateSpawnPositionInfo()
                        { spawnPosition = cacheGate.camera.ScreenToWorldPoint(child.transform.position) };

                    if (position.attackPositions == null)
                    {
                        newInfo.attackPositions = Array.Empty<Vector2>();
                    }
                    else
                    {
                        newInfo.attackPositions = new Vector2[position.attackPositions.Count];
                        for (var i = 0; i < position.attackPositions.Count; i++)
                        {
                            newInfo.attackPositions[i] = cacheGate.camera.ScreenToWorldPoint(position.attackPositions[i].transform.position);
                        }
                    }
                    
                    cacheGate.SpawnPositions.Add(position.guid, newInfo);
                }
            }
        }
    }
}