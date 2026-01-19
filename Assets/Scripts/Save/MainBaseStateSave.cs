using System.Collections.Generic;
using Save.DTO;
using UnityEngine;
using System.Linq;
using Game;
using Unity.VisualScripting;
using Unit = Game.Unit;
[System.Serializable]
public class CannonsData
{
    public string CannonObjectName; //only to preview
    public GameObject cannonContainer;
    public GameObject Placeholder;
}

public class MainBaseStateSave : MonoBehaviour
{
    public List<MainBaseDataDTO> saveData = new();
    public GameObject mainBase;
    public List<CannonsData> baseCanonData;
    void Start()
    {
        //SaveToDTO();
        //LoadFromDTO();
    }
    
    [ContextMenu("Zapisz do DTO")]
    public List<MainBaseDataDTO> SaveToDTO()
    {
        var filteredChildren = mainBase.GetComponentsInChildren<Transform>()
            .Where(t => t.gameObject.layer == LayerMask.NameToLayer("Interactable"))
            .ToList();

        foreach (var cannon in filteredChildren)
        {
            Unit unitComponent = cannon.GetComponent<Unit>();

            if (unitComponent != null)
            {
                MainBaseDataDTO cannonDto = new MainBaseDataDTO
                {
                    cannonName = cannon.name,
                    cannonStatsData = unitComponent.SaveToDTO()
                };
                saveData.Add(cannonDto);
            }
        }
        Debug.Log("Saved");
        return saveData;
    }
    [ContextMenu("Wczytaj z DTO")]
    public void LoadFromDTO(List<MainBaseDataDTO> cannonsData)
    {
        if (cannonsData == null || cannonsData.Count == 0) return;
        
        var currentChildren = mainBase.GetComponentsInChildren<Transform>()
            .Where(t => t.gameObject.layer == LayerMask.NameToLayer("Interactable"))
            .ToList();
        
        foreach (var dto in cannonsData)
        {
            Transform targetTransform = currentChildren.FirstOrDefault(t => t.name == dto.cannonName);

            if (targetTransform != null)
            {
                Unit unitComponent = targetTransform.GetComponent<Unit>();
                if (unitComponent != null)
                {
                    unitComponent.LoadFromDTO(dto.cannonStatsData);
                }
            }
            else
            {
                Debug.LogWarning($"Nie znaleziono obiektu o nazwie {dto.cannonName} w bazie {name}[Load]");
            }
        }
        Debug.Log("Loaded");
    }
    
    [ContextMenu("ResetCannons")]
    private void ResetCannons()
    {
        foreach (var cannonData in baseCanonData)
        {
            if (cannonData != null && cannonData.cannonContainer != null)
            {
                //delete cannon
                var unit = cannonData.cannonContainer.GetComponentInChildren<Unit>(true);
                if (unit) unit.Destroy();
                
                //set placeholder
                if (cannonData.Placeholder)
                {
                    cannonData.Placeholder.SetActive(true);
                    Debug.Log($"placeholder activated {cannonData.Placeholder.name}");
                    var purchaser =cannonData.Placeholder.GetComponent<PurchasableObject>();
                    if (purchaser) purchaser.CannonDisposed();
                }
            }
        }
    }
}
