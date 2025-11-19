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
        [SerializeField] protected EnemyHealthBar healthBar;
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

        public GameObject wreckPrefab;

        private void Start()
        {
            currentHealth = maxHealth;
            nextWaveManager = NextWaveManager.Instance;

            animController = GetComponent<EnemyAnimationController>();
            spriteRenderer = GetComponent<SpriteRenderer>();

            healthBar.SetMaxAmount(maxHealth);
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

            if (wreckPrefab != null)
            {
                GameObject wreckObj = Instantiate(wreckPrefab, transform.position, Quaternion.identity);
                wreckObj.transform.SetParent(WreckManager.Instance.wrecksContainer.transform, false);
                WreckManager.Instance.RegisterWreck(wreckObj.GetComponent<Wreck>());

                // kopiowanie sprite’a przeciwnika
                SpriteRenderer enemyRenderer = GetComponent<SpriteRenderer>();
                SpriteRenderer wreckRenderer = wreckObj.GetComponent<SpriteRenderer>();
                if (enemyRenderer != null && wreckRenderer != null)
                {
                    wreckRenderer.sprite = enemyRenderer.sprite;
                    //wreckRenderer.color = Color.gray; // np. domyślny kolor wraku
                }
            }
            Destroy(gameObject);
        }

        public int GetID()
        {
            return id;
        }
    }
}
