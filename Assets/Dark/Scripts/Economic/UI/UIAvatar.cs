using System;
using Data;
using UnityEngine;
using UnityEngine.UI;

namespace Economic.UI
{
    public class UIAvatar : MonoBehaviour
    {
        public GameObject[] avatars;

        private void OnEnable()
        {
            var activeId = PlayerDataManager.Instance.Data.characterClass;
            if (activeId >= avatars.Length) activeId = 0;
            for (var i = 0; i < avatars.Length; i++)
            {
                avatars[i].SetActive(i == activeId);
            }
        }
    }
}