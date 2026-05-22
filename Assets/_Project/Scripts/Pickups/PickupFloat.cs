using UnityEngine;

namespace AIDirectorArena.Pickups
{
    public class PickupFloat : MonoBehaviour
    {
        [SerializeField] private float amplitude = 0.1f;
        [SerializeField] private float frequency = 2f;

        private Vector3 startPosition;

        private void Start()
        {
            startPosition = transform.position;
        }

        private void Update()
        {
            float offsetY = Mathf.Sin(Time.time * frequency) * amplitude;
            transform.position = startPosition + new Vector3(0f, offsetY, 0f);
        }
    }
}