using Save.DTO;
using UnityEngine;

public class DroneSaveSystem : MonoBehaviour
{
   public GameObject droneShop;
   public Transform gatherersContainer;
   public Transform droneSpawnLocation;
   public GameObject dronePrefab;
   private GatherersDataDTO gathererDataDTO = new();
   public void LoadGatherers(GatherersDataDTO gatherers)
   {
      var shopScript = droneShop.GetComponent<DroneShopLogic>();
      if(shopScript) SpawnLoadedDrones(gatherers.amount);
   }

   public GatherersDataDTO SaveGatherers()
   {
      var shopScript = droneShop.GetComponent<DroneShopLogic>();
      if (shopScript)
      {
         gathererDataDTO.amount = shopScript.CountDrones();
      }
      else gathererDataDTO.amount = 0;
   
      return gathererDataDTO;
   }
   
   public void SpawnLoadedDrones(int numberOfDrones)
   {
      for (int i = 0; i < numberOfDrones; i++) {
         Instantiate(dronePrefab, droneSpawnLocation.position, Quaternion.identity, gatherersContainer);
      }
   }
}
