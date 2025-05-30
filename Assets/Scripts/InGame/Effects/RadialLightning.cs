using System.Collections.Generic;
using UnityEngine;

namespace InGame.Effects
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class RadialLightning : MonoBehaviour
    {
        public int segments = 8;
        public float width = 0.1f;
        public float length = 4f;
        public float jaggedness = 0.3f;
        public int rayCount = 12;
        public float updateInterval = 0.1f;
        public float fadeDuration = 0.5f;
        public float burstDuration = 2f;
        public Material lightningMaterial;

        private Mesh mesh;
        [SerializeField] private MeshFilter meshFilter;
        [SerializeField] private MeshRenderer meshRenderer;
        private float timer = 0f;
        private float alpha = 1f;
        private float fadeTimer = 0f;
        private float burstTimer = 0f;
        private bool isBurstActive = false;
        private bool isFadingOut = false;

        private List<Vector3> vertices = new List<Vector3>();
        private List<int> triangles = new List<int>();
        private List<float> currentAngles = new List<float>();
        private List<float> targetAngles = new List<float>();

        public void Init()
        {
            mesh = new Mesh();
            meshFilter.mesh = mesh;
            if (lightningMaterial != null)
            {
                meshRenderer.material = new Material(lightningMaterial);
            }
            meshRenderer.enabled = false;

            for (int i = 0; i < rayCount; i++)
            {
                float angle = Random.Range(0f, 360f);
                currentAngles.Add(angle);
                targetAngles.Add(angle);
            }
        }

        public void Execute(float duration)
        {
            burstDuration = duration;
            isBurstActive = true;
            isFadingOut = false;
            burstTimer = 0f;
            fadeTimer = 0f;
            alpha = 1f;
            meshRenderer.enabled = true;
        }
        
        private void Update()
        {
            if (isBurstActive)
            {
                burstTimer += Time.deltaTime;
                if (burstTimer >= burstDuration)
                {
                    isBurstActive = false;
                    isFadingOut = true;
                    fadeTimer = 0f;
                }
            }

            if (isFadingOut)
            {
                fadeTimer += Time.deltaTime;
                alpha = Mathf.Lerp(1f, 0f, fadeTimer / fadeDuration);

                Color currentColor = meshRenderer.material.color;
                currentColor.a = alpha;
                meshRenderer.material.color = currentColor;

                if (fadeTimer >= fadeDuration)
                {
                    isFadingOut = false;
                    meshRenderer.enabled = false;
                }
            }

            if (!isBurstActive && !isFadingOut)
            {
                RadialLightningPool.Instance.Release(this);
                return;
            }

            timer += Time.deltaTime;
            if (timer >= updateInterval)
            {
                for (int i = 0; i < rayCount; i++)
                {
                    targetAngles[i] = Random.Range(0f, 360f);
                }
                timer = 0f;
            }

            for (int i = 0; i < rayCount; i++)
            {
                currentAngles[i] = Mathf.LerpAngle(currentAngles[i], targetAngles[i], Time.deltaTime / updateInterval);
            }

            GenerateRadialLightning();
        }

        void GenerateRadialLightning()
        {
            vertices.Clear();
            triangles.Clear();

            for (int r = 0; r < rayCount; r++)
            {
                Quaternion rotation = Quaternion.Euler(0, 0, currentAngles[r]);
                int startIndex = vertices.Count;

                for (int i = 0; i <= segments; i++)
                {
                    float t = i / (float)segments;
                    float y = t * length;

                    float offset = (i == 0 || i == segments) ? 0 : Random.Range(-jaggedness, jaggedness);
                    Vector3 center = new Vector3(offset, y, 0);
                    Vector3 left = center + Vector3.left * width * 0.5f;
                    Vector3 right = center + Vector3.right * width * 0.5f;

                    vertices.Add(rotation * left);
                    vertices.Add(rotation * right);

                    if (i < segments)
                    {
                        int bi = startIndex + i * 2;
                        triangles.Add(bi);
                        triangles.Add(bi + 2);
                        triangles.Add(bi + 1);
                        triangles.Add(bi + 2);
                        triangles.Add(bi + 3);
                        triangles.Add(bi + 1);
                    }
                }
            }

            mesh.Clear();
            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);
        }
    }
}