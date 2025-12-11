using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Game.StatsPanel.CannonsContent
{
    public class CannonPurchaser:HighlightObject
    {
        public Transform spawnedCannon;
        public GameObject  cannonPrefab;
        public GameObject cannonPlaceholder;
        private void BuyCannon()
        {
            GameObject newCannon = Instantiate(
                cannonPrefab,
                spawnedCannon.position,
                spawnedCannon.rotation,
                spawnedCannon);
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            //TODO add check if player have enough resources to buy and animations
            base.OnPointerClick(eventData);
            //hide popup
            var popup = GetComponent<PopupHandler>();
            if (popup != null) popup.HidePopup();
            
            BuyCannon(); //spawn cannon
            
            var baseCannon = cannonPlaceholder.GetComponent<PurchasableObject>();
            if (baseCannon != null) baseCannon.CannonPurchased();
            
        }
    }
}