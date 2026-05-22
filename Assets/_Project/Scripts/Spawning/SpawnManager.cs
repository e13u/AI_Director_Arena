using System.Collections.Generic;
using AIDirectorArena.Enemies;
using AIDirectorArena.Player;
using UnityEngine;

namespace AIDirectorArena.Spawning
{
    public class SpawnManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private EnemyGruntAI enemyPrefab;
        [SerializeField] private List<SpawnPoint> spawnPoints = new();

        [Header("Spawn Settings")]
        [SerializeField] private float initialDelay = 1f;
        [SerializeField] private float spawnInterval = 2f;
        [SerializeField] private int maxAliveEnemies = 6;
        [SerializeField] private float minimumDistanceFromPlayer = 3f;
        [SerializeField] private bool stopSpawningWhenPlayerIsDead = true;

        private readonly List<EnemyGruntAI> aliveEnemies = new();

        private Transform playerTransform;
        private PlayerHealth playerHealth;
        private float nextSpawnTime;

        public float CurrentSpawnInterval => spawnInterval;
        public int CurrentMaxAliveEnemies => maxAliveEnemies;
        public int CurrentAliveCount => aliveEnemies.Count;

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
                Debug.LogWarning("SpawnManager não encontrou um objeto com a tag Player.");
            }

            nextSpawnTime = Time.time + initialDelay;
        }

        private void Update()
        {
            CleanupDeadEnemies();

            if (stopSpawningWhenPlayerIsDead && playerHealth != null && playerHealth.IsDead)
            {
                return;
            }

            if (Time.time < nextSpawnTime)
            {
                return;
            }

            TrySpawnEnemy();
            nextSpawnTime = Time.time + spawnInterval;
        }

        public void SetSpawnInterval(float newInterval)
        {
            spawnInterval = Mathf.Max(0.2f, newInterval);
        }

        public void SetMaxAliveEnemies(int newMaxAlive)
        {
            maxAliveEnemies = Mathf.Max(1, newMaxAlive);
        }

        private void TrySpawnEnemy()
        {
            if (enemyPrefab == null)
            {
                Debug.LogWarning("SpawnManager está sem Enemy Prefab.");
                return;
            }

            if (spawnPoints == null || spawnPoints.Count == 0)
            {
                Debug.LogWarning("SpawnManager não possui SpawnPoints configurados.");
                return;
            }

            if (aliveEnemies.Count >= maxAliveEnemies)
            {
                return;
            }

            SpawnPoint selectedPoint = GetRandomValidSpawnPoint();

            if (selectedPoint == null)
            {
                return;
            }

            Vector2 randomOffset = Random.insideUnitCircle * 0.5f;
            Vector3 spawnPosition = selectedPoint.Position + (Vector3)randomOffset;

            EnemyGruntAI enemyInstance = Instantiate(
                enemyPrefab,
                spawnPosition,
                Quaternion.identity);

            aliveEnemies.Add(enemyInstance);
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

        private void CleanupDeadEnemies()
        {
            for (int i = aliveEnemies.Count - 1; i >= 0; i--)
            {
                if (aliveEnemies[i] == null)
                {
                    aliveEnemies.RemoveAt(i);
                }
            }
        }
    }
}