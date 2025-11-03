using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.MainCannon
{
    public class MainCannonShootController : MonoBehaviour
    {
        [Header("Input")]
        [SerializeField] private InputActionReference fireActionRef;

        [Header("Barrel")]
        [SerializeField] private Transform barrelTransform;

        [Header("Shoot")]
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private Transform firePoint;
        [SerializeField] private float bulletSpeed = 10f;

        [Header("Click detection zone")]
        [SerializeField] private Collider2D clickAreaCollider;
        [SerializeField] private bool useWorldArea = true;

        private Camera mainCamera;

        private void OnEnable()
        {
            fireActionRef.action.performed += OnFire;
            fireActionRef.action.Enable();
        }

        private void OnDisable()
        {
            fireActionRef.action.performed -= OnFire;
            fireActionRef.action.Disable();
        }

        private void Start()
        {
            mainCamera = Camera.main;
        }

        private void OnFire(InputAction.CallbackContext context)
        {
            if (!IsClickWithinArea()) return;

            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mouseWorldPos.z = barrelTransform.position.z;

            Vector3 direction = (mouseWorldPos - barrelTransform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            barrelTransform.rotation = Quaternion.Euler(0, 0, angle - 90f); 

            Fire(direction);
        }

        private void Fire(Vector3 direction)
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = direction * bulletSpeed;
            }
        }

        private bool IsClickWithinArea()
        {
            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector2 mouseWorld2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);

            return clickAreaCollider != null && clickAreaCollider.OverlapPoint(mouseWorld2D);
        }
    }
}
