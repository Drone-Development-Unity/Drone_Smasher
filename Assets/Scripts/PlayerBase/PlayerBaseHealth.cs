using Assets.Scripts.Game.UIElements;
using Assets.Scripts.Save.DTO;
using Assets.Scripts.SaveSystem;
using Game.Managers;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PlayerBase
{
    public class PlayerBaseHealth : MonoBehaviour, IVulnerable
    {
        // singleton
        [HideInInspector] public static PlayerBaseHealth Instance { get; private set; }
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

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

        public void ResetHP()
        {
            currentHealth = maxHealth;
            healthBar.SetMaxAmount(maxHealth);
            healthBar.SetAmount(currentHealth);
        }

        public virtual void Die()
        {
            IsAlive = false;

            StartCoroutine(DeathSequence());
        }

        private IEnumerator DeathSequence()
        {
            Time.timeScale = 0f;

            GameUIManager.Instance.ShowDeathScreen();

            yield return new WaitForSecondsRealtime(3f);

            Time.timeScale = 1f;

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

    }
}