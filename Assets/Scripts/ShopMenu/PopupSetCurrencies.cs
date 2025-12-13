using System;
using System.Collections.Generic;
using System.Linq;
using Game;
using UnityEngine;

/// <summary>
/// Used by CannonShopPopups/Popup to propagate prefabs of currencies in list view of popup
/// </summary>
public class PopupSetCurrencies : MonoBehaviour
{
    [SerializeField] private Transform currenciesContent; //reference to popup Viewport/Content
    [SerializeField] private GameObject currencyPrefab; //reference to prefab - currency element in Viewport/Content
    private CurrencyManager currManager; //reference to ui manager
    
    //Create currencies prefabs inside Viewport/Content of this popup
    public void SetCurrencies(List<ShopCurrencyDetails> currenciesList)
    {
        currManager = CurrencyManager.Instance; //get currency manager instance
        foreach (var currency in currenciesList)
        {
            GameObject newCurrency = Instantiate(currencyPrefab, currenciesContent);
            if(currManager == null)Debug.Log("Currency Manager is null");
            //find correct currency from manager from currency id
            var currManagerRef = currManager.currencies.Find(c => c.currencyId == currency.currencyId);
            
            //set data on this prefab
            var setter = newCurrency.GetComponent<SetCurrencyDetails>();
            if(setter != null && currManagerRef!= null) setter.SetCurrencyData((int)currency.price, currManagerRef.icon);
        }
    }


}