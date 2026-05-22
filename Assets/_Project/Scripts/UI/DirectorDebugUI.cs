using AIDirectorArena.Director;
using AIDirectorArena.Player;
using AIDirectorArena.Spawning;
using TMPro;
using UnityEngine;

namespace AIDirectorArena.UI
{
    public class DirectorDebugUI : MonoBehaviour
    {
        [SerializeField] private AIDirectorV1 aiDirector;
        [SerializeField] private SpawnManager spawnManager;
        [SerializeField] private PickupSpawner pickupSpawner;
        [SerializeField] private PlayerHealth playerHealth;
        [SerializeField] private TMP_Text debugText;

        [Header("Options")]
        [SerializeField] private bool useStateColor = true;

        private void Update()
        {
            if (aiDirector == null || spawnManager == null || playerHealth == null || debugText == null)
            {
                return;
            }

            float healthRatio = playerHealth.MaxHealth > 0f
                ? playerHealth.CurrentHealth / playerHealth.MaxHealth
                : 0f;

            string pickupInfo = pickupSpawner == null
                ? "Pickup Spawner: none"
                : $"Pickup Interval: {pickupSpawner.CurrentSpawnInterval:F1}s\n" +
                  $"Pickup Max Active: {pickupSpawner.CurrentMaxActivePickups}\n" +
                  $"Pickup Alive Now: {pickupSpawner.CurrentActivePickupCount}";

            debugText.text =
                $"AI Director\n" +
                $"State: {aiDirector.CurrentState}\n" +
                $"Reason: {aiDirector.CurrentReason}\n" +
                $"Recent Kills: {aiDirector.RecentKills}\n" +
                $"Player HP: {Mathf.CeilToInt(playerHealth.CurrentHealth)} / {Mathf.CeilToInt(playerHealth.MaxHealth)} ({healthRatio:P0})\n" +
                $"Enemy Spawn Interval: {spawnManager.CurrentSpawnInterval:F1}s\n" +
                $"Enemy Max Alive: {spawnManager.CurrentMaxAliveEnemies}\n" +
                $"Enemy Alive Now: {spawnManager.CurrentAliveCount}\n" +
                $"{pickupInfo}";

            if (useStateColor)
            {
                debugText.color = GetColorForState(aiDirector.CurrentState);
            }
        }

        private Color GetColorForState(DirectorState state)
        {
            switch (state)
            {
                case DirectorState.Relief:
                    return Color.green;

                case DirectorState.Pressure:
                    return new Color(1f, 0.4f, 0.4f);

                default:
                    return Color.white;
            }
        }
    }
}