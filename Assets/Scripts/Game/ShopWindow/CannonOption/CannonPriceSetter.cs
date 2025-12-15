using System.Collections.Generic;
using System.Linq;
using Game;
using Game.Managers;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Used by ShopMenu Viewport/Cannons options to determine prices for cannons in various currencies
/// </summary>
public class CannonPriceSetter : MonoBehaviour
{
    [SerializeField] public List<ShopCurrencyDetails> currenciesList; //used to determine price for cannon in currencies types
    private CurrencyManager currManager; //reference to ui manager
    
    //Used to set properties of popup currency
    private Image currencyImage;
    
    //Popup currency prefab reference
    [SerializeField] public GameObject popupCurrency;
    
    private void Start()
    {
        currManager = CurrencyManager.Instance; //get currency manager instance
        if(!CheckCurrencies())Debug.Log("Probably user typed wrong id in CannonPriceSetter.cs");
        //Set prices and propagate objects in Viewport in popup details

        //set currencies prefabs inside Viewport of popup prefab
        var popupSet = popupCurrency.GetComponent<PopupSetCurrencies>();
        if (popupSet != null)
        {
            popupSet.SetCurrencies(currenciesList);
        }
    }
    /// <summary>
    /// Used to check if currencies id that user typed in ShopMenu/Viewport/Cannons/CannonOption - CannonPriceSetter.cs are correct
    /// </summary>
    /// <returns>
    /// False - if there is at least one wrong id
    /// True  - if all id's are correct
    /// </returns>
    private bool CheckCurrencies()
    {
        //get currencies details from currencyManager
        foreach (var currency in currenciesList)
        {
            var element = currManager.currencies.Find(c => c.currencyId == currency.currencyId);
            if (element == null)
            {
                return false; //user typed wrong currency id in CannonOption
            }
        }
        return true;
    }
}
