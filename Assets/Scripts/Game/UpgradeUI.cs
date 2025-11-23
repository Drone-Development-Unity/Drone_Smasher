using System;
using Game.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    /// <summary>
    /// Used by UpgradeOption prefab
    /// </summary>
    public class UpgradeUI:MonoBehaviour
    {
        public Image upgradeImage;
        public TextMeshProUGUI upgradeName;
        public TextMeshProUGUI upgradeCost;
        public Image upgradeCurrencyImage;
        public TextMeshProUGUI upgradeComboValue; //upgraded amount
        public GameObject baseObjectReference;
        public CurrencyData upgradeCurrency;
        public int currId;
        public void SetUpgrade(UpgradeData data)
        {
            upgradeImage.sprite = data.upgradeImage;
            baseObjectReference = data.baseObject;
            upgradeName.text = data.upgradeName;
            upgradeCost.text = data.upgradeCost.ToString();
            upgradeComboValue.text = data.upgradeComboValue.ToString();
            currId = data.currencyId;
            upgradeCurrency = CurrencyManager.Instance.GetCurrencyInstance(currId);
            if (upgradeCurrency == null)
            {
                Debug.Log("Could not find currency with id: " + currId);
            }
            upgradeCurrencyImage.sprite = upgradeCurrency?.icon;
            
        }
        public void ApplyUpgrade()
        {
            Unit unitScript = baseObjectReference.GetComponent<Unit>();
            StatsData stats = unitScript.GetStats();
            var upgrade = stats.upgrades.Find(u => u.upgradeName == upgradeName.text);
            var property = stats.properties.Find(p => p.propertyType.ToString() == upgrade.targetPropertyName);
            //change of currency amount
            if (CurrencyManager.Instance.SpendCurrency(upgradeCurrency.currencyId, upgrade.upgradeCost) &&
                property != null)
            {
                //Debug.Log("Upgraged");
                property.propertyValue += upgrade.upgradeAmount;
                upgrade.upgradeComboValue++;
                //change of upgrade price
                upgrade.upgradeCost = Math.Round(upgrade.upgradeCost * 1.3f);
                GameUIManager.Instance.ShowObjectProperties(stats);

            }
            //TODO add animation for when user have not enough money to upgrade something
            //TODO maybe something like connected currency amount color change to red and shake
        }
    }
}