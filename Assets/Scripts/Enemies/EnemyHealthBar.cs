using Assets.Scripts.Game.UIElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Enemies
{
    public class EnemyHealthBar : UIBar
    {
        [Header("Combo settings")]
        [SerializeField] protected Slider comboBar;

        [SerializeField] protected float comboDropSpeed = 0.1f;
        protected float comboTimer = 0f;
        [SerializeField] protected float comboDuration = 1.0f;

        [Header("Show Bar settings")]
        [SerializeField] protected float showUIDuration = 2.0f;
        [SerializeField] protected float hideUISpeed = 5f;

        protected float showUITimer = 0f;
        protected float lastHealthVal = 0f;

        public override void SetMaxAmount(float amount)
        {
            base.SetMaxAmount(amount);
            comboBar.maxValue = amount;
            comboBar.value = amount;

            lastHealthVal = amount;

            Hide();
        }

        public override void SetAmount(float amount)
        {
            base.SetAmount(amount);

            comboBar.value = lastHealthVal;

            comboTimer = comboDuration;
            showUITimer = showUIDuration;

            Show();
        }

        protected virtual void FixedUpdate()
        {
            // combo bar drop
            if (progressBar.value < comboBar.value && comboTimer <= 0)
            {
                comboBar.value -= comboDropSpeed * Time.deltaTime;
                lastHealthVal = progressBar.value;
            }

            // fade out
            if (showUITimer <= 0 && canvasGroup.alpha > 0)
            {
                comboBar.value = 0f;
                canvasGroup.alpha = Mathf.Max(0f, canvasGroup.alpha - hideUISpeed * Time.deltaTime);
            }
            comboTimer -= Time.deltaTime;
            showUITimer -= Time.deltaTime;
        }
        private void LateUpdate()
        {
            transform.rotation = Quaternion.identity; // health bar doesn't rotate
        }
    }
}

