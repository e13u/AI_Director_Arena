using AIDirectorArena.Player;
using TMPro;
using UnityEngine;

namespace AIDirectorArena.UI
{
    public class SurvivalTimerUI : MonoBehaviour
    {
        [SerializeField] private PlayerHealth playerHealth;
        [SerializeField] private TMP_Text timeText;
        [SerializeField] private string prefix = "Time: ";

        public float SurvivedTime { get; private set; }

        private void Start()
        {
            UpdateTimeText();
        }

        private void Update()
        {
            if (playerHealth != null && playerHealth.IsDead)
            {
                return;
            }

            SurvivedTime += Time.deltaTime;
            UpdateTimeText();
        }

        public string GetFormattedTime()
        {
            int totalSeconds = Mathf.FloorToInt(SurvivedTime);
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;

            return $"{minutes:00}:{seconds:00}";
        }

        private void UpdateTimeText()
        {
            if (timeText == null)
            {
                return;
            }

            timeText.text = $"{prefix}{GetFormattedTime()}";
        }
    }
}