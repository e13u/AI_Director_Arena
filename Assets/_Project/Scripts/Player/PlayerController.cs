using UnityEngine;

namespace AIDirectorArena.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 6f;

        private Rigidbody2D rb;
        private Vector2 movementInput;
        private Vector2 mouseWorldPosition;
        private PlayerHealth playerHealth;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            playerHealth = GetComponent<PlayerHealth>();
        }

        private void Update()
        {
            if (playerHealth != null && playerHealth.IsDead)
            {   
                movementInput = Vector2.zero;
                return;
            }
            HandleInput();
            RotateToMouse();
        }

        private void FixedUpdate()
        {
            if (playerHealth != null && playerHealth.IsDead){
                return;
            }
            Move();
        }

        private void HandleInput()
        {
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveY = Input.GetAxisRaw("Vertical");

            movementInput = new Vector2(moveX, moveY).normalized;

            Vector3 mouseScreenPosition = Input.mousePosition;
            mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        }

        private void Move()
        {
            Vector2 targetPosition = rb.position + movementInput * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(targetPosition);
        }

        private void RotateToMouse()
        {
            Vector2 direction = mouseWorldPosition - (Vector2)transform.position;

            if (direction.sqrMagnitude < 0.001f)
            {
                return;
            }

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            rb.rotation = angle;
        }
    }
}