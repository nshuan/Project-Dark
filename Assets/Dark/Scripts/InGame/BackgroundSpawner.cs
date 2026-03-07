using System.Collections.Generic;
using UnityEngine;

namespace InGame
{
    public class BackgroundSpawner : MonoBehaviour
    {
        [SerializeField] private List<GameObject> listBackground;

        public void Spawn(int index)
        {
            if (listBackground == null || listBackground.Count == 0) return;
            index = Mathf.Clamp(index, 0, listBackground.Count - 1);
            for (var i = 0; i < listBackground.Count; i++)
            {
                listBackground[i].SetActive(i == index);
            }
        }
    }
}