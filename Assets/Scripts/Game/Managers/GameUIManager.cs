using TMPro;
using UnityEngine;

namespace Game.Managers
{
    /// <summary>
    /// Manages StatsPanel information showing
    /// </summary>
    public class GameUIManager : MonoBehaviour
    {
        public static GameUIManager Instance;
        
        [Header("StatsPanel Elements")]
        public TextMeshProUGUI objectName;
        public TextMeshProUGUI descriptionText;
        public Transform propertiesContainer;
        public Transform upgradesContainer;
        public GameObject propertyPrefab;
        public GameObject upgradePrefab;

        private void Awake()
        {
            Instance = this;
        }

        public void ShowObjectProperties(StatsData data)
        {
            objectName.text = data.objectName;
            descriptionText.text = data.description;

            // Clear previous upgrades and properties
            foreach (Transform child in upgradesContainer)
                Destroy(child.gameObject);
            foreach (Transform child in propertiesContainer)
                Destroy(child.gameObject);
            
            // Populate properties
            foreach (var property in data.properties)
            {
                var go = Instantiate(propertyPrefab, propertiesContainer);
                var propertyUI = go.GetComponent<PropertyUI>();
                propertyUI.SetProperty(property);
            }
            // Populate upgrades
            foreach (var upgrade in data.upgrades)
            {
                var go = Instantiate(upgradePrefab, upgradesContainer);
                var upgradeUI = go.GetComponent<UpgradeUI>();
                upgradeUI.SetUpgrade(upgrade);
            }
        }
    }
}
