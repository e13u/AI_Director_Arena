using System;
using System.Collections;
using UnityEngine;

namespace AIDirectorArena.Player
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class PlayerHealth : MonoBehaviour
    {
        public event Action<float, float> OnHealthChanged;
        public event Action OnDied;

        [SerializeField] private float maxHealth = 10f;
        [SerializeField] private Color hitColor = Color.red;
        [SerializeField] private float hitFlashDuration = 0.08f;

        public float MaxHealth => maxHealth;
        public float CurrentHealth { get; private set; }
        public bool IsDead { get; private set; }

        private SpriteRenderer spriteRenderer;
        private Color originalColor;
        private Coroutine hitFlashRoutine;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            originalColor = spriteRenderer.color;
            CurrentHealth = maxHealth;
        }

        private void Start()
        {
            OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
        }

        public void TakeDamage(float damage)
        {
            if (IsDead || damage <= 0f)
            {
                return;
            }

            CurrentHealth -= damage;
            CurrentHealth = Mathf.Max(CurrentHealth, 0f);

            if (hitFlashRoutine != null)
            {
                StopCoroutine(hitFlashRoutine);
            }

            hitFlashRoutine = StartCoroutine(HitFlashRoutine());

            OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);

            Debug.Log($"Player recebeu {damage} de dano. HP atual: {CurrentHealth}/{MaxHealth}");

            if (CurrentHealth <= 0f)
            {
                Die();
            }
        }

        public void Heal(float amount)
        {
            if (IsDead || amount <= 0f)
            {
                return;
            }

            CurrentHealth += amount;
            CurrentHealth = Mathf.Min(CurrentHealth, maxHealth);

            OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
        }

        private IEnumerator HitFlashRoutine()
        {
            spriteRenderer.color = hitColor;
            yield return new WaitForSeconds(hitFlashDuration);
            spriteRenderer.color = originalColor;
        }

        private void Die()
        {
            if (IsDead)
            {
                return;
            }

            IsDead = true;
            OnDied?.Invoke();

            Debug.Log("Player morreu.");
        }
    }
}