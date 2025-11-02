using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Enemies
{
    public class BaseEnemy : MonoBehaviour, IVulnerable
    {
        [Header("Health stuff")]
        [SerializeField] protected int maxHealth;
        protected int currentHealth;

        private void Start()
        {
            currentHealth = maxHealth;
        }
        public virtual void TakeDamage(int damage)
        {
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        public virtual void Die()
        {
            Debug.Log($"{gameObject.name} died.");
            Destroy(gameObject);
        }
    }
}
