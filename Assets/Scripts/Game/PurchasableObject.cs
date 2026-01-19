
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
        public bool isPurchased = false;
        private new void Start()
        {
            base.Start();
            uiManager = GameUIManager.Instance;
            if(isPurchased)CannonPurchased(); //deactivate script if cannon is purchased
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
                }
            }
        }

        public void CannonPurchased()
        {
            gameObject.SetActive(false);
            //Debug.Log("Hide Placeholder UI");
            isPurchased = true;
            //deactivate script when purchased cannon
            enabled = false;
            uiManager.TryHideShopMenu(); //hide shop menu after purchase
        }
        public void CannonDisposed()
        {
            //gameObject.SetActive(true);
            //Debug.Log("Hide Placeholder UI");
            isPurchased = false;
            //deactivate script when disposed cannon
            enabled = true;
            uiManager.TryHideShopMenu(); //hide shop menu after dispose
            
            //active childs
            foreach (Transform child in gameObject.transform)
            {
                child.gameObject.SetActive(true);
            }
        }
        
    }
}