using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace InGame
{
    public class MoveChargeArcher : MoveChargeController
    {
        [SerializeField] private float spawnSpeed;
        [SerializeField] private float spawnMaxDuration;
        [SerializeField] private float rotateSpeed;

        private List<ProjectileEntity> projectiles;
        private List<Coroutine> spawnCoroutines;

        private void Awake()
        {
            projectiles  = new List<ProjectileEntity>();
            spawnCoroutines = new List<Coroutine>();
        }

        public override void AddBullet(Vector2 spawnPos, Vector2 aimDirection)
        {
            var p = ProjectilePool.Instance.Get(projectilePrefab, null, false);
            p.transform.position = spawnPos;
            p.transform.localScale = Vector3.one;
            // Do đây setactive rồi nhng mà chưa set spawn pos cho projectile nên check trong update bị sai
            // Nên ở đây phải set startpos luoon
            p.RangeCenter = spawnPos;
            p.gameObject.SetActive(true);
            projectiles.Add(p);
            spawnCoroutines.Add(StartCoroutine(IESpawnProjectile(p, aimDirection.normalized)));
        }

        public override void AddSize(float size)
        {
            foreach (var projectile in projectiles)
            {
                projectile.transform.localScale = Vector3.one * size;
            }
        }

        public override void Attack(Action<ProjectileEntity, Vector2> actionSetupProjectile)
        {
            for (var i = 0; i < projectiles.Count; i++)
            {
                StopCoroutine(spawnCoroutines[i]);
                actionSetupProjectile?.Invoke(projectiles[i], Cam.ScreenToWorldPoint(Input.mousePosition) - projectiles[i].transform.position);
            }

            projectiles = new List<ProjectileEntity>();
            spawnCoroutines = new List<Coroutine>();
        }

        private IEnumerator IESpawnProjectile(ProjectileEntity projectile, Vector2 direction)
        {
            var spawnDirection = Quaternion.Euler(0f, 0f, Random.Range(-45f, 45f)) * direction * Random.Range(0.5f, 0.6f);
            var timer = 0f;
            while (timer < spawnMaxDuration)
            {
                timer += Time.deltaTime;
                projectile.transform.position -= spawnSpeed * Time.deltaTime * spawnDirection;
                spawnDirection = Vector3.RotateTowards(spawnDirection, Cam.ScreenToWorldPoint(Input.mousePosition) - projectile.transform.position,
                    Mathf.Deg2Rad * rotateSpeed * Time.deltaTime, 0f);
                projectile.transform.rotation = Quaternion.Euler(0f, 0f,  Mathf.Atan2(spawnDirection.y, spawnDirection.x) * Mathf.Rad2Deg);
                yield return null;
            }

            while (true)
            {
                spawnDirection = Vector3.RotateTowards(spawnDirection, Cam.ScreenToWorldPoint(Input.mousePosition) - projectile.transform.position,
                    Mathf.Deg2Rad * rotateSpeed * Time.deltaTime, 0f);
                projectile.transform.rotation = Quaternion.Euler(0f, 0f,  Mathf.Atan2(spawnDirection.y, spawnDirection.x) * Mathf.Rad2Deg);
                yield return null;
            }
        }
    }
}