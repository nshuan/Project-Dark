using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace InGame.Effects
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class Lightning : MonoBehaviour
    {
        public List<Transform> points = new List<Transform>();
        public float jaggedness = 0.3f;
        public float width = 0.05f;
        public int segmentsPerUnit = 4;
        public float flickerSpeed = 0.05f;
        public float lightningDuration = 0.5f;

        private Mesh lightningMesh;
        private float flickerTimer = 0f;
        private float lightningTimer = 0f;
        private bool isAnimating = false;
        
        public void Init()
        {
            lightningMesh = new Mesh();
            GetComponent<MeshFilter>().mesh = lightningMesh;
        }

        public void Execute(float duration)
        {
            isAnimating = true;
            lightningDuration = duration;
            lightningTimer = 0f;
            flickerTimer = 0f;
            GenerateLightningMeshPath();
        }

        public void ForceStop()
        {
            isAnimating = false;
            lightningMesh.Clear(); // Hide lightning when done
            points.Clear();
        }
        
        void Update()
        {
            if (isAnimating)
            {
                flickerTimer += Time.deltaTime;
                lightningTimer += Time.deltaTime;

                if (flickerTimer >= flickerSpeed)
                {
                    flickerTimer = 0f;
                    GenerateLightningMeshPath();
                }

                if (lightningTimer >= lightningDuration)
                {
                    isAnimating = false;
                    lightningMesh.Clear(); // Hide lightning when done
                    points.Clear();
                }
            }
        }

        void GenerateLightningMeshPath()
        {
            lightningMesh.Clear();
            if (points.Count < 2) return;
            if (points.Count((point) => point.gameObject.activeInHierarchy) < 2) return;

            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            List<Vector2> uvs = new List<Vector2>();

            int vertIndex = 0;
            float uvY = 0;

            for (int seg = 0; seg < points.Count - 1; seg++)
            {
                Vector2 p0 = points[seg].position;
                Vector2 p1 = points[seg + 1].position;
                Vector2 direction = (p1 - p0).normalized;
                Vector2 perpendicular = new Vector2(-direction.y, direction.x) * width;

                float segmentLength = Vector2.Distance(p0, p1);
                int segSteps = Mathf.Max(2, Mathf.CeilToInt(segmentLength * segmentsPerUnit));

                for (int i = 0; i < segSteps; i++)
                {
                    float t = (float)i / (segSteps - 1);
                    Vector2 point = Vector2.Lerp(p0, p1, t);

                    float noise = Mathf.PerlinNoise(Time.time * 10f + i + seg * 100f, t * 10f);
                    float offset = (noise - 0.5f) * 2f * jaggedness;
                    Vector2 offsetVec = perpendicular.normalized * offset;

                    point += offsetVec;
                    Vector3 p3 = new Vector3(point.x, point.y, 0);

                    vertices.Add(p3 - (Vector3)perpendicular);
                    vertices.Add(p3 + (Vector3)perpendicular);

                    uvs.Add(new Vector2(0, uvY));
                    uvs.Add(new Vector2(1, uvY));
                    uvY += 1f / (points.Count * segSteps);

                    if (i < segSteps - 1)
                    {
                        triangles.Add(vertIndex);
                        triangles.Add(vertIndex + 1);
                        triangles.Add(vertIndex + 2);

                        triangles.Add(vertIndex + 1);
                        triangles.Add(vertIndex + 3);
                        triangles.Add(vertIndex + 2);

                        vertIndex += 2;
                    }
                }

                vertIndex += 2; // move past last 2 verts
            }

            lightningMesh.vertices = vertices.ToArray();
            lightningMesh.triangles = triangles.ToArray();
            lightningMesh.uv = uvs.ToArray();
            lightningMesh.RecalculateNormals();
        }
    }
}