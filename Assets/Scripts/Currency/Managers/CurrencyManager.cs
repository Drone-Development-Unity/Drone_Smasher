using System;
using System.Collections.Generic;
using Game;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }
    public event Action<int, double> OnCurrencyChanged;

    [Header("Necessary prefabs")]
    [SerializeField] private GameObject currencyListElement; //NecessariesPanel/CurrenciesPanelScrollView object
    [SerializeField] private Transform currenciesContainer;
    [SerializeField] private GameObject necessariesPanelObject;
    public List<CurrencyData> currencies;

    private TextMeshProUGUI currencyName;
    private Sprite currencySprite;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        ShowCurrencies();
    }
    public void ShowCurrencies()
    {
        // Clear previous visible currencies
        foreach (Transform child in currenciesContainer)
            Destroy(child.gameObject);

        // Populate currencies
        foreach (var currency in currencies)
        {
            var cur = Instantiate(currencyListElement, currenciesContainer);
            var currencyUI = cur.GetComponent<CurrencyUI>();
            currencyUI.SetCurrency(currency);
        }
    }
    
    
    public void AddCurrency(int currencyId,double value)
    {
        foreach (var currency in currencies)
        {
            if (currency.currencyId == currencyId)
            {
                currency.amount += value;                
                OnCurrencyChanged?.Invoke(currency.currencyId, currency.amount);
            }
        }
    }

    public bool SpendCurrency(int _currencyId,double value)
    {
        foreach (var currency in currencies)
        {
            if (currency.currencyId == _currencyId)
            {
                if (currency.amount >= value)
                {
                    currency.amount -= value;
                    OnCurrencyChanged?.Invoke(currency.currencyId, currency.amount);
                    return true;
                }
                return false;
            }
        }
        return false;
    }

    public CurrencyData GetCurrencyInstance(int _currencyId)
    {
        foreach (var currency in currencies)
        {
            if (currency.currencyId == _currencyId)
            {
                return currency;
            }
        }
        return null;
    }
    
}
