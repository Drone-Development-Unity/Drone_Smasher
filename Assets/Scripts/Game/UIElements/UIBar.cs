using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.UIElements
{
    public class UIBar : MonoBehaviour
    {
        [SerializeField] protected Slider progressBar;
        [SerializeField] protected Gradient gradient;
        [SerializeField] protected Image content;

        protected CanvasGroup canvasGroup;

        protected virtual void Start()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public virtual void SetMaxAmount(float amount)
        {
            progressBar.maxValue = amount;
            progressBar.value = amount;

            content.color = gradient.Evaluate(1f);
        }

        public virtual void SetAmount(float amount)
        {
            progressBar.value = amount;
            content.color = gradient.Evaluate(progressBar.normalizedValue);
        }

        public void Show()
        {
            canvasGroup.alpha = 1f;
        }

        public void Hide()
        {
            canvasGroup.alpha = 0f;
        }
    }
}
