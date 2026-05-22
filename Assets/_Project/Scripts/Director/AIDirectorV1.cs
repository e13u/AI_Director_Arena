using AIDirectorArena.Enemies;
using AIDirectorArena.Player;
using AIDirectorArena.Spawning;
using UnityEngine;

namespace AIDirectorArena.Director
{
    public class AIDirectorV1 : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerHealth playerHealth;
        [SerializeField] private SpawnManager spawnManager;
        [SerializeField] private PickupSpawner pickupSpawner;

        [Header("Evaluation")]
        [SerializeField] private float evaluationInterval = 1f;
        [SerializeField] private float recentKillWindowDuration = 8f;

        [Header("Thresholds")]
        [SerializeField] private float reliefHealthThreshold = 0.20f;
        [SerializeField] private float pressureHealthThreshold = 0.60f;
        [SerializeField] private int pressureRecentKillsThreshold = 2;

        [Header("Relief Settings - Enemies")]
        [SerializeField] private float reliefSpawnInterval = 2.3f;
        [SerializeField] private int reliefMaxAlive = 3;

        [Header("Flow Settings - Enemies")]
        [SerializeField] private float flowSpawnInterval = 1.6f;
        [SerializeField] private int flowMaxAlive = 5;

        [Header("Pressure Settings - Enemies")]
        [SerializeField] private float pressureSpawnInterval = 0.75f;
        [SerializeField] private int pressureMaxAlive = 10;

        [Header("Relief Settings - Pickups")]
        [SerializeField] private float reliefPickupSpawnInterval = 6f;
        [SerializeField] private int reliefMaxActivePickups = 2;

        [Header("Flow Settings - Pickups")]
        [SerializeField] private float flowPickupSpawnInterval = 12f;
        [SerializeField] private int flowMaxActivePickups = 1;

        [Header("Pressure Settings - Pickups")]
        [SerializeField] private float pressurePickupSpawnInterval = 20f;
        [SerializeField] private int pressureMaxActivePickups = 1;

        public DirectorState CurrentState { get; private set; } = DirectorState.Flow;
        public int RecentKills { get; private set; }
        public string CurrentReason { get; private set; } = "Initial flow state";

        private float evaluationTimer;
        private float recentKillTimer;

        private void OnEnable()
        {
            EnemyGruntAI.OnAnyEnemyKilled += HandleEnemyKilled;
        }

        private void Start()
        {
            ApplyState(CurrentState);
        }

        private void OnDisable()
        {
            EnemyGruntAI.OnAnyEnemyKilled -= HandleEnemyKilled;
        }

        private void Update()
        {
            if (playerHealth == null || spawnManager == null)
            {
                return;
            }

            if (playerHealth.IsDead)
            {
                return;
            }

            evaluationTimer += Time.deltaTime;
            recentKillTimer += Time.deltaTime;

            if (recentKillTimer >= recentKillWindowDuration)
            {
                RecentKills = 0;
                recentKillTimer = 0f;
            }

            if (evaluationTimer >= evaluationInterval)
            {
                evaluationTimer = 0f;
                EvaluateState();
            }
        }

        private void HandleEnemyKilled()
        {
            RecentKills++;
        }

        private void EvaluateState()
        {
            float healthRatio = playerHealth.CurrentHealth / playerHealth.MaxHealth;
            DirectorState newState = DirectorState.Flow;
            string reason = "Balanced pace";

            if (healthRatio <= reliefHealthThreshold)
            {
                newState = DirectorState.Relief;
                reason = "Low player health";
            }
            else if (healthRatio >= pressureHealthThreshold &&
                     RecentKills >= pressureRecentKillsThreshold)
            {
                newState = DirectorState.Pressure;
                reason = "High health and strong recent kills";
            }

            CurrentReason = reason;

            if (newState != CurrentState)
            {
                CurrentState = newState;
                ApplyState(CurrentState);
            }
            else
            {
                // Mantém parâmetros sincronizados mesmo sem trocar de estado.
                ApplyState(CurrentState);
            }
        }

        private void ApplyState(DirectorState state)
        {
            switch (state)
            {
                case DirectorState.Relief:
                    spawnManager.SetSpawnInterval(reliefSpawnInterval);
                    spawnManager.SetMaxAliveEnemies(reliefMaxAlive);

                    if (pickupSpawner != null)
                    {
                        pickupSpawner.SetSpawnInterval(reliefPickupSpawnInterval);
                        pickupSpawner.SetMaxActivePickups(reliefMaxActivePickups);
                    }
                    break;

                case DirectorState.Pressure:
                    spawnManager.SetSpawnInterval(pressureSpawnInterval);
                    spawnManager.SetMaxAliveEnemies(pressureMaxAlive);

                    if (pickupSpawner != null)
                    {
                        pickupSpawner.SetSpawnInterval(pressurePickupSpawnInterval);
                        pickupSpawner.SetMaxActivePickups(pressureMaxActivePickups);
                    }
                    break;

                default:
                    spawnManager.SetSpawnInterval(flowSpawnInterval);
                    spawnManager.SetMaxAliveEnemies(flowMaxAlive);

                    if (pickupSpawner != null)
                    {
                        pickupSpawner.SetSpawnInterval(flowPickupSpawnInterval);
                        pickupSpawner.SetMaxActivePickups(flowMaxActivePickups);
                    }
                    break;
            }

            Debug.Log(
                $"[AI Director] State: {CurrentState} | " +
                $"Reason: {CurrentReason} | " +
                $"RecentKills: {RecentKills} | " +
                $"Enemy SpawnInterval: {spawnManager.CurrentSpawnInterval} | " +
                $"Enemy MaxAlive: {spawnManager.CurrentMaxAliveEnemies}" +
                $"{GetPickupDebugInfo()}");
        }

        private string GetPickupDebugInfo()
        {
            if (pickupSpawner == null)
            {
                return " | PickupSpawner: none";
            }

            return
                $" | Pickup SpawnInterval: {pickupSpawner.CurrentSpawnInterval}" +
                $" | Pickup MaxActive: {pickupSpawner.CurrentMaxActivePickups}" +
                $" | Pickup ActiveNow: {pickupSpawner.CurrentActivePickupCount}";
        }
    }
}