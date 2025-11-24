using Assets.Scripts.Enemies.Controllers;
using Assets.Scripts.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Enemies
{
    public class BaseEnemy : MonoBehaviour, IVulnerable
    {
        [Header("Health stuff")]
        [SerializeField] protected int maxHealth;
        [SerializeField] protected EnemyHealthBar healthBar;
        
        [Header("References")]
        [SerializeField] protected GameObject enemyParent;
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
        private WreckSpawnerController wreckSpawner;
        private SpriteRenderer spriteRenderer;

        private void Start()
        {
            currentHealth = maxHealth;
            nextWaveManager = NextWaveManager.Instance;

            animController = GetComponent<EnemyAnimationController>();
            wreckSpawner = GetComponent<WreckSpawnerController>();

            spriteRenderer = GetComponent<SpriteRenderer>();

            healthBar.SetMaxAmount(maxHealth);
            
            //default enemyParent if null
            if (enemyParent == null)
            {
                enemyParent = gameObject;
            }
        }
        public virtual void TakeDamage(int damage)
        {
            //Debug.Log($"{gameObject.name} took {damage} damage.");

            currentHealth -= damage;

            healthBar.SetAmount(currentHealth);

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

            // explosion animation
            if (animController != null)
                animController.RunExplosionParticles(transform.position);

            // wreck spawn
            if (wreckSpawner != null)
                wreckSpawner.SpawnWreck(transform.position, Quaternion.identity, spriteRenderer);

            Destroy(enemyParent);
        }
      
        public int GetID()
        {
            return id;
        }
    }
}
