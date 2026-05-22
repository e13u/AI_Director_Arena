using System.Collections;
using AIDirectorArena.Combat;
using UnityEngine;

namespace AIDirectorArena.Enemies
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class EnemyDummy : MonoBehaviour, IDamageable
    {
        [SerializeField] private float maxHealth = 5f;
        [SerializeField] private Color hitColor = Color.red;
        [SerializeField] private float hitFlashDuration = 0.08f;

        private float currentHealth;
        private SpriteRenderer spriteRenderer;
        private Color originalColor;
        private Coroutine hitFlashRoutine;

        private void Awake()
        {
            currentHealth = maxHealth;
            spriteRenderer = GetComponent<SpriteRenderer>();
            originalColor = spriteRenderer.color;
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
            Destroy(gameObject);
        }
    }
}