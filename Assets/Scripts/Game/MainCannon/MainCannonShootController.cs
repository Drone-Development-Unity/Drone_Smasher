using DG.Tweening;
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
        [SerializeField] private float rotateAnimationDuration = 0.1f;
        [SerializeField] private float angleTolerance = 1f;

        [Header("Click detection zone")]
        [SerializeField] private Collider2D clickAreaCollider;

        private Tween _rotationTween;
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
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            // current Barrel rotation angle
            float currentAngle = barrelTransform.eulerAngles.z;
            
            float angleDelta = Mathf.DeltaAngle(currentAngle, targetAngle);

            if (Mathf.Abs(angleDelta) < angleTolerance)
            {
                // Fire instantly if angle difference is too small
                Fire(direction);
            }
            else
            {
                // If ingle is too big shoot after angle change
                _rotationTween?.Kill();
                _rotationTween = barrelTransform
                    .DORotate(new Vector3(0, 0, targetAngle), rotateAnimationDuration)
                    .SetEase(Ease.OutQuad)
                    .OnComplete(() => Fire(direction));
            }
        }
        private void OnDestroy()
        {
            _rotationTween?.Kill();
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
