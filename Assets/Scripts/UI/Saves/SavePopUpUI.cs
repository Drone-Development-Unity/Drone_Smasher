using Assets.Scripts.SaveSystem;
using Assets.Scripts.SaveSystem.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Saves
{
    public class SavePopUpUI : MonoBehaviour
    {
        [Header("Labes")]
        [SerializeField] private TMP_Text titleLabel;
        [SerializeField] private TMP_Text levelLabel;
        [SerializeField] private TMP_Text playedHLabel;
        [SerializeField] private TMP_Text lastPlayedLabel;
        [SerializeField] private TMP_Text createdAtLabel;

        [Header("SaveTiles")]
        [SerializeField] private GameObject saveCointainer;

        [Header("PopUps")]
        [SerializeField] private GameObject saveInfoPopUp;
        [SerializeField] private GameObject confirmationPopUp;
        [SerializeField] private Button clearSaveBtn;

        private int currentSlotId = -1;
        private void Awake()
        {
            if (clearSaveBtn != null)
            {
                clearSaveBtn.onClick.AddListener(OnClearSaveBtn);
            }
            else
            {
                Debug.LogWarning("[SavePopUpUI] clearSaveBtn is NULL");
            }
        }

        public void UpdateMeta(MetaDataDTO meta) 
        {
            if (meta == null)
            {
                Debug.LogWarning("[SavePopUpUI] UpdateMeta called with NULL meta");
                return;
            }

            if (titleLabel == null ||
                levelLabel == null ||
                lastPlayedLabel == null ||
                createdAtLabel == null)
            {
                Debug.LogWarning("[SavePopUpUI] One or more TMP_Text references are NULL");
                return;
            }

            titleLabel.text = $"Save {meta.SlotId}";
            levelLabel.text = $"Level: {meta.Level}";
            //playedHLabel.text = $"Played: {meta.PlayedHours:F1}h";
            lastPlayedLabel.text = $"Last Played: {meta.LastPlayed}";
            createdAtLabel.text = $"Created At: {meta.CreatedAt}";

            currentSlotId = meta.SlotId;
        }

        public void OnClearSaveBtn()
        {
            if(saveInfoPopUp == null || confirmationPopUp == null || saveCointainer == null
                || currentSlotId < 0) return;

            SavesMenuUI savesMenuUI = saveCointainer.GetComponent<SavesMenuUI>();
            if (savesMenuUI == null)
            {
                Debug.LogWarning("[SavePopUpUI] SavesMenuUI not found on saveContainer");
                return;
            }

            SaveManager.Instance.ClearSave(currentSlotId);

            savesMenuUI.InitTiles();

            saveInfoPopUp.SetActive(false);
            confirmationPopUp.SetActive(false);
            gameObject.SetActive(false);

            
        }
    }
}
