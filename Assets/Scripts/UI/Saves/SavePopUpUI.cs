using Assets.Scripts.SaveSystem.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI.Saves
{
    public class SavePopUpUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text titleLabel;
        [SerializeField] private TMP_Text levelLabel;
        [SerializeField] private TMP_Text playedHLabel;
        [SerializeField] private TMP_Text lastPlayedLabel;
        [SerializeField] private TMP_Text createdAtLabel;

        public void UpdateMeta(MetaDataDTO meta) 
        {
            titleLabel.text = $"Save {meta.SlotId}";
            levelLabel.text = $"Level: {meta.Level}";
            playedHLabel.text = $"Played: {meta.PlayedHours:F1}h";
            lastPlayedLabel.text = $"Last Played: {meta.LastPlayed}";
            createdAtLabel.text = $"Created At: {meta.CreatedAt}";
        }
    }
}
