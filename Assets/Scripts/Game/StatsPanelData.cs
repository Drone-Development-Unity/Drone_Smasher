using System.Collections.Generic;
using UnityEngine.UI;

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
        public string propertyName;
        public double propertyValue;
    }

    [System.Serializable]
    public class UpgradeData
    {
        public Image upgradeImage;
        public string upgradeName;
        public double upgradeCost;
        public Image upgradeValueImage;
        public int upgradeComboValue; //upgraded amount
    }
}