using System;
using System.Collections;
using UnityEngine;

namespace InGame
{
    public class LightningBall : MonoBehaviour
    {
        public GameObject vfxLightningBall;
        public Transform Target { get; set; }

        private void OnDisable()
        {
            vfxLightningBall.SetActive(false);
        }

        private void Update()
        {
            if (!Target) return;
            if (!Target.gameObject.activeInHierarchy) return;

            transform.position = Target.position;
        }

        public void ShowVfx()
        {
            vfxLightningBall.SetActive(true);
        }

        public void HideVfx()
        {
            vfxLightningBall.SetActive(false);
        }
    }
}