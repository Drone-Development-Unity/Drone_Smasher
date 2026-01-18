using Assets.Scripts.SaveSystem;
using Assets.Scripts.SaveSystem.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Saves
{
    public class SaveTile : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private TMP_Text playedText;
        [SerializeField] private Button infoBtn;
        [SerializeField] private Button playBtn;

        private int slotNumber;
        private MetaDataDTO currentMeta;

        public void Setup(int slotIndex, MetaDataDTO meta)
        {
            slotNumber = slotIndex + 1;
            currentMeta = meta;

            UpdateTexts(meta);

            // info btn
            infoBtn.onClick.RemoveAllListeners();
            infoBtn.onClick.AddListener(OnInfoClicked);
            // play btn
            playBtn.onClick.RemoveAllListeners();
            playBtn.onClick.AddListener(OnPlayClicked);
        }

        private void UpdateTexts(MetaDataDTO meta)
        {
            if (!meta.Exists)
            {
                levelText.text = "LEVEL: --";
                playedText.text = "PLAYED HR: 0h";
            }
            else
            {
                levelText.text = $"LEVEL: {meta.Level}";
                playedText.text = $"PLAYED HR: {meta.PlayedHours:F1}h";
            }
        }

        private void OnInfoClicked()
        {
            if (SelectGameMenu.Instance != null && currentMeta != null)
            {
                SelectGameMenu.Instance.ShowInfo(currentMeta);
            }
            else
            {
                Debug.LogWarning("SelectGameMenu singleton nie został ustawiony!");
            }
        }

        private void OnPlayClicked()
        {
            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.SetSelectedSlot(slotNumber);
                SceneManager.LoadScene("GameScene");
            }
            else
            {
                Debug.LogWarning("SaveManager nie został ustawiony!");
            }
        }
    }
}
