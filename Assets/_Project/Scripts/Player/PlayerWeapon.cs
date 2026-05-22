using AIDirectorArena.Combat;
using UnityEngine;

namespace AIDirectorArena.Player
{
    public class PlayerWeapon : MonoBehaviour
    {
        [SerializeField] private Transform firePoint;
        [SerializeField] private Projectile projectilePrefab;
        [SerializeField] private float fireRate = 5f;
        [SerializeField] private float projectileDamage = 1f;

        private float nextFireTime;
        private PlayerHealth playerHealth;

        private void Awake()
        {
            playerHealth = GetComponent<PlayerHealth>();
        }

        private void Update()
        {
            HandleShooting();
        }

        private void HandleShooting()
        {
            if (playerHealth != null && playerHealth.IsDead)
            {
                return;
            }
            
            if (!Input.GetMouseButton(0))
            {
                return;
            }

            if (Time.time < nextFireTime)
            {
                return;
            }

            Shoot();
            nextFireTime = Time.time + 1f / fireRate;
        }

        private void Shoot()
        {
            if (firePoint == null || projectilePrefab == null)
            {
                Debug.LogWarning("PlayerWeapon is missing firePoint or projectilePrefab.");
                return;
            }

            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 shootDirection = (mouseWorldPosition - firePoint.position);
            //shootDirection.z = 0f;

            Projectile projectileInstance = Instantiate(
                projectilePrefab,
                firePoint.position,
                Quaternion.identity);

            projectileInstance.Initialize(shootDirection, projectileDamage);
        }
    }
}