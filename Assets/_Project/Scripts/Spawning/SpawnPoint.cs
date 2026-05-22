using UnityEngine;

namespace AIDirectorArena.Spawning
{
    public class SpawnPoint : MonoBehaviour
    {
        [SerializeField] private float gizmoRadius = 0.25f;
        [SerializeField] private Color gizmoColor = Color.yellow;

        public Vector3 Position => transform.position;

        private void OnDrawGizmos()
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawSphere(transform.position, gizmoRadius);
        }
    }
}