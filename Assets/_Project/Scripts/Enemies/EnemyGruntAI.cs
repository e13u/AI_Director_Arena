using System;
using System.Collections;
using AIDirectorArena.Combat;
using AIDirectorArena.Player;
using UnityEngine;

namespace AIDirectorArena.Enemies
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class EnemyGruntAI : MonoBehaviour, IDamageable
    {
        public static event Action OnAnyEnemyKilled;

        [Header("Stats")]
        [SerializeField] private float maxHealth = 5f;
        [SerializeField] private float moveSpeed = 2.5f;
        [SerializeField] private float stoppingDistance = 0.05f;

        [Header("Contact Damage")]
        [SerializeField] private float contactDamage = 1f;
        [SerializeField] private float damageInterval = 1f;

        [Header("Hit Feedback")]
        [SerializeField] private Color hitColor = Color.red;
        [SerializeField] private float hitFlashDuration = 0.08f;

        private float currentHealth;
        private Transform target;
        private Rigidbody2D rb;
        private SpriteRenderer spriteRenderer;
        private Color originalColor;
        private Coroutine hitFlashRoutine;
        private float nextDamageTime;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();

            currentHealth = maxHealth;
            originalColor = spriteRenderer.color;
        }

        private void Start()
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                target = playerObject.transform;
            }
            else
            {
                Debug.LogWarning("EnemyGruntAI não encontrou um objeto com a tag Player.");
            }
        }

        private void FixedUpdate()
        {
            FollowTarget();
        }

        private void FollowTarget()
        {
            if (target == null)
            {
                return;
            }

            Vector2 currentPosition = rb.position;
            Vector2 targetPosition = target.position;
            Vector2 direction = targetPosition - currentPosition;

            float distance = direction.magnitude;

            if (distance <= stoppingDistance)
            {
                return;
            }

            Vector2 moveDirection = direction.normalized;
            Vector2 nextPosition = currentPosition + moveDirection * moveSpeed * Time.fixedDeltaTime;

            rb.MovePosition(nextPosition);
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (!collision.gameObject.CompareTag("Player"))
            {
                return;
            }

            if (Time.time < nextDamageTime)
            {
                return;
            }

            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth == null)
            {
                return;
            }

            playerHealth.TakeDamage(contactDamage);
            nextDamageTime = Time.time + damageInterval;
        }

        public void TakeDamage(float damage)
        {
            currentHealth -= damage;

            if (hitFlashRoutine != null)
            {
                StopCoroutine(hitFlashRoutine);
            }

            hitFlashRoutine = StartCoroutine(HitFlashRoutine());

            Debug.Log($"{gameObject.name} recebeu {damage} de dano. Vida restante: {currentHealth}");

            if (currentHealth <= 0f)
            {
                Die();
            }
        }

        private IEnumerator HitFlashRoutine()
        {
            spriteRenderer.color = hitColor;
            yield return new WaitForSeconds(hitFlashDuration);
            spriteRenderer.color = originalColor;
        }

        private void Die()
        {
            OnAnyEnemyKilled?.Invoke();
            Destroy(gameObject);
        }
    }
}