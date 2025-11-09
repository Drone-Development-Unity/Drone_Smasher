using Assets.Scripts.Wave;
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

        private bool _isAlive = true;
        public bool IsAlive
        {
            get => _isAlive;
            set => _isAlive = value;
        }

        protected int id = Guid.NewGuid().GetHashCode();

        private NextWaveManager nextWaveManager;

        private EnemyAnimationController animController;
        private SpriteRenderer spriteRenderer;

        private void Start()
        {
            currentHealth = maxHealth;
            nextWaveManager = NextWaveManager.Instance;

            animController = GetComponent<EnemyAnimationController>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        public virtual void TakeDamage(int damage)
        {
            //Debug.Log($"{gameObject.name} took {damage} damage.");

            currentHealth -= damage;

            if (animController != null && spriteRenderer != null)
                animController.OnHitAnim(spriteRenderer, IsAlive);

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        public virtual void Die()
        {
            IsAlive = false;

            //Debug.Log($"{gameObject.name} died.");
            nextWaveManager.EnemyKilled(this);
            Destroy(gameObject);
        }

        public int GetID()
        {
            return id;
        }
    }
}
