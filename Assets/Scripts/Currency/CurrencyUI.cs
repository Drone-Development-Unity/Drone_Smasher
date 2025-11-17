using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    /// <summary>
    /// Used by CurrencyListElement prefab
    /// </summary>
    public class CurrencyUI:MonoBehaviour
    {
        public Image currencyImageUI;
        public TextMeshProUGUI currencyAmount;
        private int currencyId;
        
        public void SetCurrency(CurrencyData data)
        {
            currencyImageUI.sprite = data.icon;
            currencyAmount.text = data.amount.ToString();
            currencyId = data.currencyId;
            
            //event
            CurrencyManager.Instance.OnCurrencyChanged += HandleCurrencyChanged;
        }

        private void HandleCurrencyChanged(int changedId, double newAmount)
        {
            if (changedId == currencyId)
            {
                currencyAmount.text = newAmount.ToString();
            }
        }

        private void OnDestroy()
        {
            if (CurrencyManager.Instance != null)
                CurrencyManager.Instance.OnCurrencyChanged -= HandleCurrencyChanged;
        }
    }
}