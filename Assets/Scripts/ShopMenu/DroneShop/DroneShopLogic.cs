using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DroneShopLogic : MonoBehaviour
{
    [Header("Ustawienia")] 
    public Transform gatherersContainer;
    public Transform droneSpawnLocation;
    public int currencyId;
    public double priceMultiplier = 1.4f;
    [Header("Ceny dronów")] 
    public int standardDroneBasePrice = 200;
    
    [Header("Referencje")]
    public Image currencyImage;
    public GameObject dronePrefab;
    public TextMeshProUGUI dronePurchasedNumber;
    public TextMeshProUGUI buyDroneNewPrice;

    
    private int standardDroneNewPrice = 0;
    private int standardDroneAmount = 0;
    private CurrencyManager currencyManager;
    private CurrencyData currencyData;
    private void Start()
    {
        currencyManager = CurrencyManager.Instance;
        currencyData = currencyManager.GetCurrencyInstance(currencyId);
        currencyImage.sprite = currencyData.icon;

    }

    private void OnEnable()
    {
        if (gatherersContainer != null && dronePrefab != null && dronePurchasedNumber != null)
        {
            //get child amount (purchased drones amount)
            standardDroneAmount = gatherersContainer.childCount;
            standardDroneNewPrice = CalculatePurchasePrice(standardDroneBasePrice, standardDroneAmount);
            //set text
            dronePurchasedNumber.text = standardDroneAmount.ToString();
            buyDroneNewPrice.text = standardDroneNewPrice.ToString();
        }
        else
        {
            Debug.LogWarning("References not set DroneShopLogic");
        }
    }

    private int CalculatePurchasePrice(int standardPrice, int purchasedAmount)
    {
        return Mathf.RoundToInt((float)(purchasedAmount * standardPrice * priceMultiplier));
    }

    private void UpdateText()
    {
        //get child amount (purchased drones amount)
        standardDroneAmount++;
        standardDroneNewPrice = CalculatePurchasePrice(standardDroneBasePrice, standardDroneAmount);
        //set text
        dronePurchasedNumber.text = standardDroneAmount.ToString();
        buyDroneNewPrice.text = standardDroneNewPrice.ToString();
    }

    public void BuyDrone()
    {
        if (currencyManager.IsCurrencySufficient(currencyId, standardDroneNewPrice))
        {
            currencyManager.SpendCurrency(currencyId, standardDroneNewPrice);
            //after successful buy
            UpdateText();
            SpawnDrone(dronePrefab);
        }
    }

    private void SpawnDrone(GameObject dronePrefab)
    {
        Instantiate(dronePrefab, droneSpawnLocation.position, Quaternion.identity);
    }
}