using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    /// <summary>
    /// Used by CurrencyListElement prefab
    /// </summary>
    public class CurrencyUI : MonoBehaviour
    {
        public Image currencyImageUI;
        public TextMeshProUGUI currencyAmount;
        public int currencyId;

        public void SetCurrency(CurrencyData data)
        {
            currencyImageUI.sprite = data.icon;
            currencyAmount.text = data.amount.ToString();
            currencyId = data.currencyId;

            //event
            CurrencyManager.Instance.OnCurrencyChanged += HandleCurrencyChanged;
        }

        private void HandleCurrencyChanged(int changedId, double prevAmoount, double newAmount)
        {
            if (changedId == currencyId)
            {
                CurrencyChangeAnimationo(prevAmoount, newAmount);
            }
        }

        private void CurrencyChangeAnimationo(double pAmount, double newAmount)
        {
            DOTween.Kill(currencyAmount);
            DOTween.To(
                    () => (float)pAmount,
                    x =>
                    {
                        pAmount = x;
                        currencyAmount.text = pAmount.ToString("F0");
                    },
                    (float)newAmount,
                    0.3f
                ).SetEase(Ease.OutQuad)
                .SetTarget(currencyAmount);
        }

        private void OnDestroy()
        {
            if (CurrencyManager.Instance != null)
                CurrencyManager.Instance.OnCurrencyChanged -= HandleCurrencyChanged;
        }
    }
}