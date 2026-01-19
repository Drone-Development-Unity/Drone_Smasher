using System;
using System.Collections.Generic;
using Game.Managers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Game.StatsPanel.CannonsContent
{
    public class CannonPurchaser:HighlightObject
    {
        public Transform spawnedCannon;
        public GameObject  cannonPrefab;
        public GameObject cannonPlaceholder;
        
        //First set by CheckCurrenciesSufficiency()
        private List<ShopCurrencyDetails> currenciesList;
        private CurrencyManager currManager;
        
        //list for storing indexes of insufficient currencies
        private List<int> insufficientIndexes = new();
        private bool BuyCannon()
        {
            if (cannonPrefab == null) return false;
            GameObject newCannon = Instantiate(
                cannonPrefab,
                spawnedCannon.position,
                spawnedCannon.rotation,
                spawnedCannon);
            return true;
        }
        private bool SellCannon(GameObject cannon, GameObject cannonHolder)
        {
            if (cannon == null) return false;
            var baseCannon = cannonHolder.GetComponent<PurchasableObject>();
            if (baseCannon != null) baseCannon.CannonDisposed();
            return true;
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            //TODO add check if player have enough resources to buy and animations
            base.OnPointerClick(eventData);
            
            if (CheckCurrenciesSufficiency())
            {
                //spawn cannon and spend money
                if (BuyCannon())
                {
                    SpendCurrencies(); //If user have enough coins spend coins 

                    //hide popup
                    var popup = GetComponent<PopupHandler>();
                    if (popup != null) popup.HidePopup();

                    var baseCannon = cannonPlaceholder.GetComponent<PurchasableObject>();
                    if (baseCannon != null) baseCannon.CannonPurchased();
                }
                else Debug.Log("Cannot spawn cannon - probably reference to prefab was not set in inspector");
            }
            else
            {
                //TODO make animation when user cant buy cannon
                //Debug.Log("User dont have enough coins");
                var uiManager = GameUIManager.Instance; //get ui manager instance
                if (uiManager != null)
                {
                    GameObject currencyList = uiManager.currencyList;
                    foreach (Transform certainCurrency in currencyList.transform)
                    {
                        var currId = certainCurrency.GetComponent<CurrencyUI>().currencyId;
                        if (insufficientIndexes.Contains(currId))
                        {
                            var animationScript = certainCurrency.GetComponent<CurrencyListAnimations>();
                            if (animationScript != null)animationScript.CurrencyNotSufficientAnimation();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Check if player have enough coins to buy cannon
        /// </summary>
        /// <returns>
        /// True - if user have enough coins
        /// False - if user don't have enough coins
        /// </returns>
        private bool CheckCurrenciesSufficiency()
        {
            insufficientIndexes.Clear(); //clear after each check
            
            currenciesList = GetCurrencies();
            
            currManager = CurrencyManager.Instance; //get currency manager instance
            if (currManager != null  && currenciesList != null)
            {
                foreach (var curr in currenciesList)
                {
                    if (!currManager.IsCurrencySufficient(curr.currencyId, (int)curr.price))
                    {
                        insufficientIndexes.Add(curr.currencyId);
                    }
                }
                if (insufficientIndexes.Count == 0) return true;
            }
            return false;
        }
        //Used to get currencies list with prices from this object CannonPriceSetter.cs script
        private List<ShopCurrencyDetails> GetCurrencies()
        {
            var currList = GetComponent<CannonPriceSetter>();
            if (currList != null) return currList.currenciesList;
            return null;
        }

        private void SpendCurrencies()
        {
            foreach (var curr in currenciesList)
            {
                currManager.SpendCurrency(curr.currencyId, curr.price);
            }
        }
    }
}