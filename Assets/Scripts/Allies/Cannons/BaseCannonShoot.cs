using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

namespace Assets.Scripts.Allies.Cannons
{
    internal class BaseCannonShoot : MonoBehaviour
    {
        [Header("FirePoint")]
        public Transform firePoint;
        protected List<Transform> targetEnemies = new List<Transform>();
        protected Transform targetEnemy;
        protected Rigidbody2D rb;
        protected GameObject bulletsContainer;

        [Header("Reloading")]
        public float minReloadDelay;
        public float maxReloadDelay;
        protected float reloadDelay;
        protected float reloadTimer = 0f;

        [Header("Rotation")]
        public float rotationSpeed = 200f;

        protected virtual void FixedUpdate()
        {
            if (targetEnemy != null)
            {
                GetClosestEnemyTarget();
            }
            else if (targetEnemies.Count == 0)
            {
                float currentRot = rb.rotation;
                float targetRot = 0f;
                float speed = rotationSpeed * 0.5f * Time.deltaTime;

                rb.rotation = Mathf.MoveTowardsAngle(currentRot, targetRot, speed);
            }
            
        }

        virtual public void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            bulletsContainer = GameObject.Find("BulletsContainer");

            
            GetClosestEnemyTarget();

            reloadDelay = UnityEngine.Random.Range(minReloadDelay, maxReloadDelay);
        }

        private void FetchEnemies()
        {
            targetEnemies.Clear();

            GameObject[] enemiesObj = GameObject.FindGameObjectsWithTag("Enemy");
            if (enemiesObj.Length > 0)
            {
                foreach (GameObject enemyObj in enemiesObj)
                {
                    if (enemyObj != null)
                        targetEnemies.Add(enemyObj.transform);
                }
            }
        }
        protected void GetClosestEnemyTarget()
        {
            FetchEnemies();

            float closestDistance = Mathf.Infinity;
            Vector3 currentPosition = transform.position;

            foreach (Transform enemy in targetEnemies)
            {
                if (enemy == null)
                    continue;

                float distance = Vector3.Distance(currentPosition, enemy.position);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    targetEnemy = enemy;
                }
            }
        }
    }
}
