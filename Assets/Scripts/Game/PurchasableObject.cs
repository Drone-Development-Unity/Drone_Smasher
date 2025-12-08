
using Game.Managers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game
{
    /// <summary>
    /// Implements HighlightObject and add abilities to trigger/enable shop window in StatsPanel
    /// </summary>
    public class PurchasableObject : HighlightObject
    {
        private GameUIManager uiManager;
        public Transform placeholder; //purchased object will be child of this placeholder
        public GameObject placeholderUI;//used to hide placeholder ui when purchased new cannon
        public bool isPurchased = false;
        private new void Start()
        {
            base.Start();
            uiManager = GameUIManager.Instance;
        }
        public override void OnPointerClick(PointerEventData eventData)
        {
            //if is not purchased - trigger shop menu
            if (!isPurchased)
            {
                base.OnPointerClick(eventData);
                if (uiManager != null)
                {
                    uiManager.ShowCannonShopWindow(placeholder, gameObject);
                    uiManager.TryHideUpgradesMenu();
                }
            }
        }

        public void CannonPurchased()
        {
            placeholderUI.SetActive(false);
            //Debug.Log("Hide Placeholder UI");
            isPurchased = true;
        }
        
    }
}