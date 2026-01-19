using System.Linq;
using Assets.Scripts.Bullets;
using Assets.Scripts.Game.UIElements;
using DG.Tweening; // Zostawiam, jeśli używasz gdzieś indziej, choć tutaj zmienimy logikę obrotu na Update
using Game.StatsPanel;
using UnityEngine;

namespace Game.MainCannon
{
    public class AutoCannonController : MonoBehaviour
    {
        [Header("Barrel")] 
        [SerializeField] private Transform barrelTransform;

        [Header("Shoot Settings")] 
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private Transform firePoint;
        [SerializeField] private float bulletSpeed = 10f;
        [SerializeField] private float rotationSpeed = 200f; // Prędkość obrotu w stopniach na sekundę
        [SerializeField] private float angleTolerance = 5f; // Jak dokładnie musi być wycelowane, żeby strzelić
        [SerializeField] private float detectionRange = 10f; // Zasięg wykrywania wrogów

        [Header("Multishot Settings")]
        [SerializeField] private int projectileCount = 1; 
        [SerializeField] private float projectileSpacing = 0.5f;

        [Header("Cooldown")]
        [SerializeField] private UIBar cooldownBar; // Opcjonalne, jeśli chcesz widzieć pasek ładowania nad wieżyczką
        [SerializeField] private float cooldownDelay = 0.5f;
        private float cooldownTimer = 0f;

        [Header("Audio")]
        [SerializeField] private AudioSource shootSound;

        [Header("Enemies")] 
        [SerializeField] private GameObject enemyContainer;

        private Transform currentTarget;

        private void Start()
        {
            enemyContainer = GameObject.Find("EnemiesContainer");
            // Inicjalizacja paska cooldownu (jeśli jest przypięty)
            if (cooldownBar != null)
            {
                cooldownBar.SetMaxAmount(cooldownDelay);
                cooldownBar.SetAmount(cooldownDelay);
            }
            
            // Ustawiamy timer na gotowy do strzału od razu
            cooldownTimer = cooldownDelay;
        }

        private void Update()
        {
            HandleCooldown();
            FindAndTrackTarget();
        }
        
        private void HandleCooldown()
        {
            if (cooldownTimer < cooldownDelay)
            {
                cooldownTimer += Time.deltaTime;
                if (cooldownBar != null)
                {
                    cooldownBar.SetAmount(cooldownTimer);
                }
            }
        }

        private void FindAndTrackTarget()
        {
            // 1. Znajdź cel
            if (currentTarget == null || !currentTarget.gameObject.activeInHierarchy || Vector3.Distance(transform.position, currentTarget.position) > detectionRange)
            {
                currentTarget = FindClosestEnemy();
            }

            // Jeśli nadal nie ma celu, nic nie rób
            if (currentTarget == null) return;

            // 2. Obliczanie kierunku
            Vector3 direction = currentTarget.position - barrelTransform.position;
            direction.z = 0; // Upewniamy się, że działamy w 2D

            // Oblicz kąt (zakładamy, że sprite lufy jest skierowany w górę lub w prawo - tutaj dostosowane do Twojego Atan2 - 90)
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);

            // 3. Płynny obrót w stronę celu (w Update lepiej używać RotateTowards niż Tweenów)
            barrelTransform.rotation = Quaternion.RotateTowards(barrelTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // 4. Sprawdzenie czy wycelowaliśmy
            float angleDelta = Quaternion.Angle(barrelTransform.rotation, targetRotation);

            if (angleDelta < angleTolerance)
            {
                TryShoot(direction.normalized, targetAngle);
            }
        }

        private void TryShoot(Vector3 direction, float angle)
        {
            // Sprawdzamy cooldown
            if (cooldownTimer < cooldownDelay) return;

            // Strzał!
            cooldownTimer = 0f;
            if (cooldownBar != null) cooldownBar.SetAmount(0f);

            if (shootSound != null) shootSound.Play();

            // Obliczamy wektor "w prawo" względem kierunku strzału, żeby rozsunąć pociski na boki
            Vector3 rightVector = new Vector3(direction.y, -direction.x, 0).normalized;
            
            // Obliczamy pozycję startową (najbardziej na lewo), żeby całość była wycentrowana względem lufy
            float totalWidth = (projectileCount - 1) * projectileSpacing;
            Vector3 startOffset = -rightVector * (totalWidth / 2f);

            // PĘTLA TWORZĄCA POCISKI
            for (int i = 0; i < projectileCount; i++)
            {
                // Oblicz pozycję konkretnego pocisku
                Vector3 spawnOffset = startOffset + (rightVector * (i * projectileSpacing));
                Vector3 spawnPosition = firePoint.position + spawnOffset;

                GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.Euler(0, 0, angle));
                
                projectile.GetComponent<BulletCollisionDetection>().Initialize(gameObject, GetDamageProperty(), direction);
                
                Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.linearVelocity = direction * bulletSpeed;
                }
            }
        }

        private Transform FindClosestEnemy()
        {
            if (enemyContainer == null) return null;

            Transform closestEnemy = null;
            float closestDistance = detectionRange; // Szukamy tylko w zasięgu

            foreach (Transform enemy in enemyContainer.transform)
            {
                if (enemy == null || !enemy.gameObject.activeInHierarchy) continue;

                float dist = Vector3.Distance(transform.position, enemy.position);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closestEnemy = enemy;
                }
            }

            return closestEnemy;
        }

        // Pobieranie obrażeń ze statystyk jednostki (tak jak w oryginale)
        private int GetDamageProperty()
        {
            var unit = GetComponent<Unit>();
            if (unit == null) return 10; // Wartość domyślna jeśli nie ma komponentu Unit

            var stats = unit.GetStats();
            if (stats == null || stats.properties == null) return 10;

            int damage = (int)(stats.properties
                .FirstOrDefault(p => p.propertyType == StatType.Damage)?.propertyValue ?? 0);
            return damage;
        }
        
        // Wizualizacja zasięgu w edytorze (Gizmos)
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectionRange);
        }
    }
}