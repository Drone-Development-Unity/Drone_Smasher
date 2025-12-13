using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Enemies.BulletShooters
{
    public class EnemyDoubleBarrelShooter : EnemyBulletShooter
    {
        [Header("Second FirePoint")]
        [SerializeField] protected Transform secondFirePoint;
        private bool shootLeft = true;

        protected override IEnumerator SpawnBulletsInBurtsCoroutine()
        {
            waitingToShoot = true;

            for (int i = 0; i < numberOfBulletsInBurst; i++)
            {
                Transform currentFP = shootLeft ? firePoint : secondFirePoint;
                shootLeft = !shootLeft;

                SpawnBullet(currentFP);
                yield return new WaitForSeconds(burstDelayBetweenBullets);
            }

            waitingToShoot = false;
        }
    }
}
