using System.Collections.Generic;
using Core;
using UnityEngine;

namespace InGame.GateEditorV2
{
    public class LevelBackgroundVariantEditorV2 : SerializedMonoSingleton<LevelBackgroundVariantEditorV2>
    {
        public Dictionary<LevelMapType, GameObject> backgroundMap;

        public void SetMapType(LevelMapType mapType)
        {
            foreach (var pair in backgroundMap)
            {
                if (pair.Key == mapType) pair.Value.SetActive(true);
                else pair.Value.SetActive(false);
            }
        }
    }
}