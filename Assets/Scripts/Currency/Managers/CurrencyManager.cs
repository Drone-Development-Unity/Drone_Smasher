using System;
using System.Collections.Generic;
using Game;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }
    public event Action<int, double, double> OnCurrencyChanged;

    [Header("Necessary prefabs")]
    [SerializeField] private GameObject currencyListElement; //NecessariesPanel/CurrenciesPanelScrollView object
    [SerializeField] private Transform currenciesContainer;
    [SerializeField] private GameObject necessariesPanelObject;
    
    [Header("Actual currencies elements")]
    public List<CurrencyData> currencies;//store currencies data
    
    [Header("Currencies data")]
    public List<CurrenciesLoadData> currenciesData; //used to load currencies data
    
    private TextMeshProUGUI currencyName;
    private Sprite currencySprite;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        //ConnectLoadedCurrencies();
        
        //Save currencies
        //var dto = ConvertToDTO();
        
        //Load currencies
        //ShowCurrencies();
        //ConnectLoadedCurrencies();
        
        
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
                double currencyPrev = currency.amount;
                currency.amount += Math.Round(value);                
                OnCurrencyChanged?.Invoke(currency.currencyId, currencyPrev, currency.amount);
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
                    double  currencyPrev = currency.amount;
                    currency.amount -= value;
                    OnCurrencyChanged?.Invoke(currency.currencyId, currencyPrev, currency.amount);
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

    //Used by CannonOption CannonPurchaser.cs to check if user have enough coins
    public bool IsCurrencySufficient(int _currencyId, int amount)
    {
        var curr = currencies.Find(c => c.currencyId == _currencyId);
        if (curr != null)
        {
            //sufficient amount
            if(curr.amount >= amount) return true;
        }
        return false;
    }

    //Used to connect currencyData from currenciesData to currencies list
    private void ConnectLoadedCurrencies()
    {
        foreach (var currency in currencies)
            {
                foreach (var currData in currenciesData)
                {
                    if (currency.currencyId == currData.currencyId)
                    {
                        currency.icon=currData.currencyIcon;
                        currency.currencyName=currData.currencyName;
                    } 
                }
            }
    }
    
    //used to convert currency data to DTO format to save/load data
    public CurrenciesDTO ConvertToDTO()
    {
        var dto = new CurrenciesDTO();
        dto.currencies = new List<CurrencyDataDTO>();

        foreach (var c in currencies)
        {
            dto.currencies.Add(new CurrencyDataDTO
            {
                currencyId = c.currencyId,
                amount = c.amount
            });
        }
        
        return dto;
    }

    //used to load currency data from DTO format
    public void LoadCurrencyFromDTO(CurrenciesDTO dto)
    {
        if (dto == null || dto.currencies == null) return;

        //clear local data
        currencies.Clear();

        //rewrite dto to currencies
        foreach (var d in dto.currencies)
        {
            var newCurrency = new CurrencyData
            {
                currencyId = d.currencyId,
                amount = d.amount
            };

            currencies.Add(newCurrency);
        }

        ConnectLoadedCurrencies();
        ShowCurrencies();
    }
    [ContextMenu("CreateNewCurrencies")]
    public void CreateAndLoadCurrencies()
    {
        
        //clear local data
        currencies.Clear();

        //rewrite dto to currencies
        for(int i = 0; i<currenciesData.Count;i++)
        {
            var newCurrency = new CurrencyData
            {
                currencyId = i,
                amount = 0
            };

            currencies.Add(newCurrency);
        }

        ConnectLoadedCurrencies();
        ShowCurrencies();
    }
    
}
