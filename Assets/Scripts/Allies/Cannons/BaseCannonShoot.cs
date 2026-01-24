using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game;
using Game.StatsPanel;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using Random = UnityEngine.Random;

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

        protected StatsData stats; 
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
            stats = GetComponent<Unit>().GetStats();
            GetClosestEnemyTarget();

            reloadDelay = UnityEngine.Random.Range(minReloadDelay, maxReloadDelay);
        }

        //-------------UNIT-------------
        protected int GetDamageProperty()
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
            //if(success)Debug.Log($"Critical hit {gameObject.name}");
            return success;
        }

        private float calculateCrtiDmg(int baseDmg)
        {
            int critDmg = (int)(stats.properties
                .FirstOrDefault(p => p.propertyType == StatType.CritDmgPct)?.propertyValue ?? 0);
            float criticalDamage = baseDmg * (critDmg / 100f + 1);
            //Debug.Log($"Critical damage: {criticalDamage} (base: {baseDmg}) {gameObject.name}");
            return criticalDamage;
        }
        //-------------UNIT-------------
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
