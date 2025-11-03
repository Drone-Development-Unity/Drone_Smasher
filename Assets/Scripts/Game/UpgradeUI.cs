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

        public void SetUpgrade(UpgradeData data)
        {
            upgradeImage = data.upgradeImage;
            upgradeName.text = data.upgradeName;
            upgradeCost.text = data.upgradeCost.ToString();
            upgradeCurrencyImage = data.upgradeValueImage;
            upgradeComboValue.text = data.upgradeComboValue.ToString();
        }
    }
}