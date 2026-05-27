using AIDirectorArena.Player;
using UnityEngine;
using UnityEngine.UI;

namespace AIDirectorArena.UI
{
    public class PlayerHealthBarUI : MonoBehaviour
    {
        [SerializeField] private PlayerHealth playerHealth;
        [SerializeField] private Image fillImage;

        [Header("Colors")]
        [SerializeField] private Color highHealthColor = Color.green;
        [SerializeField] private Color mediumHealthColor = Color.yellow;
        [SerializeField] private Color lowHealthColor = Color.red;

        [Header("Thresholds")]
        [SerializeField] private float mediumHealthThreshold = 0.6f;
        [SerializeField] private float lowHealthThreshold = 0.3f;

        private void Start()
        {
            if (playerHealth == null)
            {
                Debug.LogWarning("PlayerHealthBarUI está sem referência para PlayerHealth.");
                return;
            }

            if (fillImage == null)
            {
                Debug.LogWarning("PlayerHealthBarUI está sem referência para Fill Image.");
                return;
            }

            playerHealth.OnHealthChanged += UpdateHealthBar;
            playerHealth.OnDied += HandlePlayerDied;

            UpdateHealthBar(playerHealth.CurrentHealth, playerHealth.MaxHealth);
        }

        private void OnDestroy()
        {
            if (playerHealth == null)
            {
                return;
            }

            playerHealth.OnHealthChanged -= UpdateHealthBar;
            playerHealth.OnDied -= HandlePlayerDied;
        }

        private void UpdateHealthBar(float currentHealth, float maxHealth)
        {
            if (fillImage == null || maxHealth <= 0f)
            {
                return;
            }

            float normalizedHealth = Mathf.Clamp01(currentHealth / maxHealth);
            fillImage.fillAmount = normalizedHealth;
            fillImage.color = GetColorForHealth(normalizedHealth);
        }

        private void HandlePlayerDied()
        {
            if (fillImage == null)
            {
                return;
            }

            fillImage.fillAmount = 0f;
            fillImage.color = lowHealthColor;
        }

        private Color GetColorForHealth(float normalizedHealth)
        {
            if (normalizedHealth <= lowHealthThreshold)
            {
                return lowHealthColor;
            }

            if (normalizedHealth <= mediumHealthThreshold)
            {
                return mediumHealthColor;
            }

            return highHealthColor;
        }
    }
}