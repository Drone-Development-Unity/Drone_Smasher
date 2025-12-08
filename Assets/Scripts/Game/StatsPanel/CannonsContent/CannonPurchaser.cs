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
            base.OnPointerClick(eventData);
            BuyCannon();
            var baseCannon = cannonPlaceholder.GetComponent<PurchasableObject>();
            if (baseCannon != null) baseCannon.HidePlaceholderUI();
            //else Debug.Log("Placeholder UI - null");
        }
    }
}