using Assets.Scripts.Bullets.Rocket;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Allies.Cannons.SpecificCannons
{
    internal class RocketLauncher : BaseCannonShoot
    {
        [Header("Bullet")]
        public GameObject enemyBullet;

        [Header("Shooting")]
        public float rocketSpawnDelay = 0.2f;
        public int numberOfRocketInSalvo = 3;

        [Header("Rotation")]
        public float rotationDuration = 0.5f;
        public float rotationAngleOfReloading = 45f;
        public float rotationAngleOfReadyToShot = 0f;
        public bool rotateToEnemy = false;

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            reloadTimer -= Time.deltaTime;
            if (reloadTimer <= 0f)
            {
                reloadTimer = reloadDelay;

                GetClosestEnemyTarget();
                if (targetEnemy != null)
                {
                    StartCoroutine(SpawnRocketsCoroutine());
                }
            }
        }

        IEnumerator SpawnRocketsCoroutine()
        {
            // rotate cannon to ReadyToShot angle
            if (rotateToEnemy) {
                Vector2 direction = targetEnemy.position - transform.position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                rotationAngleOfReadyToShot = angle - 90f;
                rotationAngleOfReloading = rotationAngleOfReadyToShot;
            }

            yield return StartCoroutine(RotateToAngle(rotationAngleOfReadyToShot, rotationDuration));
            yield return new WaitForSeconds(rotationDuration);

            // shoot rockets
            for (int i = 0; i < numberOfRocketInSalvo; i++)
            {
                SpawnRocket();
                yield return new WaitForSeconds(rocketSpawnDelay);
            }

            // rotate cannon back to Reloading angle
            yield return StartCoroutine(RotateToAngle(rotationAngleOfReloading, rotationDuration));
        }

        IEnumerator RotateToAngle(float targetAngle, float duration)
        {
            float startAngle = transform.eulerAngles.z;
            float time = 0f;

            float delta = Mathf.DeltaAngle(startAngle, targetAngle);

            while (time < duration)
            {
                time += Time.deltaTime;
                float dt = time / duration;

                // obrót po najkrótszej drodze
                float angle = startAngle + delta * dt;
                transform.rotation = Quaternion.Euler(0, 0, angle);

                yield return null;
            }

            transform.rotation = Quaternion.Euler(0, 0, targetAngle);
        }

        void SpawnRocket()
        {
            if (enemyBullet == null || firePoint == null) return;
            if (targetEnemy == null) return;

            GameObject Bullet = Instantiate(enemyBullet, firePoint.position, firePoint.rotation);
            Bullet.transform.parent = bulletsContainer.transform; // make bullet child of bulletsContainer

            // give target to rocket
            var rocket = Bullet.GetComponent<RocketMovement>();
            if (rocket != null)
            {
                rocket.target = targetEnemy;
            }
        }
    }
}
