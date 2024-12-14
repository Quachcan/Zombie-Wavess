using System;
using EnemyScripts;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
    public class PlayerCombat : MonoBehaviour
    {
        [Header("Combat Settings")]
        [SerializeField]
        private Transform firePoint;
        [SerializeField]
        private float fireRate;
        [SerializeField]
        private float bulletSpeed;
        [SerializeField]
        private float bulletLifeTime;
        [SerializeField]
        private float fireCooldown;
        [SerializeField]
        private Transform targetEnemy;
        [SerializeField]
        private BulletPool bulletPool;


        private void Start()
        {
            if (bulletPool == null)
            {
                Debug.Log("No bullet pool assigned");
            }
            bulletPool = BulletPool.Instance;
        }

        // Update is called once per frame
        void Update()
        {
            if (fireCooldown > 0)
            {
                fireCooldown -= Time.deltaTime;
            }
            FindClosetEnemy();
            if (targetEnemy != null)
            {
                RotatePlayerTowardsTarget();
            }

            if (targetEnemy != null && fireCooldown < 0)
            {
                Shoot();
                fireCooldown = fireRate;
            }
        }

        private void FindClosetEnemy()
        {
            Enemy[] enemies = FindObjectsOfType<Enemy>();
            float closetDistance = Mathf.Infinity;
            Transform closetTarget = null;

            foreach (var enemy in enemies)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < closetDistance)
                {
                    closetDistance = distance;
                    closetTarget = enemy.transform;
                }
            }
            targetEnemy = closetTarget;
        }

        private void RotatePlayerTowardsTarget()
        {
            Vector3 direction = (targetEnemy.position - transform.position).normalized;
            direction.y = 0;
            
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
        }

        private void Shoot()
        {
            if (firePoint == null || targetEnemy == null)
            {
                Debug.LogWarning("Cannot Shoot : Missing bullet prefab, target or fire point");
                return;
            }
            
            Vector3 direction = (targetEnemy.position - firePoint.position).normalized;
            
            GameObject bullet = bulletPool.GetBullet();
            if (bullet == null)
            {
                Debug.LogError("BulletPool returned null bullet.");
                return;
            }
            bullet.transform.position = firePoint.position;
            bullet.transform.rotation = Quaternion.LookRotation(direction);
            bullet.SetActive(true);
            
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.SetTarget(targetEnemy);
                StartCoroutine(bulletScript.BulletCoroutineLifeTime(bulletLifeTime));
            }
            
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = direction * bulletSpeed;
            }
            
            Debug.Log("Player shot a bullet to: " + targetEnemy.name);
        }
    }
}
