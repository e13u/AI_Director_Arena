using UnityEngine;

namespace AIDirectorArena.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float speed = 14f;
        [SerializeField] private float lifetime = 2f;

        private Vector2 direction;
        private float damage;
        private bool initialized;
        private bool hasHitSomething;

        public void Initialize(Vector2 shootDirection, float projectileDamage)
        {
            direction = shootDirection.normalized;
            damage = projectileDamage;
            initialized = true;
            hasHitSomething = false;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);

            Destroy(gameObject, lifetime);
        }

        private void Update()
        {
            if (!initialized || hasHitSomething)
            {
                return;
            }

            transform.position += (Vector3)(direction * speed * Time.deltaTime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (hasHitSomething)
            {
                return;
            }

            if (other.CompareTag("Player"))
            {
                return;
            }

            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }

            hasHitSomething = true;
            Destroy(gameObject);
        }
    }
}