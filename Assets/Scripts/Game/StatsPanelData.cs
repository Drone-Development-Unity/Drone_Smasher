using System;
using System.Collections.Generic;
using Game.StatsPanel;
using UnityEngine; 
namespace Game
{
    /// <summary>
    /// Defines datatypes in StatsPanel UI
    /// </summary>
    [System.Serializable]
    public class StatsData
    {
        public string objectName;
        public string description;
        public List<PropertyData> properties;
        public List<UpgradeData> upgrades;
    }

    [System.Serializable]
    public class PropertyData
    {
        public StatType propertyType;
        public double propertyValue;
    }

    [System.Serializable]
    public class UpgradeData
    {
        //Editable in editor
        public Sprite upgradeImage;
        public string upgradeName;
        public double upgradeCost;
        public Sprite upgradeCurrencyImage;
        public double upgradeAmount; //amount of value that upgrade increases
        public string targetPropertyName; //name of connected property
        public int currencyId;
        //Non editable
        [NonSerialized] CurrencyData upgradeCurrency;
        public GameObject baseObject;
        public int upgradeComboValue; //upgraded amount
    }
}