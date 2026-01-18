using System.Collections.Generic;
using Save.DTO;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;
using Unit = Game.Unit;

public class MainBaseStateSave : MonoBehaviour
{
    public List<MainBaseDataDTO> saveData = new();

    void Start()
    {
        //SaveToDTO();
        //LoadFromDTO();
    }
    
    [ContextMenu("Zapisz do DTO")]
    public List<MainBaseDataDTO> SaveToDTO()
    {
        var filteredChildren = GetComponentsInChildren<Transform>()
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
        
        var currentChildren = GetComponentsInChildren<Transform>()
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
}
