using Assets.Scripts.Game.UIElements;
using System;
using UnityEngine;

namespace PlayerBase
{
    public class PlayerBase : MonoBehaviour, IVulnerable
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
        [SerializeField] UIBar healthBar;


        private SpriteRenderer spriteRenderer;

        private void Start()
        {
            currentHealth = maxHealth;
            spriteRenderer = GetComponent<SpriteRenderer>();

            healthBar.SetMaxAmount(maxHealth);
            healthBar.SetAmount(currentHealth);
        }

        public virtual void TakeDamage(int damage)
        {
            //Debug.Log($"{gameObject.name} took {damage} damage.");

            currentHealth -= damage;
            healthBar.SetAmount(currentHealth);

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