using System;
using System.Collections.Generic;
using Save.DTO;
using Unity.VisualScripting;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// Set object stats to edit and display in StatsPanel UI
    /// </summary>
    public class Unit: MonoBehaviour,IStatsDisplay
    {
        [SerializeField] private StatsData _stats;

        private void Awake()
        {
            foreach(var upgrade in _stats.upgrades)
            {
                upgrade.baseObject = gameObject;
            }
            
        }

        private void Start()
        {
            var dto = SaveToDTO();
            LoadFromDTO(dto);
            LoadCurrenciesSpriteData();
        }

        public StatsData GetStats()
        {
            return _stats;
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }
        
        //save
        public StatsDataDTO SaveToDTO()
        {
            var dto = new StatsDataDTO
            {
                objectName = _stats.objectName,
                description = _stats.description,
                properties = new List<PropertyDataDTO>(),
                upgrades = new List<UpgradeDataDTO>()
            };
            
            foreach (var p in _stats.properties)
            {
                dto.properties.Add(new PropertyDataDTO {
                    propertyType = p.propertyType,
                    propertyValue = p.propertyValue
                });
            }
            
            foreach (var u in _stats.upgrades)
            {
                dto.upgrades.Add(new UpgradeDataDTO {
                    upgradeName = u.upgradeName,
                    upgradeCost = u.upgradeCost,
                    upgradeAmount = u.upgradeAmount,
                    targetPropertyName = u.targetPropertyName,
                    currencyId = u.currencyId,
                    upgradeComboValue = u.upgradeComboValue,

                    upgradeImageName = u.upgradeImage != null ? u.upgradeImage.name : "",
                });
            }

            return dto;
        }

        //load
        public void LoadFromDTO(StatsDataDTO dto)
        {
            if (dto == null) return;

            _stats.objectName = dto.objectName;
            _stats.description = dto.description;
            
            _stats.properties.Clear();
            foreach (var pDto in dto.properties)
            {
                _stats.properties.Add(new PropertyData {
                    propertyType = pDto.propertyType,
                    propertyValue = pDto.propertyValue
                });
            }
            
            _stats.upgrades.Clear();
            foreach (var uDto in dto.upgrades)
            {
                var newUpgrade = new UpgradeData {
                    upgradeName = uDto.upgradeName,
                    upgradeCost = uDto.upgradeCost,
                    upgradeAmount = uDto.upgradeAmount,
                    targetPropertyName = uDto.targetPropertyName,
                    currencyId = uDto.currencyId,
                    upgradeComboValue = uDto.upgradeComboValue,
                    baseObject = gameObject
                };
                //load upgrade icon from /Resources (from upgrade icon name)
                if (!string.IsNullOrEmpty(uDto.upgradeImageName))
                    newUpgrade.upgradeImage = Resources.Load<Sprite>(uDto.upgradeImageName);

                _stats.upgrades.Add(newUpgrade);
            }
        }

        public void LoadCurrenciesSpriteData()
        {
            foreach (var upgrade in _stats.upgrades)
            {
                upgrade.upgradeCurrencyImage = CurrencyManager.Instance.GetCurrencyInstance(upgrade.currencyId).icon;
            }
        }
    }
}