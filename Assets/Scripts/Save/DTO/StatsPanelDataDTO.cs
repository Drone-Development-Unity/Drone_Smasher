using System;
using System.Collections.Generic;
using Game.StatsPanel;
using UnityEngine;

namespace Save.DTO
{
    [System.Serializable]
    public class StatsDataDTO
    {
        public string objectName;
        public string description;
        public List<PropertyDataDTO> properties;
        public List<UpgradeDataDTO> upgrades;
    }

    [System.Serializable]
    public class PropertyDataDTO
    {
        public StatType propertyType;
        public double propertyValue;
    }

    [System.Serializable]
    public class UpgradeDataDTO
    {
        public string upgradeImageName;
        public string upgradeName;
        public double upgradeCost;
        public double upgradeAmount; //amount of value that upgrade increases
        public string targetPropertyName; //name of connected property
        public int currencyId;
        public int upgradeComboValue; //upgraded amount
    }
}