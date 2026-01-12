using Assets.Scripts.Enemies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace Assets.Scripts.Bullets.Rocket
{
    internal class RocketMovement : MonoBehaviour
    {
        [HideInInspector] public Transform target;
        [Header("Movement")]
        public float speed = 5f;
        public float rotateSpeed = 200f;
        protected float rotationTimer;
        public float rotationTime = 3f;

        [Header("Aiming noise")]
        public float maxRandomNoiseEnemyPosition = 10f;

        protected Rigidbody2D rb;

        [Header("Explode")]
        public ParticleSystem burstParticle;
        public float explodeRadius;
        public int damage;

        protected Vector2 direction;
        protected Vector2 targetPosition;
        protected float targetAngle;




        protected virtual void Start()
        {
            rb = GetComponent<Rigidbody2D>();

            if (target != null)
            {
                Vector2 enemyPos = target.position;

                float randomNoisePos = UnityEngine.Random.Range(-maxRandomNoiseEnemyPosition, maxRandomNoiseEnemyPosition);
                Vector2 noisyTarget = new Vector2(enemyPos.x + randomNoisePos, enemyPos.y + randomNoisePos);

                targetPosition = noisyTarget;  //saving position
                rotationTimer = rotationTime;
            }

            BulletCollisionDetection bulletCollisionDetection = GetComponent<BulletCollisionDetection>();
            if (bulletCollisionDetection != null)
            {
                bulletCollisionDetection.Initialize(gameObject, damage, direction);
            }
        }

        protected virtual void FixedUpdate()
        {


            //check if enemy in radius of explosion
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explodeRadius);
            foreach (Collider2D hit in hits)
            {
                if (hit.CompareTag("Enemy"))
                {
                    Explode();
                    break;
                }
            }

            if (rotationTimer >= 0f)
            {
                //destination
                Vector2 toTarget = targetPosition - rb.position;
                direction = toTarget.normalized;

                targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            }

            //rotation
            float rotation = Mathf.MoveTowardsAngle(rb.rotation, targetAngle, rotateSpeed * Time.deltaTime);
            rb.MoveRotation(rotation);


            //movement
            rb.linearVelocity = transform.up * speed;

            rotationTimer -= Time.deltaTime;
        }

        protected virtual void Explode()
        {
            // harm enemies in radius of explosion
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explodeRadius);
            foreach (Collider2D hit in hits)
            {
                if (hit.CompareTag("Enemy"))
                {
                    var enemy = hit.GetComponent<BaseEnemy>();
                    if (enemy != null)
                        enemy.TakeDamage(damage);
                }
            }

            ExplodeParticles();
            Destroy(gameObject);
        }

        public void ExplodeParticles()
        {
            if (burstParticle == null) return;

            ParticleSystem ep = Instantiate(burstParticle, transform.position, Quaternion.identity);
            ep.Play();
            Destroy(ep.gameObject, ep.main.duration + ep.main.startLifetime.constantMax);
        }

        protected void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explodeRadius);

            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(targetPosition, 0.1f);
        }
    }
}
