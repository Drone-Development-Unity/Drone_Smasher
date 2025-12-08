using Game.StatsPanel.CannonsContent;
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
        
        [Header("ShopWindow prefabs")]
        [SerializeField] private GameObject CannonShopObject; //used to trigger cannonshop window when place for cannon clicked
        [SerializeField] private GameObject upgradesWindow; //used to hide upgradesWindow if shop is being triggered
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

        public void ShowCannonShopWindow(Transform spawnedCannon, GameObject cannonPlaceholder)
        {
            CannonShopObject.SetActive(true);
            //Iterate for every cannon from shop and add transform of clicked object to them
            foreach (Transform child in CannonShopObject.transform)
            {
                //check if script 'CannonPurchaser' exists in child
                var cannonScript = child.GetComponent<CannonPurchaser>();
                if (cannonScript != null)
                {
                    cannonScript.spawnedCannon = spawnedCannon;
                    cannonScript.cannonPlaceholder = cannonPlaceholder;
                }
            }
        }

        public void TryHideShopMenu()
        {
            if(CannonShopObject.activeSelf)CannonShopObject.SetActive(false);
        }
        public void TryHideUpgradesMenu()
        {
            if(upgradesWindow.activeSelf)upgradesWindow.SetActive(false);
        }
    }
}
