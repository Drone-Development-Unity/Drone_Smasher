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
        
        [Header("Necessary prefabs")]

        [SerializeField] private GameObject propertyPrefab; //ObjectPropertiesScrollView object
        [SerializeField] private GameObject upgradePrefab;//ObjectUpgradesScrollView object
        [SerializeField] private GameObject StatsPanelObject;
        
        private TextMeshProUGUI objectName;
        private TextMeshProUGUI descriptionText;
        private Transform propertiesContainer;
        private Transform upgradesContainer;
        private void Start()
        {
            StatsPanelReferences refs = StatsPanelObject.GetComponent<StatsPanelReferences>();
            objectName = refs.objectName;
            descriptionText = refs.descriptionText;
            propertiesContainer = refs.propertiesContainer;
            upgradesContainer = refs.upgradesContainer;
        }
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
