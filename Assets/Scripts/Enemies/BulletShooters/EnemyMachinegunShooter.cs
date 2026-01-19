using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Enemies.BulletShooters
{
    public class EnemyMachinegunShooter : EnemyBulletShooter
    {
        [Header("Machine Gun Settings")]
        [SerializeField] private float spreadRandomness = 15f; // Jak bardzo trzęsie lufą (w stopniach)

        protected override IEnumerator SpawnBulletsInBurtsCoroutine()
        {
            waitingToShoot = true;

            // Zapamiętujemy, gdzie celował na początku (prosto w dół/w gracza)
            Quaternion originalRotation = firePoint.rotation;

            for (int i = 0; i < numberOfBulletsInBurst; i++)
            {
                // Losujemy odchylenie dla TEGO KONKRETNEGO pocisku
                float randomZ = Random.Range(-spreadRandomness, spreadRandomness);
                
                // Obracamy lufę o ten losowy kąt
                firePoint.rotation = originalRotation * Quaternion.Euler(0, 0, randomZ);

                SpawnBullet(firePoint);
                
                // Czekamy bardzo krótko na kolejny pocisk
                yield return new WaitForSeconds(burstDelayBetweenBullets);
            }

            // Po zakończeniu serii wracamy do celowania prosto
            firePoint.rotation = originalRotation;

            waitingToShoot = false;
        }
    }
}