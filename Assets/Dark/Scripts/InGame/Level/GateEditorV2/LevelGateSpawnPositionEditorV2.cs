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
        public Button btnClose;
        public LevelGateSpawnPositionItemV2 positionPrefab;
        public Button btnDelete;

        private LevelGatePrefabEditorV2 cacheGate;
        
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
                    item.transform.position = cacheGate.camera.WorldToScreenPoint(position);
                }
            }
            
            gameObject.SetActive(true);
            btnClose.onClick.RemoveAllListeners();
            btnClose.onClick.AddListener(() => gameObject.SetActive(false));
            btnDelete.onClick.RemoveAllListeners();
            btnDelete.onClick.AddListener(() =>
            {
                foreach (Transform child in positionParent)
                {
                    if (child.TryGetComponent<LevelGateSpawnPositionItemV2>(out var position))
                    {
                        if (position.Selecting)
                            Destroy(child.gameObject);
                    }
                }

                UpdateSpawnPositions();
            });
        }

        public void Select(LevelGateSpawnPositionItemV2 selected)
        {
            foreach (Transform child in positionParent)
            {
                if (child.TryGetComponent<LevelGateSpawnPositionItemV2>(out var position))
                {
                    position.Deselect();
                }
            }
            
            selected.Select();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            var item = Instantiate(positionPrefab, positionParent);
            item.manager = this;
            item.transform.position = eventData.position;
            Select(item);
            cacheGate.SpawnPositions ??= new List<Vector2>();
            cacheGate.SpawnPositions.Add(cacheGate.camera.ScreenToWorldPoint(eventData.position));
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
            cacheGate.SpawnPositions = new List<Vector2>();
            foreach (Transform child in positionParent)
            {
                if (child.TryGetComponent<LevelGateSpawnPositionItemV2>(out var position))
                {
                    cacheGate.SpawnPositions.Add(cacheGate.camera.ScreenToWorldPoint(child.transform.position));
                }
            }
        }
    }
}