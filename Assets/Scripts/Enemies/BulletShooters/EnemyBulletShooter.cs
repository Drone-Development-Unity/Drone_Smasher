using Assets.Scripts.Bullets;
using Assets.Scripts.Interfaces.Enemy;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Enemies.BulletShooters
{
    public class EnemyBulletShooter : BaseEnemyShoot, IShooter
    {
        [Header("Bullet spawn delay")]
        protected float bulletSpawnDelay;
        [SerializeField] private float minBulletSpawnDelay;
        [SerializeField] private float maxBulletSpawnDelay;

        [Header("Bullet spawn chance")]
        protected float bulletSpawnChance;
        [SerializeField] private float minBulletSpawnChance;
        [SerializeField] private float maxBulletSpawnChance;

        [Header("Burst shooting")]
        protected int numberOfBulletsInBurst;
        [SerializeField] private int minNumberOfBulletsInBurst;
        [SerializeField] private int maxNumberOfBulletsInBurst;
        [SerializeField] private float burstDelayBetweenBullets;

        [Header("Bullet properties")]
        [SerializeField] protected GameObject bulletPrefab;
        [SerializeField] protected float bulletSpeed;

        [Header("FirePoints")]
        [SerializeField] protected Transform firePoint;

        public override void Start()
        {
            base.Start();

            bulletSpawnDelay = UnityEngine.Random.Range(minBulletSpawnDelay, maxBulletSpawnDelay);
            bulletSpawnChance = UnityEngine.Random.Range(minBulletSpawnChance, maxBulletSpawnChance);
            numberOfBulletsInBurst = UnityEngine.Random.Range(minNumberOfBulletsInBurst, maxNumberOfBulletsInBurst);
        }

        protected virtual void Update()
        {
            if (waitingToShoot) return;

            shootTimer -= Time.deltaTime;
            if(shootTimer <= 0f)
            {
                shootTimer = bulletSpawnDelay;

                if(UnityEngine.Random.value < bulletSpawnChance)
                {
                    StartCoroutine(SpawnBulletsInBurtsCoroutine());
                }
            }

        }



        public void SpawnBullet(Transform firePoint)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

            bullet.transform.parent = bulletsCointainer.transform;
            bullet.GetComponent<BulletCollisionDetection>().Initialize(this.gameObject, damage);

            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.linearVelocity = firePoint.up * bulletSpeed;
        }

        IEnumerator SpawnBulletsInBurtsCoroutine()
        {
            waitingToShoot = true;
            
            for(int i = 0; i < numberOfBulletsInBurst; i++)
            {
                SpawnBullet(firePoint.transform);
                yield return new WaitForSeconds(burstDelayBetweenBullets);
            }

            waitingToShoot = false;
        }
    }
}
