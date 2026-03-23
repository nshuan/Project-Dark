using System.Collections.Generic;
using Core;
using UnityEngine;

namespace InGame.GateEditorV2
{
    public class LevelBackgroundVariantEditorV2 : SerializedMonoSingleton<LevelBackgroundVariantEditorV2>
    {
        public List<GameObject> backgroundMap = new List<GameObject>();
        public Dictionary<LevelMapType, GameObject> towerMap;

        public void SetMapType(LevelMapType mapType)
        {
            foreach (var pair in towerMap)
            {
                if (pair.Key == mapType) pair.Value.SetActive(true);
                else pair.Value.SetActive(false);
            }
        }
        
        public void SetBackgroundType(int index)
        {
            for (var i = 0; i < backgroundMap.Count; i++)
            {
                backgroundMap[i].SetActive(i == index);
            }
        }
    }
}