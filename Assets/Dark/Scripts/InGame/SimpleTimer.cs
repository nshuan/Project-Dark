using System;
using TMPro;
using UnityEngine;

namespace InGame
{
    public class SimpleTimer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI txtSecond;
        
        private float second;

        private void Start()
        {
            second = 0f;
        }

        private void Update()
        {
            second += Time.deltaTime;
            txtSecond.SetText(Mathf.FloorToInt(second).ToString());
        }
    }
}