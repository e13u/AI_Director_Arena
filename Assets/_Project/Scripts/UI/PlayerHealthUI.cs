using AIDirectorArena.Player;
using TMPro;
using UnityEngine;

namespace AIDirectorArena.UI
{
    public class PlayerHealthUI : MonoBehaviour
    {
        [SerializeField] private PlayerHealth playerHealth;
        [SerializeField] private TMP_Text healthText;

        private void Start()
        {
            if (playerHealth == null)
            {
                Debug.LogWarning("PlayerHealthUI está sem referência para PlayerHealth.");
                return;
            }

            playerHealth.OnHealthChanged += UpdateHealthText;
            playerHealth.OnDied += HandlePlayerDied;

            UpdateHealthText(playerHealth.CurrentHealth, playerHealth.MaxHealth);
        }

        private void OnDestroy()
        {
            if (playerHealth == null)
            {
                return;
            }

            playerHealth.OnHealthChanged -= UpdateHealthText;
            playerHealth.OnDied -= HandlePlayerDied;
        }

        private void UpdateHealthText(float currentHealth, float maxHealth)
        {
            if (healthText == null)
            {
                return;
            }

            healthText.text = $"HP: {Mathf.CeilToInt(currentHealth)} / {Mathf.CeilToInt(maxHealth)}";
        }

        private void HandlePlayerDied()
        {
            if (healthText == null)
            {
                return;
            }

            healthText.text = "HP: 0 / 0 - GAME OVER";
        }
    }
}