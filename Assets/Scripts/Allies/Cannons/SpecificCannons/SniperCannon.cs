using Assets.Scripts.Enemies;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

namespace Assets.Scripts.Allies.Cannons.SpecificCannons
{
    internal class SniperCannon : BaseCannonShoot
    {
        [Header("Laser")]
        public LineRenderer aimingRay;
        public LineRenderer hurtfulRay;
        public ParticleSystem hurtfulRayParticle;
        private ParticleSystem currentHurtfulRayParticle;
        public int damage = 15;

        [Header("Shooting")]
        private RaycastHit2D[] hitsInfoAim;
        private Coroutine aimAndShootCoroutine;
        public float hurtfulLaserBeamDuration = 0.8f;

        [Header("Aiming")]
        public float onEnemyTrackingTime = 1f;// time of aiming the cannon at the enemy
        public float lockAimTime = 2f;
        public float aimingLaserBlinkDuration = 0.2f;
        public int numberOfBlinks = 3;

        private float aimingTimer = 0f;
        private Vector2 targetPosition;

        private Vector3 laserStart = Vector3.zero;
        private Vector3 laserEnd = Vector3.zero;

        public override void Start()
        {
            base.Start();
            if (hurtfulRay != null)
            {
                hurtfulRay.sortingOrder = 2;
            }
            SetStateAimingLaser(false);
            SetStateShootLaser(false);
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            if (aimAndShootCoroutine == null)
            {
                reloadTimer -= Time.deltaTime;
                if (reloadTimer <= 0f)
                {
                    reloadTimer = reloadDelay;

                    SetStateAimingLaser(true);
                    SetStateShootLaser(false);

                    GetClosestEnemyTarget();

                    if (targetEnemy != null)
                    {
                        aimAndShootCoroutine = StartCoroutine(AimAndShootAtEnemyCoroutine());
                    }
                    else
                    {
                        SetStateAimingLaser(false);
                        SetStateShootLaser(false);
                    }
                }
            }
        }

        private void SetStateAimingLaser(bool state)
        {
            if (aimingRay == null) return;

            aimingRay.enabled = state;
            if (state)
            {
                aimingRay.positionCount = 2;
                aimingRay.SetPosition(0, laserStart);
                aimingRay.SetPosition(1, laserEnd);
            }
        }

        private void SetStateShootLaser(bool state)
        {
            if (hurtfulRay != null)
                hurtfulRay.enabled = state;
        }

        private void ShootLaser(Vector3 start, Vector3 end)
        {
            if (hurtfulRay != null)
            {
                hurtfulRay.enabled = true;
                hurtfulRay.SetPosition(0, start);
                hurtfulRay.SetPosition(1, end);
            }

            if (hurtfulRayParticle != null)
            {
                Quaternion particleDirection = Quaternion.LookRotation(firePoint.up);
                currentHurtfulRayParticle =
                    Instantiate(hurtfulRayParticle, start, particleDirection);
                currentHurtfulRayParticle.Play();
            }
        }

        private void TurnOffShootLaser()
        {
            SetStateShootLaser(false);

            if (currentHurtfulRayParticle != null)
            {
                Destroy(currentHurtfulRayParticle.gameObject);
                currentHurtfulRayParticle = null;
            }
        }

        private IEnumerator AimAndShootAtEnemyCoroutine()
        {
            aimingTimer = lockAimTime;
            if (targetEnemy == null) {
                aimAndShootCoroutine = null;
                yield break;
            }

            while (aimingTimer > 0f)
            {
                if (targetEnemy == null)
                {
                    aimAndShootCoroutine = null;
                    yield break;
                }

                targetPosition = targetEnemy.position;


                Vector2 toTarget = targetPosition - rb.position;
                Vector2 direction = toTarget.normalized;

                float rotateAmount = Vector3.Cross(direction, transform.up).z;
                rb.rotation -= rotateAmount * rotationSpeed * Time.deltaTime;

                //Vector2 toTarget = targetPosition - rb.position;
                //float targetAngle = Vector2.SignedAngle(Vector2.up, toTarget);
                //float newAngle = Mathf.MoveTowardsAngle(
                //    rb.rotation,
                //    targetAngle,
                //    rotationSpeed * Time.deltaTime
                //);

                //rb.MoveRotation(newAngle);

                Vector2 hitPoint = firePoint.position + firePoint.up * 100f;

                hitsInfoAim = Physics2D.RaycastAll(firePoint.position, firePoint.up, 100f);

                bool enemySeen = false;

                laserStart = firePoint.position;
                laserEnd = hitPoint;

                foreach (RaycastHit2D hit in hitsInfoAim)
                {
                    if (hit.transform == transform)
                        continue;

                    if (hit.transform.CompareTag("PlayerBase"))
                        continue;

                    if (hit.transform.CompareTag("Enemy"))
                    {
                        enemySeen = true;
                        hitPoint = hit.point;
                        aimingTimer -= Time.deltaTime;
                        break;
                    }
                }

                if (!enemySeen)
                    aimingTimer = lockAimTime;

                laserEnd = hitPoint;

                aimingRay.SetPosition(0, laserStart);
                aimingRay.SetPosition(1, laserEnd);

                yield return null;
            }

            // laser blink
            laserEnd = firePoint.position + firePoint.up * 100f;
            for (int i = 0; i < numberOfBlinks * 2; i++)
            {
                aimingRay.enabled = !aimingRay.enabled;
                yield return new WaitForSeconds(aimingLaserBlinkDuration);
            }
            SetStateAimingLaser(false);

            // shoot
            RaycastHit2D[] hitsInfoShoot =
                Physics2D.RaycastAll(firePoint.position, firePoint.up, 100f);

            Vector2 shootHitPoint = firePoint.position + firePoint.up * 100f;

            foreach (RaycastHit2D hit in hitsInfoShoot)
            {
                if (hit.transform == transform)
                    continue;

                if (hit.transform.CompareTag("PlayerBase"))
                    continue;

                if (hit.transform.CompareTag("Enemy"))
                {
                    var enemy = hit.transform.GetComponent<BaseEnemy>();
                    if (enemy != null)
                        enemy.TakeDamage(GetDamageProperty());

                    //shootHitPoint = hit.point;
                    //break;
                }
            }

            ShootLaser(firePoint.position, shootHitPoint);

            yield return new WaitForSeconds(hurtfulLaserBeamDuration);

            TurnOffShootLaser();

            reloadTimer = reloadDelay;
            aimAndShootCoroutine = null;
        }
    }
}
