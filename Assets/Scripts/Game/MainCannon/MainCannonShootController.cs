using System.Linq;
using Assets.Scripts.Bullets;
using Assets.Scripts.Game.UIElements;
using DG.Tweening;
using Game.StatsPanel;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.MainCannon
{
    public class MainCannonShootController : MonoBehaviour
    {
        [Header("Input")] [SerializeField] private InputActionReference fireActionRef;

        [Header("Barrel")] [SerializeField] private Transform barrelTransform;

        [Header("Shoot")] [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private Transform firePoint;
        [SerializeField] private float bulletSpeed = 10f;
        [SerializeField] private float rotateAnimationDuration = 0.1f;
        [SerializeField] private float angleTolerance = 1f;

        [Header("Colldown")]
        [SerializeField] private UIBar cooldownBar;
        [SerializeField] private float cooldownDelay = 0.2f;
        private float cooldownTimer = 0f;
        private bool isCooldownOn = false;

        [Header("Click detection zone")]
        [SerializeField] private Collider2D clickAreaCollider;
		[SerializeField] private AudioSource shootSound;


		[Header("Click detection zone")] [SerializeField]
        private GameObject clickArea;

        [Header("Enemies")] [SerializeField] private GameObject enemyContainer;

        private bool enemyHit = false;
        private bool isRotating = false;
        
        private Tween _rotationTween;
        private Camera mainCamera;
        private Vector3 mouseWorldPos;
        private Transform currentTarget;

        private StatsData stats; 
        private void OnEnable()
        {
            fireActionRef.action.performed += MousePositionFire;
            fireActionRef.action.Enable();
        }

        private void OnDisable()
        {
            fireActionRef.action.performed -= MousePositionFire;
            fireActionRef.action.Disable();
        }

        private void Start()
        {
            mainCamera = Camera.main;
            stats = GetComponent<Unit>().GetStats();
            cooldownBar.SetMaxAmount(cooldownDelay);
            cooldownBar.SetAmount(cooldownDelay);
        }

        private void Update()
        {
            if (isCooldownOn)
            {
                cooldownTimer += Time.deltaTime;

                cooldownBar.SetAmount(cooldownTimer);

                if (cooldownTimer >= cooldownDelay)
                {
                    isCooldownOn=false;
                    cooldownTimer = 0f;

                    cooldownBar.SetAmount(cooldownDelay);
                }
            }
        }
        private Vector3 GetMouseWorldPosition()
        {
            Vector3 mousePos = Mouse.current.position.ReadValue();
            mousePos.z = barrelTransform.position.z - mainCamera.transform.position.z;
            Vector3 world = mainCamera.ScreenToWorldPoint(mousePos);
            world.z = barrelTransform.position.z;
            return world;
        }
        private void MousePositionFire(InputAction.CallbackContext context)
        {


            if (!IsClickWithinArea()) return;
            if (isRotating) return;
            //auto track
            if (!enemyHit)
            {
                AutoTrackingFire();
                return;
            }
            mouseWorldPos = GetMouseWorldPosition();
            Vector3 direction = (mouseWorldPos - barrelTransform.position);
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            float currentAngle = barrelTransform.eulerAngles.z; // current Barrel rotation angle
            float angleDelta = Mathf.DeltaAngle(currentAngle, targetAngle);

            if (Mathf.Abs(angleDelta) < angleTolerance)
            {
                // Fire instantly if angle difference is too small
                Fire(direction, targetAngle);
            }
            else
            {
                RotateAndFire(direction, targetAngle);
            }
        }

        private void OnDestroy()
        {
            _rotationTween?.Kill();
        }
        //Spawns bullet at correct direction
        private void Fire(Vector3 direction, float angle)
        {
            // cooldown
            if (isCooldownOn) return;
            isCooldownOn = true;

            if (shootSound != null)
            {
                shootSound.Play();
			}

			direction.z = 0;
            direction = direction.normalized;
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.Euler(0,0,angle));
            projectile.GetComponent<BulletCollisionDetection>().Initialize(gameObject, GetDamageProperty(), direction);
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.linearVelocity = direction * bulletSpeed;
        }

        private int GetDamageProperty()
        {
            int damage = (int)(stats.properties
                .FirstOrDefault(p => p.propertyType == StatType.Damage)?.propertyValue ?? 0);
            //return damage;
            if (isCritical()) damage = Mathf.RoundToInt(calculateCrtiDmg(damage));
            return damage;
        }

        private bool isCritical()
        {
            int critChance = (int)(stats.properties
                .FirstOrDefault(p => p.propertyType == StatType.CritChancePct)?.propertyValue ?? 0);
            bool success = Random.Range(0, 100) < critChance;
            //if(success)Debug.Log("Critical hit");
            return success;
        }

        private float calculateCrtiDmg(int baseDmg)
        {
            int critDmg = (int)(stats.properties
                .FirstOrDefault(p => p.propertyType == StatType.CritDmgPct)?.propertyValue ?? 0);
            float criticalDamage = baseDmg * (critDmg / 100f + 1);
            //Debug.Log($"Critical damage: {criticalDamage} (base: {baseDmg})");
            return criticalDamage;
        }
        //Check click area
        private bool IsClickWithinArea()
        {
            mouseWorldPos = GetMouseWorldPosition();
            mouseWorldPos.z = firePoint.position.z;
            Vector2 mouseWorld2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);

            if (clickArea != null)
            {
                var renderer = clickArea.GetComponent<SpriteRenderer>();
                mouseWorldPos.z = renderer.bounds.center.z;
                if (renderer != null && renderer.bounds.Contains(mouseWorldPos))
                {
                    Collider2D hit = Physics2D.OverlapPoint(mouseWorld2D);
                    if (hit != null)
                    {
                        if (hit.CompareTag("Enemy")) enemyHit = true;
                        else enemyHit = false;
                    }
                    else enemyHit = false;
                    return true;
                }
            }
            return false;
        }

        //Auto tracker
        private void AutoTrackingFire()
        {
            if (isRotating) return;
            if (enemyContainer == null) return;
            if (currentTarget == null) //target tracking
            {
                currentTarget = FindClosestEnemy();
                if (currentTarget == null) return;
            }

            Vector3 direction = currentTarget.position - barrelTransform.position;
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            float currentAngle = barrelTransform.eulerAngles.z;
            float angleDelta = Mathf.DeltaAngle(currentAngle, targetAngle);
            if (Mathf.Abs(angleDelta) < angleTolerance)
                Fire(direction, targetAngle);
            else
                RotateAndFire(direction, targetAngle);
        }

        //Enemy finder
        private Transform FindClosestEnemy()
        {
            Transform closestEnemy = null;
            float closestDistance = Mathf.Infinity;

            foreach (Transform enemy in enemyContainer.transform)
            {
                if (enemy == null) continue;

                float dist = Vector3.Distance(transform.position, enemy.position);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closestEnemy = enemy;
                }
            }

            return closestEnemy;
        }

        //Handles rotation animation
        private void RotateAndFire(Vector3 direction, float targetAngle)
        {
            if (isRotating) return;
            isRotating = true;

            _rotationTween?.Kill();
            _rotationTween = barrelTransform
                .DORotate(new Vector3(0, 0, targetAngle), rotateAnimationDuration)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    Fire(direction, targetAngle);
                    isRotating = false; 
                });
        }
    }
}