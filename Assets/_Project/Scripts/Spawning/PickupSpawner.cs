using System.Collections.Generic;
using AIDirectorArena.Pickups;
using AIDirectorArena.Player;
using UnityEngine;

namespace AIDirectorArena.Spawning
{
    public class PickupSpawner : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private HealthPickup healthPickupPrefab;
        [SerializeField] private List<SpawnPoint> spawnPoints = new();

        [Header("Spawn Settings")]
        [SerializeField] private float initialDelay = 5f;
        [SerializeField] private float spawnInterval = 10f;
        [SerializeField] private int maxActivePickups = 1;
        [SerializeField] private float minimumDistanceFromPlayer = 2.5f;
        [SerializeField] private bool stopSpawningWhenPlayerIsDead = true;
        [SerializeField] private bool onlySpawnWhenPlayerNeedsHealing = true;

        private readonly List<HealthPickup> activePickups = new();

        private Transform playerTransform;
        private PlayerHealth playerHealth;
        private float nextSpawnTime;

        public int CurrentActivePickupCount => activePickups.Count;
        public float CurrentSpawnInterval => spawnInterval;
        public int CurrentMaxActivePickups => maxActivePickups;

        private void Start()
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                playerTransform = playerObject.transform;
                playerHealth = playerObject.GetComponent<PlayerHealth>();
            }
            else
            {
                Debug.LogWarning("PickupSpawner não encontrou um objeto com a tag Player.");
            }

            nextSpawnTime = Time.time + initialDelay;
        }

        private void Update()
        {
            CleanupCollectedPickups();

            if (stopSpawningWhenPlayerIsDead && playerHealth != null && playerHealth.IsDead)
            {
                return;
            }

            if (onlySpawnWhenPlayerNeedsHealing &&
                playerHealth != null &&
                playerHealth.CurrentHealth >= playerHealth.MaxHealth)
            {
                return;
            }

            if (Time.time < nextSpawnTime)
            {
                return;
            }

            TrySpawnPickup();
            nextSpawnTime = Time.time + spawnInterval;
        }

        public void SetSpawnInterval(float newInterval)
        {
            spawnInterval = Mathf.Max(1f, newInterval);
        }

        public void SetMaxActivePickups(int newMax)
        {
            maxActivePickups = Mathf.Max(0, newMax);
        }

        private void TrySpawnPickup()
        {
            if (healthPickupPrefab == null)
            {
                Debug.LogWarning("PickupSpawner está sem Health Pickup Prefab.");
                return;
            }

            if (spawnPoints == null || spawnPoints.Count == 0)
            {
                Debug.LogWarning("PickupSpawner não possui SpawnPoints configurados.");
                return;
            }

            if (maxActivePickups <= 0)
            {
                return;
            }

            if (activePickups.Count >= maxActivePickups)
            {
                return;
            }

            SpawnPoint selectedPoint = GetRandomValidSpawnPoint();

            if (selectedPoint == null)
            {
                return;
            }

            Vector2 randomOffset = Random.insideUnitCircle * 0.4f;
            Vector3 spawnPosition = selectedPoint.Position + (Vector3)randomOffset;

            HealthPickup pickupInstance = Instantiate(
                healthPickupPrefab,
                spawnPosition,
                Quaternion.identity);

            activePickups.Add(pickupInstance);
        }

        private SpawnPoint GetRandomValidSpawnPoint()
        {
            List<SpawnPoint> validPoints = new();

            for (int i = 0; i < spawnPoints.Count; i++)
            {
                SpawnPoint point = spawnPoints[i];

                if (point == null)
                {
                    continue;
                }

                if (playerTransform == null)
                {
                    validPoints.Add(point);
                    continue;
                }

                float distanceToPlayer = Vector2.Distance(point.Position, playerTransform.position);

                if (distanceToPlayer >= minimumDistanceFromPlayer)
                {
                    validPoints.Add(point);
                }
            }

            if (validPoints.Count == 0)
            {
                return null;
            }

            int randomIndex = Random.Range(0, validPoints.Count);
            return validPoints[randomIndex];
        }

        private void CleanupCollectedPickups()
        {
            for (int i = activePickups.Count - 1; i >= 0; i--)
            {
                if (activePickups[i] == null)
                {
                    activePickups.RemoveAt(i);
                }
            }
        }
    }
}