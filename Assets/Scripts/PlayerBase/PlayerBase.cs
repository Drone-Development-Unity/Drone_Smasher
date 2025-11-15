using System;
using UnityEngine;

namespace PlayerBase
{
    public class PlayerBase : MonoBehaviour, IVulnerable
    {
        [Header("Health stuff")] [SerializeField]
        protected int maxHealth;
        [SerializeField] protected int currentHealth;

        private bool _isAlive = true;

        public bool IsAlive
        {
            get => _isAlive;
            set => _isAlive = value;
        }
        private SpriteRenderer spriteRenderer;

        private void Start()
        {
            currentHealth = maxHealth;
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public virtual void TakeDamage(int damage)
        {
            //Debug.Log($"{gameObject.name} took {damage} damage.");

            currentHealth -= damage;

            /*
        if (animController != null && spriteRenderer != null)
            animController.OnHitAnim(spriteRenderer, IsAlive);
            */

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        public virtual void Die()
        {
            IsAlive = false;
            //Show some UI to reset lvl or something
        }
    }
}