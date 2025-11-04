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
        public void SetUpgrade(UpgradeData data)
        {
            baseObjectReference = data.baseObject;
            upgradeImage = data.upgradeImage;
            upgradeName.text = data.upgradeName;
            upgradeCost.text = data.upgradeCost.ToString();
            upgradeCurrencyImage = data.upgradeCurrencyImage;
            upgradeComboValue.text = data.upgradeComboValue.ToString();
        }
        public void ApplyUpgrade()
        {
            Unit unitScript = baseObjectReference.GetComponent<Unit>();
            StatsData stats = unitScript.GetStats();
            var upgrade = stats.upgrades.Find(u => u.upgradeName == upgradeName.text);
            var property = stats.properties.Find(p => p.propertyName == upgrade.targetPropertyName);
            
            if (property != null)
            {
                property.propertyValue += upgrade.upgradeAmount;
                upgrade.upgradeComboValue++;
                //change of upgrade price
                upgrade.upgradeCost = Math.Round(upgrade.upgradeCost * 1.3f);
                
                GameUIManager.Instance.ShowObjectProperties(stats);
            }
        }
    }
}