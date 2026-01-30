using System;
using System.Collections.Generic;
using System.Globalization;
using Core;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace InGame.GateEditorV2
{
    public class LevelTowerPositionEditorV2 : MonoBehaviour, IBeginDragHandler, IDragHandler
    {
        public Transform inspector;
        public Transform prefabInpPosition;
        
        private Camera cam;

        private TowerEntity[] towersToMove;
        private int movingIndex;
        private List<(TMP_InputField, TMP_InputField)> positionInputs;
        
        private void Start()
        {
            cam = Camera.main;
        }

        public void Setup(TowerEntity[] towers)
        {
            cam = Camera.main;
            towersToMove = towers;
            foreach (Transform child in inspector)
            {
                Destroy(child.gameObject);
            }

            if (!cam) return;
            positionInputs = new List<(TMP_InputField, TMP_InputField)>();
            if (towers != null)
            {
                for (var i = 0; i < towers.Length; i++)
                {
                    var tower = towers[i];
                    var editor = Instantiate(prefabInpPosition, inspector);
                    editor.gameObject.SetActive(true);
                    var label = editor.Find("txtLabel");
                    if (label)
                    {
                        if (label.TryGetComponent<TextMeshProUGUI>(out var txtLabel))
                            txtLabel.SetText($"Tower {i}");
                    }
                        
                    var inpfields = editor.GetComponentsInChildren<TMP_InputField>();
                    inpfields.Sort((i1, i2) => i1.transform.GetSiblingIndex().CompareTo(i2.transform.GetSiblingIndex()));
                    (TMP_InputField, TMP_InputField) pair = (null, null);
                    if (inpfields.Length > 0)
                    {
                        inpfields[0].text = tower.transform.position.x.ToString(CultureInfo.InvariantCulture);
                        inpfields[0].onValueChanged.RemoveAllListeners();
                        inpfields[0].onValueChanged.AddListener((value) =>
                        {
                            if (float.TryParse(value, out var x))
                                tower.transform.position = new Vector2(x, tower.transform.position.y);
                        });

                        pair.Item1 = inpfields[0];
                    }
                    if (inpfields.Length > 1)
                    {
                        inpfields[1].text = tower.transform.position.y.ToString(CultureInfo.InvariantCulture);
                        inpfields[1].onValueChanged.RemoveAllListeners();
                        inpfields[1].onValueChanged.AddListener((value) =>
                        {
                            if (float.TryParse(value, out var y))
                                tower.transform.position = new Vector2(tower.transform.position.x, y);
                        });
                        
                        pair.Item2 = inpfields[1];
                    }
                    
                    positionInputs.Add(pair);
                }
            }
            
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (towersToMove == null) return;
            if (!cam) return;

            movingIndex = -1;

            for (var i = 0; i < towersToMove.Length; i++)
            {
                var tower = towersToMove[i];
                if (Vector2.Distance(tower.transform.position, cam.ScreenToWorldPoint(eventData.position)) < 2f)
                {
                    movingIndex = i;
                    break;
                }
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (towersToMove == null) return;
            if (movingIndex < 0) return;
            if (movingIndex >= towersToMove.Length) return;

            var worldPos = cam.ScreenToWorldPoint(eventData.position);
            worldPos.z = 0f;
            towersToMove[movingIndex].transform.position = worldPos;
            if (positionInputs != null && movingIndex < positionInputs.Count)
            {
                if (positionInputs[movingIndex].Item1)
                    positionInputs[movingIndex].Item1.SetTextWithoutNotify(towersToMove[movingIndex].transform.position.x
                        .ToString(CultureInfo.InvariantCulture));
                if (positionInputs[movingIndex].Item2)
                    positionInputs[movingIndex].Item2.SetTextWithoutNotify(towersToMove[movingIndex].transform.position.y
                        .ToString(CultureInfo.InvariantCulture));
            }
        }
    }
}