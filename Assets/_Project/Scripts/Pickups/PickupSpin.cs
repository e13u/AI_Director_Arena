using UnityEngine;

namespace AIDirectorArena.Pickups
{
    public class PickupSpin : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed = 90f;

        private void Update()
        {
            transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
        }
    }
}