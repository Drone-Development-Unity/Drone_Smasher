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
    public void SaveToDTO()
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
    }
    [ContextMenu("Wczytaj z DTO")]
    public void LoadFromDTO()
    {
        if (saveData == null || saveData.Count == 0) return;
        
        var currentChildren = GetComponentsInChildren<Transform>()
            .Where(t => t.gameObject.layer == LayerMask.NameToLayer("Interactable"))
            .ToList();
        
        foreach (var dto in saveData)
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
