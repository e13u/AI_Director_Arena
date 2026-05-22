using AIDirectorArena.Player;
using UnityEngine;

namespace AIDirectorArena.Pickups
{
    public class HealthPickup : MonoBehaviour
    {
        [SerializeField] private float healAmount = 3f;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player"))
            {
                return;
            }

            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth == null)
            {
                return;
            }

            if (playerHealth.IsDead)
            {
                return;
            }

            if (playerHealth.CurrentHealth >= playerHealth.MaxHealth)
            {
                return;
            }

            playerHealth.Heal(healAmount);
            Destroy(gameObject);
        }
    }
}