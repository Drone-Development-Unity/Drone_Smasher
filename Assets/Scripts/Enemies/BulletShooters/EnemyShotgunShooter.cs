using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Enemies.BulletShooters
{
    public class EnemyShotgunShooter : EnemyBulletShooter
    {
        [Header("Shotgun Settings")]
        [SerializeField] private int pelletsCount = 5; // Ile pocisków w wachlarzu
        [SerializeField] private float spreadAngle = 30f; // Kąt rozrzutu całego wachlarza

        protected override IEnumerator SpawnBulletsInBurtsCoroutine()
        {
            waitingToShoot = true;

            // Obliczamy kąt początkowy (najbardziej na lewo)
            float angleStep = spreadAngle / (pelletsCount - 1);
            float startAngle = -spreadAngle / 2f;
            
            // Zapamiętujemy oryginalną rotację, żeby jej nie zepsuć na stałe
            Quaternion originalRotation = firePoint.rotation;

            for (int i = 0; i < numberOfBulletsInBurst; i++) // Ilość serii
            {
                // Wystrzelenie całego wachlarza na raz
                for (int j = 0; j < pelletsCount; j++)
                {
                    // Obracamy lufę o odpowiedni kąt dla danego "śrutu"
                    float currentAngle = startAngle + (angleStep * j);
                    firePoint.rotation = originalRotation * Quaternion.Euler(0, 0, currentAngle);
                    
                    SpawnBullet(firePoint);
                }

                // Przywracamy rotację po strzale
                firePoint.rotation = originalRotation;

                yield return new WaitForSeconds(burstDelayBetweenBullets);
            }

            waitingToShoot = false;
        }
    }
}